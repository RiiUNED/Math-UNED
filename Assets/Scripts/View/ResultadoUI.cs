using UnityEngine;
using TMPro;

namespace MultiplicationGame.View
{
    public class ResultadoUI : MonoBehaviour
    {
        public GameObject panelResultado;
        public GameObject panelInicio;
        public TextMeshProUGUI resultadoFinalText;

        public void MostrarResultado()
        {
            panelResultado.SetActive(true);
            panelInicio.SetActive(false);
            resultadoFinalText.text = $"¡Fin del juego!\n¡Bien hecho!";
        }

        public void VolverAlMenu()
        {
            panelResultado.SetActive(false);
            panelInicio.SetActive(true);
        }
    }
}
