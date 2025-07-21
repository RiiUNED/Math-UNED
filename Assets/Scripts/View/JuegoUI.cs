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
        public TMP_InputField inputField;
        public Button botonEnviar;
        public Button botonSkip;
        public TextMeshProUGUI botonSkipText;

        [Header("Progreso de aciertos")]
        public Image[] puntosAcierto;
        public Color colorAcierto = Color.green;
        [SerializeField] private Color colorInicialAcierto = new Color32(255, 0, 0, 255);

        private GameController controlador;

        private void Start()
        {
            botonEnviar.onClick.AddListener(EnviarRespuesta);
            botonSkip.onClick.AddListener(SaltarPregunta);
        }

        public void IniciarJuego(GameController controladorRecibido, int tabla, bool aleatoria)
        {
            controlador = controladorRecibido;

            controlador.OnPreguntaCambiada += ActualizarPregunta;
            controlador.OnJuegoFinalizado += FinalizarJuego;
            controlador.OnAciertoRegistrado += MarcarAcierto;
            controlador.OnSkipsAgotados += DeshabilitarBotonSkip;

            ResetearVista();
            controlador.IniciarJuego(tabla, aleatoria);
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
            gameObject.SetActive(false); // Oculta el panel
            // Aquí puedes notificar a otro script que muestre el panel de resultado
        }

        private void ResetearVista()
        {
            inputField.text = "";
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
    }
}
