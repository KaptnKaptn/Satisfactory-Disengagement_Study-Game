using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Closure_GoalPlatform : Platform
{
    #region Components
    private GameManager gameManager;
    private Player_Joystick player;
    #endregion

    [Header("Closure")]
    public Sprite doorOpen;
    private SpriteRenderer spriteRenderer;


    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        #region Goal Condition
        if (player != null)
        {
            //check if player is grounded on goal platform
            if (player.IsGrounded())
            {
                player.EndGame();
                spriteRenderer.sprite = doorOpen;
                gameManager.EndGame("closure");
                player = null;
            }
        }
        #endregion
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = collision.GetComponent<Player_Joystick>();
        }
    }
    
    //remove player to stop goal condition
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = null;
        }
    }
}
