using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Player Follow")]
    public Transform player;
    public float playerOffset;
    private float playerYPos;
    private Vector3 startPos;
    private Vector3 newPos;
    public bool gameEnd;

    [Header("Highest Position")]
    public float goalOffset;
    private Vector3 maxPos;
    private GameObject goalPlatform;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        newPos = startPos;

        maxPos = Vector3.positiveInfinity;
    }

    // Update is called once per frame
    void Update()
    {
        goalPlatform = GameObject.FindGameObjectWithTag("Finish");

        if (goalPlatform != null)
        {
            maxPos = goalPlatform.transform.position;
            maxPos.y -= goalOffset;
        }

        playerYPos = player.position.y + playerOffset;
        if (playerYPos != newPos.y && transform.position.y < maxPos.y && !gameEnd)
        {
            if (playerYPos > startPos.y)
            {
                newPos.y = playerYPos;
            }
            else
            {
                newPos = startPos;
            }

        }

        transform.position = newPos;

        if (gameEnd)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            newPos.y = -50;
            newPos.x = player.position.x + playerOffset / 2;
        }
    }
}
