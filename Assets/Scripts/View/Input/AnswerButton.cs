using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class AnswerButton : MonoBehaviour
{
    [SerializeField] private int number;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private NumPad numPad; 

    public enum KeyPadType { alphaNumeric, keyPad, none }
    [SerializeField] private KeyPadType keyPadType;

    void Update()
    {
        if(keyPadType == KeyPadType.none) return;
        
        if (keyPadType == KeyPadType.keyPad && Input.GetKeyDown(KeyCode.Keypad0 + number))
        {
            OnButtonPressed();
        }
        else if (keyPadType == KeyPadType.alphaNumeric && Input.GetKeyDown(KeyCode.Alpha0 + number))
        {
            OnButtonPressed();
        }
    }
    void OnValidate()
    {
        buttonText.text = number.ToString();
        gameObject.name = "Button" + number;
#if UNITY_EDITOR
        EditorUtility.SetDirty(buttonText);
        #endif
    }

    public void OnButtonPressed()
    {
        numPad.OnNumberButtonPressed(number);
    }
}
