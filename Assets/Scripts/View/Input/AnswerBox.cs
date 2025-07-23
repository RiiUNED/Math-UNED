using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnswerBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private GameObject placeholder;

    private bool inputEnabled = true;

    private int currentAnswer;
    public int GetCurrentAnswer()
    {
        return currentAnswer;
    }

    public void EnableInput()
    {
        inputEnabled = true;
    }

    public void DisableInput()
    {
        inputEnabled = false;
    }

    public void Clear()
    {
        currentAnswer = 0;
        UpdateText();
    }

    public void RemoveDigit()
    {
        if (!inputEnabled) return;

        if (currentAnswer > 0)
        {
            currentAnswer /= 10;
            UpdateText();
        }
    }

    public void AddDigit(int digit)
    {        
        if (!inputEnabled) return;
        
        currentAnswer = currentAnswer * 10 + digit;
        UpdateText();
    }

    private void UpdateText()
    {
        if (currentAnswer == 0)
        {
            answerText.text = "";
            placeholder.SetActive(true);
        }
        else
        {
            placeholder.SetActive(false);
            answerText.text = currentAnswer.ToString();
        }
    }
}
