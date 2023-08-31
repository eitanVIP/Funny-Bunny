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
    [SerializeField] float Speed;
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

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(raycastOrigin.position, transform.right * raycastLength);
        Gizmos.DrawRay(raycastOrigin.position, Down * downRaycastLength);
    }
}