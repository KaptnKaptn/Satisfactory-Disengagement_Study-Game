using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    public TextMeshProUGUI bubbleText;
    public string text;
    public float textSpeed;
    public float fullTextTime;

    void OnEnable()
    {
        bubbleText.text = string.Empty;

        StartCoroutine(TypeLine(text));
    }

    private IEnumerator TypeLine(string line)
    {
        foreach (char c in line.ToCharArray())
        {
            bubbleText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        yield return new WaitForSeconds(fullTextTime);
        gameObject.SetActive(false);
    }
}
