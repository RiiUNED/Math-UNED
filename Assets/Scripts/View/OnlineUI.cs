using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    [DisallowMultipleComponent]
    public class OnlineUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private SkipOnlineUI skipButton;
        [SerializeField] private AnswerBox inputRespuesta;
        [SerializeField] private TextMeshProUGUI preguntaText;

        [Header("Barras de Progreso (se actualizan desde el servidor)")]
        [SerializeField] private TwoColorProgressBar progresoAciertosPropios; // usa 'puntaje'
        [SerializeField] private TwoColorProgressBar progresoAciertosRival;   // usa 'rival'

        [Header("Límite local de skips")]
        [SerializeField] private int skipsMaxCliente = 3;

        [Header("Servidor")]
        [SerializeField] private string pollingURL = "";          // Endpoint (POST)
        [SerializeField] private float intervaloSegundos = 1.5f;  // Intervalo entre polls
        [SerializeField] private int timeoutSegundos = 10;        // Timeout por request
        [SerializeField] private float esperaInicial = 2f;        // Tiempo antes del primer polling

        private Coroutine _rutina;

        // DTO serializable para JsonUtility (payload saliente)
        [System.Serializable]
        private class OnlinePollDTO
        {
            public int  session_id;
            public int  player_id;
            public int  numero_jugador;
            public bool skip;
            public int  skips;
            public int  aciertos;  // lo alineamos con 'puntaje' del modelo
            public int  rival;
            public int  ex_num;
            public int  res;       // respuesta del jugador (AnswerBox) — en polling será 0
        }

        private void OnEnable()
        {
            MostrarDatosSesion();
            ActualizarEstadoBotonDesdeModelo();
            AplicarUIDesdeModelo(); // Inicializa pregunta + barras con lo que ya tenga el modelo

            if (_rutina == null) _rutina = StartCoroutine(LoopPolling());
        }

        private void OnDisable()
        {
            if (_rutina != null)
            {
                StopCoroutine(_rutina);
                _rutina = null;
            }
        }

        // ---------- UI Actions ----------

        // Botón "Enviar": envía JSON con 'res' = valor de AnswerBox (solo aquí)
        public void OnEnviarClicked()
        {
            StartCoroutine(EnviarRespuestaUnaVez());
        }

        public void OnSkipClicked()
        {
            if (!SesionController.TryObtenerDatosJuego(
                out int boardId, out int op1, out int op2,
                out int exNum, out int puntaje, out int skipsActuales, out int rival))
            {
                Debug.LogWarning("[OnlineUI] No hay datos de juego para actualizar skips.");
                return;
            }

            int despuesDeClick = skipsActuales + 1;
            var jsonParcial = "{\"skips\":" + Mathf.Min(despuesDeClick, skipsMaxCliente) + "}";
            SesionController.RegistrarSesionDesdeJson(jsonParcial);

            if (despuesDeClick >= skipsMaxCliente)
            {
                if (skipButton != null) skipButton.SetInteractable(false);
                Debug.Log($"[OnlineUI] Skip #{despuesDeClick}/{skipsMaxCliente}. Límite alcanzado -> botón desactivado.");
            }
            else
            {
                if (skipButton != null) skipButton.SetInteractable(true);
                Debug.Log($"[OnlineUI] Skip #{despuesDeClick}/{skipsMaxCliente}. Botón sigue activo.");
            }
        }

        // ---------- Modelo / UI Helpers ----------

        private void ActualizarEstadoBotonDesdeModelo()
        {
            if (SesionController.TryObtenerDatosJuego(
                out int boardId, out int op1, out int op2,
                out int exNum, out int puntaje, out int skipsActuales, out int rival))
            {
                bool puedePulsar = (skipsActuales < skipsMaxCliente);
                if (skipButton != null) skipButton.SetInteractable(puedePulsar);
            }
            else
            {
                if (skipButton != null) skipButton.SetInteractable(false);
            }
        }

        private void MostrarDatosSesion()
        {
            if (SesionController.TryObtenerDatosJuego(
                out int boardId, out int op1, out int op2,
                out int exNum, out int puntaje, out int skips, out int rival))
            {
                if (preguntaText != null)
                    preguntaText.text = $"¿Cuánto es {op1} x {op2}?";
            }
        }

        private void AplicarUIDesdeModelo()
        {
            if (SesionController.TryObtenerDatosJuego(
                out int boardId, out int op1, out int op2,
                out int exNum, out int puntaje, out int skips, out int rival))
            {
                if (preguntaText != null)
                    preguntaText.text = $"¿Cuánto es {op1} x {op2}?";

                if (progresoAciertosPropios != null)
                    progresoAciertosPropios.SetProgress(Mathf.Max(0, puntaje));

                if (progresoAciertosRival != null)
                    progresoAciertosRival.SetProgress(Mathf.Max(0, rival));
            }
        }

        // ---------- Polling ----------

        private IEnumerator LoopPolling()
        {
            yield return new WaitForSecondsRealtime(esperaInicial);
            var wait = new WaitForSecondsRealtime(intervaloSegundos);

            while (true)
            {
                if (!SesionController.TryObtenerDatosJuego(
                    out int boardId, out int op1, out int op2,
                    out int exNum, out int puntaje, out int skips, out int rival) ||
                    !SesionController.TryObtenerCredencialesPolling(
                        out int sessionId, out int playerId, out int numeroJugador))
                {
                    yield return wait;
                    continue;
                }

                // IMPORTANTE: en polling NO se envía la respuesta del jugador.
                // Forzamos res = 0 (o el valor neutro que espere tu backend).
                var dto = new OnlinePollDTO
                {
                    session_id     = sessionId,
                    player_id      = playerId,
                    numero_jugador = numeroJugador,
                    skip           = false,
                    skips          = skips,
                    aciertos       = puntaje,
                    rival          = rival,
                    ex_num         = exNum,
                    res            = 0 // <-- Nunca leer AnswerBox aquí
                };

                string cuerpo = JsonUtility.ToJson(dto);
                Debug.Log("[OnlineUI] (Polling) JSON que se envía:\n" + cuerpo);

                using (var req = new UnityWebRequest(pollingURL, "POST"))
                {
                    byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(cuerpo);
                    req.uploadHandler   = new UploadHandlerRaw(jsonToSend);
                    req.downloadHandler = new DownloadHandlerBuffer();
                    req.SetRequestHeader("Content-Type", "application/json");
                    req.timeout = timeoutSegundos;

                    yield return req.SendWebRequest();

                    if (req.result == UnityWebRequest.Result.Success)
                    {
                        string respuesta = req.downloadHandler.text;
                        Debug.Log("📩 (Polling) Respuesta del servidor:\n" + respuesta);

                        SesionController.RegistrarSesionDesdeJson(respuesta);
                        AplicarUIDesdeModelo();
                        ActualizarEstadoBotonDesdeModelo();
                    }
                    else
                    {
                        Debug.LogError("[OnlineUI] Error en polling: " + req.error);
                    }
                }

                yield return wait;
            }
        }

        // ---------- Enviar una vez al pulsar "Enviar" ----------

        private IEnumerator EnviarRespuestaUnaVez()
        {
            if (!SesionController.TryObtenerDatosJuego(
                out int boardId, out int op1, out int op2,
                out int exNum, out int puntaje, out int skips, out int rival) ||
                !SesionController.TryObtenerCredencialesPolling(
                    out int sessionId, out int playerId, out int numeroJugador))
            {
                yield break;
            }

            int respuestaJugador = (inputRespuesta != null) ? inputRespuesta.GetCurrentAnswer() : 0;

            var dto = new OnlinePollDTO
            {
                session_id     = sessionId,
                player_id      = playerId,
                numero_jugador = numeroJugador,
                skip           = false,     // si tu flujo requiere lo contrario, ajústalo
                skips          = skips,
                aciertos       = puntaje,
                rival          = rival,
                ex_num         = exNum,
                res            = respuestaJugador // <-- Solo aquí se envía la respuesta del jugador
            };

            string cuerpo = JsonUtility.ToJson(dto);
            Debug.Log("[OnlineUI] (Enviar) JSON que se envía:\n" + cuerpo);

            using (var req = new UnityWebRequest(pollingURL, "POST"))
            {
                byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(cuerpo);
                req.uploadHandler   = new UploadHandlerRaw(jsonToSend);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                req.timeout = timeoutSegundos;

                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    string respuesta = req.downloadHandler.text;
                    Debug.Log("📩 (Enviar) Respuesta del servidor:\n" + respuesta);

                    SesionController.RegistrarSesionDesdeJson(respuesta);
                    AplicarUIDesdeModelo();
                    ActualizarEstadoBotonDesdeModelo();
                    if (inputRespuesta != null)
                        inputRespuesta.Clear();
                }
                else
                {
                    Debug.LogError("[OnlineUI] Error al enviar: " + req.error);
                }
            }
        }
    }
}
