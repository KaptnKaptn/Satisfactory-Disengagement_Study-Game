using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//camera movement script
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
        #region Max Height Calculation
        goalPlatform = GameObject.FindGameObjectWithTag("Finish");

        if (goalPlatform != null)
        {
            maxPos = goalPlatform.transform.position;
            maxPos.y -= goalOffset;
        }
        #endregion

        #region Player Follow
        playerYPos = player.position.y + playerOffset;

        if (gameEnd)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            newPos.y = -50;
            newPos.x = player.position.x + playerOffset / 2;
        }

        else if (playerYPos != newPos.y && transform.position.y < maxPos.y)
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

        else if (playerYPos < transform.position.y)
        {
            newPos.y = playerYPos;
        }

        transform.position = newPos;
        #endregion

    }
}
