using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public LevelLoader levelLoader;
    public GameObject gameController;
    private PlatformManager platformManager;
    private GameStateManager gameStateManager;
    [SerializeField] private AudioManager audioManager;

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

        if (Application.isEditor)
        {
            gameController = GameObject.FindGameObjectWithTag("GameController");
            platformManager = gameController.GetComponent<PlatformManager>();
            gameStateManager = gameController.GetComponent<GameStateManager>();
        }
    }

    void Start()
    {
        levelLoader = LevelLoader.instance;

        audioManager.StartMusic();
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            gameController = GameObject.FindGameObjectWithTag("GameController");
            platformManager = gameController.GetComponent<PlatformManager>();
            gameStateManager = gameController.GetComponent<GameStateManager>();

            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
            StartCoroutine(SetupGame());
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
        levelLoader.LoadGame();
    }

    private void StartGame()
    {
        levelLoader.StartGame();
        audioManager.StartMusic();
        gameStateManager.gameTime = 0;
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

    private IEnumerator SetupGame()
    {
        while (!platformManager.setup)
        {
            yield return new WaitForSeconds(1);
        }
        //yield return WaitUntilTrue(platformManager.setup);

        StartGame();
    }

    private IEnumerator WaitUntilTrue(bool check)
    {
        while (!check)
        {
            yield return null;
        }
    }
}
