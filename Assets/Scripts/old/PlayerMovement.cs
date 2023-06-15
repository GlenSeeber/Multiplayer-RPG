using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float vertical;

    public float speed = 10f;

    private Vector2 direction;

    public Rigidbody2D rb;

    void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

	    direction = new Vector2(horizontal, vertical).normalized;

	    rb.velocity = direction*speed;
    }
}
