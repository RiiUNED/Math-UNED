using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI Referencias")]
        public TextMeshProUGUI preguntaText;
        public TMP_InputField inputField;
        public GameObject panelInicio;
        public GameObject panelJuego;
        public GameObject panelResultado;
        public TextMeshProUGUI resultadoFinalText;
        public Button botonEnviar;
        public Button botonIniciar;

        [Header("Progreso de aciertos")]
        public Image[] puntosAcierto;
        public Color colorAcierto = Color.green;

        private GameController controlador;

        private void Start()
        {
            Inicializar();
        }

        public void Inicializar()
        {
            controlador = new GameController();

            controlador.OnPreguntaCambiada += ActualizarPregunta;
            controlador.OnJuegoFinalizado += MostrarResultadoFinal;
            controlador.OnAciertoRegistrado += MarcarAcierto;

            botonEnviar.onClick.AddListener(EnviarRespuesta);
            botonIniciar.onClick.AddListener(IniciarJuego);
        }

        private void IniciarJuego()
        {
            panelInicio.SetActive(false);
            panelJuego.SetActive(true);

            // Iniciar con tabla aleatoria (modo prueba)
            controlador.IniciarJuego(0, tablaAleatoria: true);
        }

        private void EnviarRespuesta()
        {
            if (int.TryParse(inputField.text, out int respuesta))
            {
                controlador.EnviarRespuesta(respuesta);
                inputField.text = "";
                inputField.ActivateInputField();
            }
        }

        public void ActualizarPregunta(string texto)
        {
            preguntaText.text = texto;
        }

        public void MostrarResultadoFinal()
        {
            panelJuego.SetActive(false);
            panelResultado.SetActive(true);
            resultadoFinalText.text = $"¡Fin del juego!\n¡Bien hecho!";
        }

        public void MarcarAcierto(int index)
        {
            if (index >= 0 && index < puntosAcierto.Length)
            {
                puntosAcierto[index].color = colorAcierto;
            }
        }

        public void AgregarDigito(string digito)
        {
            inputField.text += digito;
            inputField.caretPosition = inputField.text.Length;
        }

        public void BorrarUltimoDigito()
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
                inputField.caretPosition = inputField.text.Length;
            }
        }
    }
}
