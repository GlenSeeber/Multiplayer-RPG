using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInputScript : MonoBehaviour
{
    public Vector2 move;

    private float horizontal;
    private float vertical;


    [SerializeField] private TextMeshProUGUI playerInputDisplay;

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        move = new Vector2(horizontal, vertical).normalized;

        playerInputDisplay.text = "Your Inputs: " + move.ToString();
    }
}
