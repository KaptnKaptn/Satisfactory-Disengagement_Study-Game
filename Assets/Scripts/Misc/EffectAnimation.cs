using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimation : MonoBehaviour
{
    public Sprite[] frames;
    public float frameTime = 0.1f;
    public bool looping;
    private SpriteRenderer sr;
    private int currentFrame;
    private float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        currentFrame = 0;
        timer = 0f;
        if (frames.Length > 0) sr.sprite = frames[0];
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= frameTime)
        {
            timer -= frameTime;
            currentFrame++;
            if (currentFrame < frames.Length)
                sr.sprite = frames[currentFrame];
            else if (currentFrame >= frames.Length && looping)
            {
                OnEnable();
            }
            else
            {
                gameObject.SetActive(false); // Animation fertig
            }
        }
    }
}
