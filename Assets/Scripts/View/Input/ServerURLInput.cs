using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplicationGame.View
{
    public class ServerURLInput : MonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private TMP_InputField inputField;

        void Start()
        {
            inputField.text = gameSettings.ApiURL;
        }

        public void OnInputFieldChanged()
        {
            gameSettings.ApiURL = inputField.text;
        }
    }
}
