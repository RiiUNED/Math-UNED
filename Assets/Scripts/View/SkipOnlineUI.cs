using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MultiplicationGame.View
{
    /// <summary>
    /// Script del botón Skip en el modo online.
    /// - Actualiza su texto en cada pulsación.
    /// - Notifica SIEMPRE a OnlineUI (padre) para que éste incremente skips y, si corresponde, desactive el botón.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class SkipOnlineUI : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private TextMeshProUGUI botonSkipText;
        [SerializeField] private OnlineUI onlineUI;  // Panel Online (padre)

        private Button boton;

        private void Awake()
        {
            boton = GetComponent<Button>();

            if (botonSkipText == null)
                botonSkipText = GetComponentInChildren<TextMeshProUGUI>(true);
            if (onlineUI == null)
                onlineUI = GetComponentInParent<OnlineUI>();

            boton.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            if (boton != null) boton.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            // 1) Actualiza el texto (como tu lógica actual)
            ActualizarTexto();

            // 2) Notifica SIEMPRE al panel online (obligatorio)
            if (onlineUI != null)
            {
                onlineUI.OnSkipClicked();
            }
            else
            {
                Debug.LogError("[SkipOnlineUI] No se encontró OnlineUI en los padres.");
            }
        }

        /// Quita el último carácter del texto (>>\n... -> acorta en cada click).
        public void ActualizarTexto()
        {
            if (botonSkipText != null && !string.IsNullOrEmpty(botonSkipText.text))
            {
                botonSkipText.text = botonSkipText.text.Substring(0, botonSkipText.text.Length - 1);
            }
        }

        /// Habilita/inhabilita el botón (la decisión la toma OnlineUI).
        public void SetInteractable(bool activo)
        {
            if (boton != null) boton.interactable = activo;
        }

        /// Restablece el texto y activa el botón (si reinicias ronda/partida).
        public void Resetear()
        {
            if (botonSkipText != null) botonSkipText.text = ">>\n...";
            if (boton != null) boton.interactable = true;
        }
    }
}
