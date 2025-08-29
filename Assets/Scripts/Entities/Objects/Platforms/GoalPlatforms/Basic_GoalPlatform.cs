using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_GoalPlatform : MonoBehaviour
{
    [Header("Basic Goal Platform")]
    private GameManager gameManager;
    private Player_Joystick player;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (player != null)
        {
            if (player.IsGrounded())
            {
                player.EndGame();
                gameManager.EndGame("basic");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = collision.GetComponent<Player_Joystick>();
        }
    }
}
