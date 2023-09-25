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
        if (!GameObject.Find("Manager").GetComponent<Manager>().gameStoped)
            transform.position += transform.right * Speed * Time.deltaTime;

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
        if (!GameObject.Find("Manager").GetComponent<Manager>().gameStoped)
            rb.velocityX = transform.right.x * Speed;

        if(GameObject.FindWithTag("Player"))
            transform.right = ((GameObject.FindWithTag("Player").transform.position.x - transform.position.x) * Vector2.right).normalized;

        RaycastHit2D[] hits2 = Physics2D.RaycastAll(raycastOrigin.position, Down, downRaycastLength, raycastHitLayer);
        bool downHit = false;
        foreach (var hit in hits2)
            if (hit.collider.gameObject != gameObject)
                downHit = true;

        if (downHit && rb.velocityY == 0)
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(raycastOrigin.position, transform.right * raycastLength);
        Gizmos.DrawRay(raycastOrigin.position, Down * downRaycastLength);
    }
}