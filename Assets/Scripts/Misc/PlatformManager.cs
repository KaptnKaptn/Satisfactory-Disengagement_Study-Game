using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Platform Generation")]
    public GameObject platformPrefab;
    public GameObject horizontalPlatformPrefab;
    public GameObject verticalPlatformPrefab;
    public GameObject goalPlatformPrefab;
    public int platformCount;

    public float xRange;
    public float yMin;
    public float yMax;
    public float xOffset;
    private Vector3 spawnPos;

    private float lastX;
    private float lastY;

    private List<GameObject> platforms;

    private bool generating = true;
    public bool finished = false;

    public int platformsGenerated;
    public int platformsPassed;

    [SerializeField] private GameObject baseTopPrefab;

    public int gameLength;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;

        goalPlatformPrefab = gameManager.goalPrefab;
        gameLength = gameManager.gameDurationInSec;

        platforms = new List<GameObject>();

        lastX = transform.position.x;
        lastY = transform.position.y;

        spawnPos = new Vector3();

        for (int i = 0; i < platformCount; i++)
        {
            CalculatePlatform();
        }
    }

    // Update is called once per frame
    void Update()
    {
        #region Platform Generation
        if (gameManager.GetGameTime() >= gameLength && !finished)
        {
            finished = true;
            generating = false;
            GeneratePlatform(goalPlatformPrefab);

            Vector3 topPos = new Vector3(0, lastY + 4, 0);
            Instantiate(baseTopPrefab, topPos, Quaternion.identity);
        }

        CleanupDestroyedPlatforms();
        if (platforms.Count < platformCount && generating)
        {
            CalculatePlatform();
        }
        #endregion
    }

    private void CalculatePlatform()
    {
        float platformProb = Random.Range(0f, 1f);

        if (platformProb <= 0.8f)
        {
            GeneratePlatform(platformPrefab);
        }
        else if (platformProb > 0.9f)
        {
            GeneratePlatform(horizontalPlatformPrefab);
        }
        else
        {
            GeneratePlatform(verticalPlatformPrefab);
        }
    }

    private void GeneratePlatform(GameObject platformType)
    {
        if (platformType == goalPlatformPrefab)
        {
            spawnPos.y = Random.Range(yMin, yMax) + lastY;
            spawnPos.x = 0;
        }

        else
        {
            int attempts = 0;

            do
            {
                spawnPos.y = Random.Range(yMin, yMax) + lastY;
                spawnPos.x = Random.Range(-xRange, xRange);
                attempts++;
            }
            while (Mathf.Abs(lastX - spawnPos.x) < xOffset && attempts < 50);
        }

        lastX = spawnPos.x;
        lastY = spawnPos.y;


        GameObject platform = Instantiate(platformType, spawnPos, Quaternion.identity);

        if (platformType == horizontalPlatformPrefab)
        {
            HorizontalMovingPlatform horizonalPlatform = platform.GetComponent<HorizontalMovingPlatform>();
            horizonalPlatform.xPos = spawnPos.x;
            horizonalPlatform.xRange = new Vector2(-xRange, xRange);
        }

        platforms.Add(platform);
        platformsGenerated++;
    }

    private void CleanupDestroyedPlatforms()
    {
        foreach (GameObject platform in platforms)
        {
            if (platform == null)
            {
                platformsPassed++;
            }
        }
        platforms.RemoveAll(item => item == null);
    }

}
