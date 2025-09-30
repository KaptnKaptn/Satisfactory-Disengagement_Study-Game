using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialTextTrigger : MonoBehaviour
{
    public string text;

    public GameObject tutorialBubble;
    private TutorialText tutorialText;

    // Start is called before the first frame update
    void Start()
    {
        tutorialText = tutorialBubble.GetComponent<TutorialText>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            tutorialText.text = text;
            tutorialBubble.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }
}
