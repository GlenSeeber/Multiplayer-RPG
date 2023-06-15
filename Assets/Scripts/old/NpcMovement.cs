using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    private float horizontal;
    private float vertical;

    public float speed = 10f;

    private Vector2 direction;

    public Rigidbody2D rb;

    public Transform player;

    public Transform npc;

    void FixedUpdate()
    {
	direction = (player.position - npc.position).normalized;

	rb.velocity = direction*speed;
    }
}
