using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumPad : MonoBehaviour
{
    [SerializeField] private AnswerBox answerBox;

    public void OnNumberButtonPressed(int number)
    {
        answerBox.AddDigit(number);
    }

    public void OnClearButtonPressed()
    {
        answerBox.RemoveDigit();
    }
}
