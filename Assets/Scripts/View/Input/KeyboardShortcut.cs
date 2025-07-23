using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardShortcut : MonoBehaviour
{
    [SerializeField] private KeyCode keyCode;
    [SerializeField] private Button targetButton;

    void Update()
    {
        if(targetButton.interactable == false) return;
        
        if (Input.GetKeyDown(keyCode))
        {
            targetButton.onClick?.Invoke();
        }
    }
}
