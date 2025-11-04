using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//manager handling the effects and UI behind a player respawn
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
