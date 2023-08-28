using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("General")]
    [SerializeField] GameObject mochiWinner;

    [Header("Movement")]
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpingPower = 16f;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform[] groundCheck;
    [SerializeField] LayerMask groundLayer;

    Animator animator;
    FixedJoystick Joystick;
    GameObject finishMenu;
    settingsMenu settings;

    float horizontal;
    bool isFacingRight = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        Joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        settings = GameObject.FindWithTag("settingsMenu").GetComponent<settingsMenu>();

        finishMenu = GameObject.Find("finishMenu");
        finishMenu.SetActive(false);
    }

    void Update()
    {
        horizontal = Joystick.Horizontal + Input.GetAxisRaw("Horizontal");
        
        animator.SetBool("Walking", horizontal != 0);

        if (Input.GetKeyDown(KeyCode.Space))
            tryJump();

        Flip();

        if (transform.position.y <= -50)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            transform.position = new Vector3(transform.position.x, -20);
            GameObject.Find("Manager").GetComponent<Manager>().Menu("reset");
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color.RGBToHSV(sr.color, out float H, out float S, out float V);

        H += Time.deltaTime;
        S = 0.5f;
        V = 1;

        if (settings.rainbowMode)
            sr.color = Color.HSVToRGB(H, S, V);
        else
            sr.color = Color.white;

        GetComponent<AudioSource>().volume = settings.SFXVolume;
    }

    #region Movement
    void FixedUpdate()
    {
        if (GameObject.Find("Manager").GetComponent<Manager>().gameUI.activeInHierarchy || horizontal == 0)
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    public void tryJump()
    {
        if (IsGrounded())
            StartCoroutine(Jump());
    }

    IEnumerator Jump()
    {
        if (!GameObject.Find("Manager").GetComponent<Manager>().gameUI.activeInHierarchy)
            yield break;
        animator.SetTrigger("Jump");
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.1f);
        if (IsGrounded())
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapArea(groundCheck[0].position, groundCheck[1].position, groundLayer);
    }

    void Flip()
    {
        if (!GameObject.Find("Manager").GetComponent<Manager>().gameUI.activeInHierarchy)
            return;

        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    #endregion

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Finish"))
        {
            if(PlayerPrefs.GetInt("Level") < SceneManager.GetActiveScene().buildIndex)
                PlayerPrefs.SetInt("Level", SceneManager.GetActiveScene().buildIndex);

            Destroy(GameObject.Find("dupsHolder"));

            Instantiate(mochiWinner, transform.position, Quaternion.identity);

            StartCoroutine(GameObject.Find("Manager").GetComponent<Manager>().levelFinished(finishMenu));

            GetComponent<SpriteRenderer>().enabled = false;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 0.75f);
        }
        else if ((collider.CompareTag("Poop Trap") || collider.CompareTag("Wipe")) || collider.CompareTag("Cabbage") && !GameObject.FindWithTag("SceneTransitioner").GetComponent<SceneTransitioner>().isTransitioning)
            GameObject.Find("Manager").GetComponent<Manager>().Menu("reset");
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((groundCheck[0].position + groundCheck[1].position) / 2, groundCheck[1].position - groundCheck[0].position);
    }
}