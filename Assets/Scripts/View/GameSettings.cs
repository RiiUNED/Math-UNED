using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplicationGame.View
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private string apiURL;

        public string ApiURL
        {
            get
            {
                apiURL = PlayerPrefs.GetString("ApiURL", apiURL);
                return apiURL;
            }
            set
            {
                apiURL = value;
                PlayerPrefs.SetString("ApiURL", apiURL);
            }
        }
    }
}   