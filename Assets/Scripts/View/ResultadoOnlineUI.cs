using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MultiplicationGame.View
{
    public class ResultadoOnlineUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resultadoFinalText;
        [SerializeField] private UIManager uiManager;

        [Header("Colores de resultado")]
        [SerializeField] private Color colorVictoria = Color.green;
        [SerializeField] private Color colorDerrota = Color.red;

        private Image backgroundImage;

        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
            if (backgroundImage == null)
            {
                Debug.LogWarning("[ResultadoOnlineUI] No se encontró componente Image en el Panel. El color no se podrá aplicar.");
            }
        }

        public void MostrarResultado(bool ganoJugador)
        {
            if (resultadoFinalText != null)
            {
                resultadoFinalText.text = ganoJugador
                    ? "¡Has ganado!\n¡Bien hecho!"
                    : "¡Has perdido!\n¡Sigue intentándolo!";
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = ganoJugador ? colorVictoria : colorDerrota;
            }
        }

        public void Salir()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Continuar()
        {
            if (uiManager != null)
            {
                uiManager.MostrarPanelSeleccionModo();
            }
        }
    }
}
