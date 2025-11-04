using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//manager handling the game data logged during a session
public class GameDataManger : MonoBehaviour
{
    public static GameDataManger instance;

    [SerializeField] private string userID;
    [SerializeField] private string versionCondition;
    [SerializeField] private int latestPlatform;

    private static string reportDirectoryName = "Report";
    private static string reportFileName = "report.csv";
    private static string reportSeperator = ";";
    private static string[] reportHeaders = new string[5] {
        "UserID",
        "Version",
        "LastPlatform",
        "EventType",
        "Timestamp"
    };

    public int timeDecimalsToRound;

    private static void AppendEvent(string[] logData)
    {
        VerifyFile();
        using (StreamWriter sw = File.AppendText(GetFilePath()))
        {
            string eventString = "";
            for (int i = 0; i < logData.Length; i++)
            {
                if (eventString != "")
                {
                    eventString += reportSeperator;
                }
                eventString += logData[i];
            }
            sw.WriteLine(eventString);
        }
    }

    private static void CreateFile()
    {
        VerifyDirectory();
        using (StreamWriter sw = File.CreateText(GetFilePath()))
        {
            string headerString = "";

            for (int i = 0; i < reportHeaders.Length; i++)
            {
                if (headerString != "")
                {
                    headerString += reportSeperator;
                }
                headerString += reportHeaders[i];
            }
            sw.WriteLine(headerString);
        }
    }

    static void VerifyDirectory()
    {
        string directory = GetDirectoryPath();
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    static void VerifyFile()
    {
        string file = GetFilePath();
        if (!File.Exists(file))
        {
            CreateFile();
        }
    }

    static string GetDirectoryPath()
    {
        return Application.persistentDataPath + "/" + reportDirectoryName;
    }

    static string GetFilePath()
    {
        return GetDirectoryPath() + "/" + reportFileName;
    }

    public void DestroyFile()
    {
        File.Delete(GetFilePath());
    }

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

        Debug.Log(GetDirectoryPath());
    }

    public void SetID(string id)
    {
        userID = id;
    }

    public bool HasID()
    {
        return userID != "";
    }

    public void SetupVersion(string condition)
    {
        versionCondition = condition;
    }

    public void SetLatestPlatform(int platformID)
    {
        latestPlatform = platformID;
    }

    public void AddEvent(string eventType, float timeOfEvent)
    {
        string[] logData = new string[5] {
        userID,
        versionCondition,
        latestPlatform.ToString(),
        eventType,
        FormatTime(timeOfEvent).ToString()
       };

        AppendEvent(logData);
    }

    private float FormatTime(float time)
    {
        float newTime = Mathf.Round(time * Mathf.Pow(10, timeDecimalsToRound));
        return newTime * Mathf.Pow(10, -timeDecimalsToRound);
    }
}
