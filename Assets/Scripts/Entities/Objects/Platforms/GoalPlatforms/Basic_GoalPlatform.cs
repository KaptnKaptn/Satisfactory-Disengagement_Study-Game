using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_GoalPlatform : Platform
{
    #region Components
    private GameManager gameManager;
    private Player_Joystick player;
    #endregion

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
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
                player = null;
                gameManager.EndGame("basic");
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
}
