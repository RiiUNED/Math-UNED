using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MultiplicationGame.View
{
    public class ResultadoMultijugadorUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resultadoFinalText;
        [SerializeField] private UIManager uiManager;
        [SerializeField] public Color colorGanador = Color.green;
        [SerializeField] public Color colorPerdedor = Color.red;
        [SerializeField] public Image panelJugador1;
        [SerializeField] public Image panelJugador2;

        public void MostrarResultado(int ganador)
        {

            if (ganador == 1)
            {
                resultadoFinalText.text = "¡Jugador 1 ganó!";
                panelJugador1.color = colorGanador;
                panelJugador2.color = colorPerdedor;
            }
            else if (ganador == 2)
            {
                resultadoFinalText.text = "¡Jugador 2 ganó!";
                panelJugador1.color = colorPerdedor;
                panelJugador2.color = colorGanador;
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
