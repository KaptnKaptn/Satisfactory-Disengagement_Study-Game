using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameController;
    private PlatformManager platformManager;
    private GameStateManager gameStateManager;
    private AudioManager audioManager;

    public List<GameObject> goalPrefabList;
    public GameObject goalPrefab;

    [Header("Game Duration")]
    public int gameDurationInSec;
    public int backgroundChangeThreshold;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // gameController = GameObject.FindGameObjectWithTag("GameController");
        //platformManager = gameController.GetComponent<PlatformManager>();
        //gameStateManager = gameController.GetComponent<GameStateManager>();
        //audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            gameController = GameObject.FindGameObjectWithTag("GameController");
            platformManager = gameController.GetComponent<PlatformManager>();
            gameStateManager = gameController.GetComponent<GameStateManager>();

            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }
    }

    public int GetPlatformCount()
    {
        return platformManager.platformsGenerated;
    }

    public int GetPlatformsPassed()
    {
        return platformManager.platformsPassed;
    }

    public void SetPlatformsPassed(int count)
    {
        platformManager.platformsPassed = count;
    }

    public bool PlatformsFinished()
    {
        return platformManager.finished;
    }

    public float GetGameTime()
    {
        return gameStateManager.gameTime;
    }

    public void EndGame(string endType)
    {
        gameStateManager.GameWon(endType);
    }

    public void SelectPrefab(int index)
    {
        goalPrefab = goalPrefabList[index];
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void PlaySFX(string clip)
    {
        audioManager.PlaySFX(clip);
    }

    public void LoopSFX(string clip)
    {
        audioManager.LoopSFX(clip);
    }

    public void StopLoopSFX()
    {
        audioManager.StopLoopSFX();
    }

    public void StopBackgroundMusic()
    {
        audioManager.StopBackgroundMusic();
    }
}
