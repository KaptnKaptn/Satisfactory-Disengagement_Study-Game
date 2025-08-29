using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Both_GoalPlatform : MonoBehaviour
{
    private GameManager gameManager;
    private Player_Joystick player;

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
        if (player != null)
        {
            if (player.IsGrounded())
            {
                player.EndGame();
                spriteRenderer.sprite = doorOpen;
                gameManager.EndGame("both");
                player = null;
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

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = null;
        }
    }
}
