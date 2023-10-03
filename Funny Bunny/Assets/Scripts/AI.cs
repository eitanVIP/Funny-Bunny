using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    public enum AIType
    {
        RightLeft,
        Smart,
        Random
    };

    [SerializeField] AIType type;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float Speed;
    [SerializeField] Vector2 jumpForce;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] float raycastLength;
    [SerializeField] float downRaycastLength;
    [SerializeField] LayerMask raycastHitLayer;
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] bool downRayFlip;
    [SerializeField] bool enableSmartAttackTimer;
    [SerializeField] GameObject poopPrefab;
    Vector2 Down;
    float smartAttackTimer = 0;

    void Start()
    {
        Down = -transform.up;
    }

    void Update()
    {
        smartAttackTimer -= Time.deltaTime;

        if (GameObject.Find("Manager").GetComponent<Manager>().gameStoped || smartAttackTimer > 0)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        switch (type)
        {
            case AIType.RightLeft:
                MoveRightLeft();
                break;

            case AIType.Smart:
                MoveSmartly();
                break;

            case AIType.Random:
                MoveRandomly();
                break;
        }
    }

    void MoveRightLeft()
    {
        rb.velocity = transform.right * Speed;

        RaycastHit2D[] hits = Physics2D.RaycastAll(raycastOrigin.position, Down, downRaycastLength, raycastHitLayer);
        bool downHit = hits.ToList().Where(x => x.collider.gameObject != gameObject).ToArray().Length != 0;

        RaycastHit2D[] hits2 = Physics2D.RaycastAll(raycastOrigin.position, transform.right, raycastLength, raycastHitLayer);
        bool rightHit = hits2.ToList().Where(x => x.collider.gameObject != gameObject).ToArray().Length != 0;

        if (rightHit || (!downHit && downRayFlip))
        {
            renderer.flipY = !renderer.flipY;
            transform.Rotate(Vector3.forward * 180);
        }
    }

    void MoveSmartly()
    {
        if (!GameObject.FindWithTag("Player"))
            return;

        float playerPosX = GameObject.FindWithTag("Player").transform.position.x;

        if (Mathf.Abs(playerPosX - transform.position.x) > 0.25f)
            rb.velocityX = transform.right.x * Speed;
        else
            rb.velocityX = 0f;

        if(GameObject.FindWithTag("Player"))
            transform.right = ((playerPosX - transform.position.x) * Vector2.right).normalized * (rb.gravityScale < 0 ? -1 : 1);

        if (rb.gravityScale < 0)
            transform.Rotate(Vector3.forward * 180);

        RaycastHit2D[] hits = Physics2D.RaycastAll(raycastOrigin.position, -transform.up, downRaycastLength, raycastHitLayer);
        bool downHit = hits.ToList().Where(x => x.collider.gameObject != gameObject).ToArray().Length != 0;

        if (downHit && Mathf.Abs(rb.velocityY) <= 0.1f)
            rb.AddForce(jumpForce * transform.up, ForceMode2D.Impulse);
    }

    void MoveRandomly()
    {
        if (rb.gravityScale < 0)
            transform.Rotate(Vector3.forward * 180);

        RaycastHit2D[] hits = Physics2D.RaycastAll(raycastOrigin.position, -transform.up, downRaycastLength, raycastHitLayer);
        bool downHit = hits.ToList().Where(x => x.collider.gameObject != gameObject).ToArray().Length != 0;

        RaycastHit2D[] hits2 = Physics2D.RaycastAll(raycastOrigin.position, transform.right, raycastLength, raycastHitLayer);
        bool rightHit = hits2.ToList().Where(x => x.collider.gameObject != gameObject).ToArray().Length != 0;

        if (downHit && Mathf.Abs(rb.velocityY) <= 0.1f)
        {
            if (rightHit)
                transform.right = -transform.right;

            Vector2 Jump = jumpForce * new Vector2(transform.right.x, 1);
            rb.AddForce(Jump, ForceMode2D.Impulse);

            GetComponent<Animator>().SetTrigger("Jump");

            AudioSource audio = GetComponent<AudioSource>();
            audio.volume = GameObject.FindWithTag("settingsMenu").GetComponent<settingsMenu>().SFXVolume;
            audio.Play();

            Collider2D[] colls = Physics2D.OverlapAreaAll(transform.position + new Vector3(-1, -0.5f), transform.position + new Vector3(1, -1.5f));

            colls = colls.ToList().Where(x => x.gameObject.CompareTag("Poop Trap")).ToArray();

            if (colls.Length == 0)
                Instantiate(poopPrefab, transform.position - 0.8f * transform.up, Quaternion.identity);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (type != AIType.Smart || !(collider.CompareTag("Player") || collider.CompareTag("Clone")) || GameObject.Find("Manager").GetComponent<Manager>().gameStoped)
            return;

        if(enableSmartAttackTimer)
            smartAttackTimer = 1;

        GetComponent<Animator>().SetTrigger("Attack");

        AudioSource audio = GetComponents<AudioSource>()[Random.Range(0, 2)];

        audio.volume = GameObject.FindWithTag("settingsMenu").GetComponent<settingsMenu>().SFXVolume;
        audio.Play();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(raycastOrigin.position, transform.right * raycastLength);
        Gizmos.DrawRay(raycastOrigin.position, -transform.up * downRaycastLength);
        Gizmos.DrawWireCube(transform.position + new Vector3(0, -1), new Vector3(2, 1));
    }
}