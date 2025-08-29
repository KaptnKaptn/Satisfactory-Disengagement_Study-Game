using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.Playables;

public class GameStateManager : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject player;

    [Header("Game End")]
    [SerializeField] private GameObject joystickCanvas;
    [SerializeField] private GameObject scoreScreen;

    [Header("Competence")]
    [SerializeField] private GameObject scoreWindow;
    [SerializeField] private GameObject rightStar;
    [SerializeField] private TextMeshProUGUI scoreText;

    public int scoreThreshold;
    public float scoreMultiplier;
    private float score;

    [Header("Closure")]
    public GameObject cutsceneRoom;
    public Vector3 teleportPos;
    public CameraFollow cameraScript;
    private PlayableDirector director;

    [Header("Game Timer")]
    public float gameTime;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        player = GameObject.FindGameObjectWithTag("Player");
        director = GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        gameTime = Time.time;
    }

    public void GameWon(string endType)
    {
        if (endType == "competence" || endType == "both")
        {
            score = gameManager.GetPlatformCount() * scoreMultiplier;

            scoreText.text = "Score: " + score;

            if (score > scoreThreshold)
            {
                rightStar.SetActive(true);
            }

            scoreWindow.SetActive(true);
        }

        if (endType == "closure" || endType == "both")
        {
            cutsceneRoom.SetActive(true);
            ClosureCutscene();
        }
        else
        {
            StartCoroutine(EndGame());
        }
    }

    private void ClosureCutscene()
    {
        joystickCanvas.SetActive(false);
        director.Play();
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);
        joystickCanvas.SetActive(false);
        scoreScreen.SetActive(true);
        gameManager.PlaySFX("Victory");
    }

    public void PlayerTeleport()
    {
        player.transform.position = teleportPos;
        player.transform.localScale = Vector3.one;
        cameraScript.gameEnd = true;
    }

    public void CutsceneEnd()
    {
        scoreScreen.SetActive(true);
        gameManager.StopBackgroundMusic();
        gameManager.PlaySFX("Victory");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
