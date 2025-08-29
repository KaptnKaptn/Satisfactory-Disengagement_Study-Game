using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FailSafe_SpeechBubble : MonoBehaviour
{
    public TextMeshProUGUI bubbleText;
    public List<string> possibleLines;
    public float textSpeed;
    public float fullTextTime;

    void OnEnable()
    {
        bubbleText.text = string.Empty;

        int randomIndex = Random.Range(0, possibleLines.Count);
        string randomLine = possibleLines[randomIndex];

        StartCoroutine(TypeLine(randomLine));
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
