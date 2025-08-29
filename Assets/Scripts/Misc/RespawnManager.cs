using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [Header("Player Respawn")]
    public GameObject respawnPortal;
    public GameObject failSafeSpeechBubble;

    // Update is called once per frame
    void Update()
    {
        if (respawnPortal.activeInHierarchy && !failSafeSpeechBubble.activeInHierarchy)
        {
            failSafeSpeechBubble.SetActive(true);
        }
    }
}
