using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public enum AIType
    {
        RightLeft,
        Smart
    };

    [SerializeField] AIType type;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float Speed;
    [SerializeField] float jumpForce;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] float raycastLength;
    [SerializeField] float downRaycastLength;
    [SerializeField] LayerMask raycastHitLayer;
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] bool downRayFlip;
    Vector2 Down;

    void Start()
    {
        Down = -transform.up;
    }

    void Update()
    {
        if (GameObject.Find("Manager").GetComponent<Manager>().gameStoped)
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
        }
    }

    void MoveRightLeft()
    {
        rb.velocity = transform.right * Speed;

        RaycastHit2D[] hits = Physics2D.RaycastAll(raycastOrigin.position, transform.right, raycastLength, raycastHitLayer);
        bool rightHit = false;
        foreach (var hit in hits)
            if (hit.collider.gameObject != gameObject)
                rightHit = true;

        RaycastHit2D[] hits2 = Physics2D.RaycastAll(raycastOrigin.position, Down, downRaycastLength, raycastHitLayer);
        bool downHit = false;
        foreach (var hit in hits2)
            if (hit.collider.gameObject != gameObject)
                downHit = true;

        if (rightHit || (!downHit && downRayFlip))
        {
            renderer.flipY = !renderer.flipY;
            transform.Rotate(Vector3.forward * 180);
        }
    }

    void MoveSmartly()
    {
        float playerPosX = GameObject.FindWithTag("Player").transform.position.x;

        if (Mathf.Abs(playerPosX - transform.position.x) > 0.25f)
            rb.velocityX = transform.right.x * Speed;

        if(GameObject.FindWithTag("Player"))
            transform.right = ((playerPosX - transform.position.x) * Vector2.right).normalized * (rb.gravityScale < 0 ? -1 : 1);

        if (rb.gravityScale < 0)
            transform.Rotate(Vector3.forward * 180);

        RaycastHit2D[] hits2 = Physics2D.RaycastAll(raycastOrigin.position, Down, downRaycastLength, raycastHitLayer);
        bool downHit = false;
        foreach (var hit in hits2)
            if (hit.collider.gameObject != gameObject)
                downHit = true;
        
        if (downHit && Mathf.Abs(rb.velocityY) <= 0.1f)
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (type != AIType.Smart || !(collider.CompareTag("Player") || collider.CompareTag("Clone")) || GameObject.Find("Manager").GetComponent<Manager>().gameStoped)
            return;

        GetComponent<Animator>().SetTrigger("Attack");

        AudioSource audio = GetComponents<AudioSource>()[Random.Range(0, 2)];

        audio.volume = GameObject.FindWithTag("settingsMenu").GetComponent<settingsMenu>().SFXVolume;
        audio.Play();
    }

    void OnDrawGizmos()
    {
        if(type == AIType.RightLeft)
        {
            Gizmos.DrawRay(raycastOrigin.position, transform.right * raycastLength);
            Gizmos.DrawRay(raycastOrigin.position, Down * downRaycastLength);
        }
    }
}