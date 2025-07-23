using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    public class JuegoUI : MonoBehaviour
    {
        [Header("Referencias UI")]
        public TextMeshProUGUI preguntaText;
        public AnswerBox answerBox;
        public Button botonEnviar;
        public Button botonSkip;
        public TextMeshProUGUI botonSkipText;
        public bool esMultijugador = false;
        [SerializeField] private UIManager uiManager;


        [Header("Progreso de aciertos")]
        public Image[] puntosAcierto;
        public Color colorAcierto = Color.green;
        [SerializeField] private Color colorInicialAcierto = new Color32(255, 0, 0, 255);

        private PlayerGameController controlador;

        public void IniciarJuego(PlayerGameController controladorRecibido, int tabla, bool aleatoria)
        {
            controlador = controladorRecibido;

            controlador.OnPreguntaCambiada += ActualizarPregunta;
            controlador.OnJuegoFinalizado += FinalizarJuego;
            controlador.OnAciertoRegistrado += MarcarAcierto;
            controlador.OnSkipsAgotados += DeshabilitarBotonSkip;

            ResetearVista();
            controlador.IniciarJuego(tabla, aleatoria);
        }


        public void EnviarRespuesta()
        {
            int respuesta = answerBox.GetCurrentAnswer();

            controlador.EnviarRespuesta(respuesta);
            answerBox.Clear();
            answerBox.EnableInput();
            
        }

        public void SaltarPregunta()
        {
            controlador.RegistrarSkip();
            controlador.SaltarEjercicio();
            ActualizarTextoBotonSkip();
        }

        private void ActualizarPregunta(string texto)
        {
            preguntaText.text = texto;
        }

        private void ActualizarTextoBotonSkip()
        {
            if (botonSkipText != null && !string.IsNullOrEmpty(botonSkipText.text))
            {
                botonSkipText.text = botonSkipText.text.Substring(0, botonSkipText.text.Length - 1);
            }
        }

        private void MarcarAcierto(int index)
        {
            if (index >= 0 && index < puntosAcierto.Length)
            {
                puntosAcierto[index].color = colorAcierto;
            }
        }

        private void DeshabilitarBotonSkip()
        {
            if (botonSkip != null)
            {
                botonSkip.interactable = false;
            }
        }

        private void FinalizarJuego()
        {
            gameObject.SetActive(false);
            if (!esMultijugador && uiManager != null)
            {
                uiManager.MostrarPanelResultado();
            }
        }


        private void ResetearVista()
        {
            answerBox.Clear();
            preguntaText.text = "";

            foreach (var punto in puntosAcierto)
            {
                punto.color = colorInicialAcierto;
            }

            if (botonSkipText != null)
                botonSkipText.text = ">>\n...";

            if (botonSkip != null)
                botonSkip.interactable = true;
        }
        private void ActualizarProgreso(int cantidad)
        {
            for (int i = 0; i < puntosAcierto.Length; i++)
            {
                puntosAcierto[i].color = (i < cantidad) ? colorAcierto : colorInicialAcierto;
            }
        }

        private void ActualizarBotonSkip(bool activo)
        {
            if (botonSkip != null)
                botonSkip.interactable = activo;
        }

        public void Actualizar(string pregunta, int aciertos, bool puedeSaltar)
        {
            ActualizarPregunta(pregunta);
            ActualizarProgreso(aciertos);
            ActualizarBotonSkip(puedeSaltar);
        }

    }
}
