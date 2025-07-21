using UnityEngine;
using TMPro;

namespace MultiplicationGame.View
{
    public class ResultadoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resultadoFinalText;
        [SerializeField] private UIManager uiManager;

        public void MostrarResultado()
        {
            if (resultadoFinalText != null)
            {
                resultadoFinalText.text = "¡Has completado el juego!\n¡Bien hecho!";
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
