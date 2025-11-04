using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

//mediator between the different manager classes
//handles the game's base logic
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameDataManger dataLogger;
    public GameObject inputField;
    public GameObject menuButton;
    public TextMeshProUGUI playerIDInput;

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
            //gameController = GameObject.FindGameObjectWithTag("GameController");
            //platformManager = gameController.GetComponent<PlatformManager>();
            //gameStateManager = gameController.GetComponent<GameStateManager>();
        }
    }

    void Start()
    {
        dataLogger = GameDataManger.instance;

        if (dataLogger.HasID())
        {
            SkipIDInput();
        }

        levelLoader = LevelLoader.instance;

        audioManager.StartMusic();
    }

    void OnLevelWasLoaded(int level)
    {
        //Game Level
        if (level == 1)
        {
            gameController = GameObject.FindGameObjectWithTag("GameController");
            platformManager = gameController.GetComponent<PlatformManager>();
            gameStateManager = gameController.GetComponent<GameStateManager>();

            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
            StartCoroutine(SetupGame());
        }

        //Tutorial Level
        if (level == 2)
        {
            gameController = GameObject.FindGameObjectWithTag("GameController");
            gameStateManager = gameController.GetComponent<GameStateManager>();

            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
            StartCoroutine(SetupTutorial());
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

    public void LoadTutorial()
    {
        levelLoader.LoadTutorial();
    }

    private void StartGame()
    {
        levelLoader.StartGame();
        audioManager.StartMusic();
        gameStateManager.gameTime = 0;
        Debug.Log("Time = 0 ?");
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

        StartGame();
    }

    private IEnumerator SetupTutorial()
    {
        yield return new WaitForSeconds(1f);

        StartGame();
    }

    public void LoadMenu()
    {
        levelLoader.LoadMenu();
        Destroy(this.gameObject);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    private void SkipIDInput()
    {
        inputField.SetActive(false);
        menuButton.SetActive(true);
    }

    public void SetPlayerID()
    {
        dataLogger.SetID(playerIDInput.text);

    }

    public void SetVersionCondition(string condition)
    {
        dataLogger.SetupVersion(condition);
    }

    public void UpdateLatestPlatform(int platformID)
    {
        dataLogger.SetLatestPlatform(platformID);
    }

    public void AddEventToLog(string eventType)
    {
        dataLogger.AddEvent(eventType, gameStateManager.gameTime);
    }
}
