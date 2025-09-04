using UnityEngine;
using UnityEngine.UI;   // Button
using TMPro;           // TextMeshProUGUI

namespace MultiplicationGame.View   // <-- mismo namespace que OnlineUI
{
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class SkipOnlineUI : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private TextMeshProUGUI botonSkipText;
        [SerializeField] private OnlineUI onlineUI;  // Panel Online (padre)

        [Header("Texto inicial")]
        [SerializeField] private string textoInicial = ">>\n..."; // visible en inspector

        private Button boton;

        private void Awake()
        {
            boton = GetComponent<Button>();

            if (botonSkipText == null)
                botonSkipText = GetComponentInChildren<TextMeshProUGUI>(true);

            if (onlineUI == null)
                onlineUI = GetComponentInParent<OnlineUI>();

            if (boton != null)
                boton.onClick.AddListener(HandleClick);
            else
                Debug.LogError("[SkipOnlineUI] No se encontró Button en el objeto.");
        }

        private void OnDestroy()
        {
            if (boton != null)
                boton.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            ActualizarTexto();
            if (onlineUI != null)
                onlineUI.OnSkipClicked();
            else
                Debug.LogWarning("[SkipOnlineUI] OnlineUI no asignado.");
        }

        /// Acorta el texto una posición al pulsar (visual).
        public void ActualizarTexto()
        {
            if (botonSkipText != null && !string.IsNullOrEmpty(botonSkipText.text) && botonSkipText.text.Length > 0)
            {
                botonSkipText.text = botonSkipText.text.Substring(0, botonSkipText.text.Length - 1);
            }
        }

        public void SetInteractable(bool activo)
        {
            if (boton != null) boton.interactable = activo;
        }

        /// Restablece el texto y activa el botón (al iniciar nueva partida).
        public void Resetear()
        {
            if (botonSkipText != null) botonSkipText.text = textoInicial;
            if (boton != null) boton.interactable = true;
        }
    }
}
