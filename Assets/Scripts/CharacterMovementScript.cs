using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementScript : MonoBehaviour
{
    public bool player = false;

    [SerializeField] private float speed = 2.0f;

    private Rigidbody2D rb;
    
    private PlayerInputScript playerInput;

    [SerializeField] private ClientScript clientScript;

    private Vector2 direction;
    
    static int idCount = 0;

    public int id;


    void Awake()
    {
        if (!player)
        {
            //set the npc id to idCount, then incriment it for the next guy
            id = idCount;
            idCount++;
        }
        else
        {
            playerInput = GetComponent<PlayerInputScript>();
        }
        rb = GetComponent<Rigidbody2D>();


    }

    void FixedUpdate()
    {

        //get input either from player or npc script
        if (player)
        {
            direction = playerInput.move;
        }
        else
        {
            direction = clientScript.clientDirection;
        }

        //apply movement
        rb.velocity = direction*speed;
    }
}
