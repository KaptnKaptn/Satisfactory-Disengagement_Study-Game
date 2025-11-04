using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalMovingPlatform : Platform
{
    #region Movement Variables
    [SerializeField] float speed;
    public float yOffset;
    private Vector2 startPos;
    private Vector2 goalPos;
    private Vector3 targetPos;
    #endregion

    #region Player Retainment Variables
    private bool playerPresent;
    private GameObject player;
    private Rigidbody2D playerRB;
    #endregion
    

    void Start()
    {
        #region  Movement-Area calculation
        startPos = new Vector2(transform.position.x, transform.position.y - yOffset);
        goalPos = new Vector2(transform.position.x, transform.position.y + yOffset);
        targetPos = startPos;
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region Self-Despawn
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        bool inCamera = viewportPos.x > -viewBuffer && viewportPos.x < 1 + viewBuffer * 10 &&
                        viewportPos.y > -viewBuffer && viewportPos.y < 1 + viewBuffer * 10;

        if (!inCamera)
        {
            Destroy(gameObject);
        }
        #endregion

        #region Platform Movement

        if (Vector2.Distance(transform.position, startPos) < 0.01f)
        {
            targetPos = goalPos;
        }
        else if (Vector2.Distance(transform.position, goalPos) < 0.01f)
        {
            targetPos = startPos;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        #endregion

        #region Player Retainment
        if (player != null)
        {
            if (player.GetComponent<Player_Joystick>().respawning)
            {
                playerPresent = false;
            }

            if (playerPresent)
            {
                if (playerRB.linearVelocity == Vector2.zero)
                {
                    player.transform.parent = this.transform;
                }
                else
                {
                    player.transform.parent = null;
                }
            }
        }
        #endregion
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerRB = collision.GetComponent<Rigidbody2D>();
            player = collision.gameObject;
            playerPresent = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        playerPresent = false;
    }

}
