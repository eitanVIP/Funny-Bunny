using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public enum AIType
    {
        RightLeft,
        RightLeftShoot,
        Smart,
        SmartShoot
    };

    [SerializeField] AIType type;
    [SerializeField] float Speed;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] float raycastLength;
    [SerializeField] float downRaycastLength;
    [SerializeField] LayerMask raycastHitLayer;
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] bool downRayFlip;

    void Update()
    {
        switch (type)
        {
            case AIType.RightLeft:
                transform.position += transform.right * Speed * Time.deltaTime;

                RaycastHit2D[] hits = Physics2D.RaycastAll(raycastOrigin.position, transform.right, raycastLength, raycastHitLayer);
                bool rightHit = false;
                foreach(var hit in hits)
                    if (hit.collider.gameObject != gameObject)
                        rightHit = true;

                RaycastHit2D[] hits2 = Physics2D.RaycastAll(raycastOrigin.position, Vector3.down, downRaycastLength, raycastHitLayer);
                bool downHit = false;
                foreach (var hit in hits2)
                    if (hit.collider.gameObject != gameObject)
                        downHit = true;

                if (rightHit || (!downHit && downRayFlip))
                {
                    renderer.flipY = !renderer.flipY;
                    transform.Rotate(Vector3.forward * 180);
                }
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(raycastOrigin.position, transform.right * raycastLength);
        Gizmos.DrawRay(raycastOrigin.position, Vector3.down * downRaycastLength);
    }
}