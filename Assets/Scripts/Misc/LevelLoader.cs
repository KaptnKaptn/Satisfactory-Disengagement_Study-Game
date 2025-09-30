using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    public Animator fadeAnim;
    public float transitionTime;

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
    }

    public void LoadGame()
    {
        StartCoroutine(LoadScene(1));
    }

    public void LoadMenu()
    {
        StartCoroutine(LoadScene(0));
    }

    public void LoadTutorial()
    {
        StartCoroutine(LoadScene(2));
    }

    public void StartGame()
    {
        fadeAnim.SetTrigger("crossfade");
    }

    private IEnumerator LoadScene(int sceneIndex)
    {
        fadeAnim.SetTrigger("crossfade");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            StartGame();
        }
    }
}
