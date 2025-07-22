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
        public Button botonSkip;
        public TextMeshProUGUI botonSkipText;

        [Header("Progreso de aciertos")]
        public Image[] puntosAcierto;
        public Color colorAcierto = Color.green;
        [SerializeField] private Color colorInicialAcierto = new Color32(255, 0, 0, 255); // #FF0000


        private PlayerGameController controlador;

        private void Start()
        {
            Inicializar();
        }

        public void Inicializar()
        {
            controlador = new PlayerGameController();

            controlador.OnPreguntaCambiada += ActualizarPregunta;
            controlador.OnJuegoFinalizado += MostrarResultadoFinal;
            controlador.OnAciertoRegistrado += MarcarAcierto;
            controlador.OnSkipsAgotados += DeshabilitarBotonSkip;

            // Limpiar cualquier listener anterior para evitar duplicados
            botonEnviar.onClick.RemoveAllListeners();
            botonSkip.onClick.RemoveAllListeners();

            // Registrar eventos solo una vez
            botonEnviar.onClick.AddListener(EnviarRespuesta);
            botonSkip.onClick.AddListener(SaltarPregunta);
        }

        public void SeleccionarTabla(int tabla)
        {
            ResetearVista(); // ← limpia la vista antes de empezar

            bool usarAleatoria = (tabla == 0);
            int tablaSeleccionada = usarAleatoria ? 1 : tabla;

            controlador.IniciarJuego(tablaSeleccionada, usarAleatoria);

            panelInicio.SetActive(false);
            panelJuego.SetActive(true);
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

        public void ActualizarPregunta(string texto)
        {
            Debug.Log("Recibida nueva pregunta: " + texto);
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

        public void ActualizarTextoBotonSkip()
        {
            if (botonSkipText != null && !string.IsNullOrEmpty(botonSkipText.text))
            {
                botonSkipText.text = botonSkipText.text.Substring(0, botonSkipText.text.Length - 1);
            }
        }

        public void DeshabilitarBotonSkip()
        {
            if (botonSkip != null)
            {
                botonSkip.interactable = false;
            }
        }

        public void SalirDelJuego()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Detener en editor
#else
            Application.Quit(); // Salir en compilado
#endif
        }

        public void VolverAlMenuInicio()
        {
            panelResultado.SetActive(false);
            panelInicio.SetActive(true);
            ResetearVista(); // ← reinicia la vista
        }

        private void ResetearVista()
        {
            // Limpiar input
            inputField.text = "";

            // Restaurar aciertos a color inicial
            foreach (var punto in puntosAcierto)
            {
                punto.color = colorInicialAcierto;
            }

            // Restaurar texto del botón Skip
            if (botonSkipText != null)
            {
                botonSkipText.text = ">>\n...";
            }

            // Habilitar botón Skip
            if (botonSkip != null)
            {
                botonSkip.interactable = true;
            }

            // Limpiar pregunta visible
            preguntaText.text = "";
        }
    }
}
