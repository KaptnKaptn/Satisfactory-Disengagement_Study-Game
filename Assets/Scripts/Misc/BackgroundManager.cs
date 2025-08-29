using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Background Sprite")]
    public SpriteRenderer backgroundSprite;
    public List<Sprite> sprites;
    public int spriteChangeThreshold;

    [Header("Background Change")]
    public List<Color> colors;
    public float colorChangeSpeed;
    public float time;
    private float currentTime;
    private int colorIndex;

    private bool changing;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;

        spriteChangeThreshold = gameManager.backgroundChangeThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        #region Sprite Change Check
        if (gameManager.GetPlatformsPassed() >= spriteChangeThreshold &&
            !gameManager.PlatformsFinished() && !changing)
        {
            changing = true;
            gameManager.SetPlatformsPassed(0);
            gameManager.PlaySFX("Background");
        }
        #endregion

        #region Background Change Transition
        if (changing)
        {
            if (colorIndex < colors.Count)
            {
                ColorChange();
                UpdateIndex();
            }
            else
            {
                FadeIn();
            }
        }
        #endregion
    }

    private void GenerateRandomBackground()
    {
        int index = Random.Range(0, sprites.Count);
        backgroundSprite.sprite = sprites[index];
    }

    private void ColorChange()
    {
        backgroundSprite.color = Color.Lerp(backgroundSprite.color,
                                            colors[colorIndex],
                                            colorChangeSpeed * Time.deltaTime);
    }

    private void UpdateIndex()
    {
        if (currentTime <= 0)
        {
            colorIndex++;
            currentTime = time;
            if (colorIndex >= colors.Count)
            {
                GenerateRandomBackground();
            }
        }
        else
        {
            currentTime -= Time.deltaTime;
        }
    }

    private void FadeIn()
    {
        if (backgroundSprite.color != Color.white)
        {
            backgroundSprite.color = Color.Lerp(backgroundSprite.color,
                                            Color.white,
                                            colorChangeSpeed * Time.deltaTime);
        }
        else
        {
            changing = false;
            colorIndex = 0;
        }
    }

}
