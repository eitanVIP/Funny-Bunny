using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBlock : MonoBehaviour
{
    public float Gravity;

    void Start()
    {
        transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, Gravity > 0 ? -90 : 90);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.collider.transform.Rotate(0, 0, collision.rigidbody.gravityScale * collision.rigidbody.gravityScale * Gravity < 0 ? 180 : 0);

        if (!collision.collider.CompareTag("Clone"))
            collision.rigidbody.gravityScale *= Gravity;
        else
            collision.collider.GetComponent<Duplicant>().g *= Gravity;

        if (collision.collider.CompareTag("Player"))
            Destroy(gameObject);
    }
}