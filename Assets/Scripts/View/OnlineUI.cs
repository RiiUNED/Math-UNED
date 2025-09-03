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
        [SerializeField] private UIManager uiManager;   // Para mostrar panelResultadoOnline(bool)

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

        [Header("Fin de partida")]
        [SerializeField] private float segundosAntesDeResultado = 2f; // espera antes de mostrar panel Resultado

        private Coroutine _rutina;
        private bool finPartida = false;           // bloquea envíos/polling cuando la partida termina
        private bool ultimoGanadorLocal = false;   // se pasa al UIManager al finalizar

        // DTO serializable para JsonUtility (payload saliente)
        [System.Serializable]
        private class OnlinePollDTO
        {
            public int  session_id;
            public int  player_id;
            public int  numero_jugador;
            public bool skip;
            public int  skips;
            public int  aciertos;  // Alineado con 'puntaje' del modelo
            public int  rival;
            public int  ex_num;
            public int  res;       // respuesta del jugador (AnswerBox) — en polling será 0
        }

        // DTO antiguo (compatibilidad) para detectar fin de partida por 'ganador'
        [System.Serializable]
        private class FinPartidaDTO_Old
        {
            public bool ganador;
            public string mensaje;
            public int puntaje;
        }

        // DTO NUEVO para detectar fin de partida por 'resultado'
        [System.Serializable]
        private class FinPartidaDTO_New
        {
            public string resultado; // "ganaste" | (p.ej. "perdiste")
            public int puntaje;
            public int rival;
            public int session_id;
            public int board_id;
            public int player_id;
            public int numero_jugador;
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
            if (finPartida) return; // bloqueado si ya terminó
            StartCoroutine(EnviarRespuestaUnaVez());
        }

        // Botón "Skip": envía JSON con skip=true y skips+1
        public void OnSkipClicked()
        {
            if (finPartida) return; // bloqueado si ya terminó
            StartCoroutine(EnviarSkipUnaVez());
        }

        // ---------- Modelo / UI Helpers ----------

        private void ActualizarEstadoBotonDesdeModelo()
        {
            if (skipButton == null) return;

            if (SesionController.TryObtenerDatosJuego(
                out int boardId, out int op1, out int op2,
                out int exNum, out int puntaje, out int skipsActuales, out int rival))
            {
                bool puedePulsar = !finPartida && (skipsActuales < skipsMaxCliente);
                skipButton.SetInteractable(puedePulsar);
            }
            else
            {
                skipButton.SetInteractable(false);
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
                if (finPartida) yield break; // no seguimos poll cuando termina

                if (!SesionController.TryObtenerDatosJuego(
                    out int boardId, out int op1, out int op2,
                    out int exNum, out int puntaje, out int skips, out int rival) ||
                    !SesionController.TryObtenerCredencialesPolling(
                        out int sessionId, out int playerId, out int numeroJugador))
                {
                    yield return wait;
                    continue;
                }

                // En polling NUNCA se envía la respuesta del jugador.
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
                    res            = 0 // Valor neutro en polling
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

                        // Evaluar fin de partida (nuevo formato + compat)
                        if (RevisarFinDePartida(respuesta)) yield break; // se inicia transición y se corta polling
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
                skip           = false,
                skips          = skips,
                aciertos       = puntaje,
                rival          = rival,
                ex_num         = exNum,
                res            = respuestaJugador
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
                        inputRespuesta.Clear(); // volver al placeholder tras enviar

                    // Evaluar fin de partida
                    RevisarFinDePartida(respuesta);
                }
                else
                {
                    Debug.LogError("[OnlineUI] Error al enviar: " + req.error);
                }
            }
        }

        // ---------- Enviar una vez al pulsar "Skip" ----------

        private IEnumerator EnviarSkipUnaVez()
        {
            if (!SesionController.TryObtenerDatosJuego(
                out int boardId, out int op1, out int op2,
                out int exNum, out int puntaje, out int skipsActuales, out int rival) ||
                !SesionController.TryObtenerCredencialesPolling(
                    out int sessionId, out int playerId, out int numeroJugador))
            {
                yield break;
            }

            // Aumentamos en 1 el contador de skips (respetando el límite local si aplica)
            int skipsAEnviar = Mathf.Min(skipsActuales + 1, skipsMaxCliente);

            var dto = new OnlinePollDTO
            {
                session_id     = sessionId,
                player_id      = playerId,
                numero_jugador = numeroJugador,
                skip           = true,              // <- requerido
                skips          = skipsAEnviar,      // <- aumentamos en 1
                aciertos       = puntaje,
                rival          = rival,
                ex_num         = exNum,
                res            = 0                  // no se envía respuesta del jugador al hacer skip
            };

            string cuerpo = JsonUtility.ToJson(dto);
            Debug.Log("[OnlineUI] (Skip) JSON que se envía:\n" + cuerpo);

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
                    Debug.Log("📩 (Skip) Respuesta del servidor:\n" + respuesta);

                    // Actualizamos el modelo con lo que devuelva el servidor
                    SesionController.RegistrarSesionDesdeJson(respuesta);

                    // Refrescamos UI y estado del botón Skip
                    AplicarUIDesdeModelo();
                    ActualizarEstadoBotonDesdeModelo();

                    // Evaluar fin de partida
                    RevisarFinDePartida(respuesta);
                }
                else
                {
                    Debug.LogError("[OnlineUI] Error al hacer skip: " + req.error);
                }
            }
        }

        // ---------- Fin de partida ----------

        /// <summary>
        /// Devuelve true si detecta fin de partida (nuevo formato 'resultado' o antiguo 'ganador')
        /// y dispara la transición.
        /// </summary>
        private bool RevisarFinDePartida(string respuestaJson)
        {
            if (string.IsNullOrEmpty(respuestaJson))
                return false;

            bool? ganoLocal = null;

            // 📦 NUEVO FORMATO: {"resultado":"ganaste", "puntaje":10, "rival":0, ... }
            if (respuestaJson.Contains("\"resultado\""))
            {
                try
                {
                    var finNew = JsonUtility.FromJson<FinPartidaDTO_New>(respuestaJson);
                    if (finNew != null && !string.IsNullOrEmpty(finNew.resultado))
                    {
                        var res = finNew.resultado.Trim().ToLowerInvariant();
                        // Consideramos victoria si el servidor manda exactamente "ganaste"
                        ganoLocal = (res == "ganaste");
                        Debug.Log($"[OnlineUI] Fin detectado (nuevo): resultado={finNew.resultado}, puntaje={finNew.puntaje}, rival={finNew.rival}");
                    }
                }
                catch { /* ignorar parse fallido */ }
            }

            // 🧭 COMPATIBILIDAD ANTIGUA: {"ganador":true/false, "mensaje":"...", "puntaje":10}
            if (ganoLocal == null && respuestaJson.Contains("\"ganador\""))
            {
                try
                {
                    var finOld = JsonUtility.FromJson<FinPartidaDTO_Old>(respuestaJson);
                    if (finOld != null)
                    {
                        ganoLocal = finOld.ganador;
                        Debug.Log($"[OnlineUI] Fin detectado (antiguo): ganador={finOld.ganador}, puntaje={finOld.puntaje}");
                    }
                }
                catch { /* ignorar parse fallido */ }
            }

            if (ganoLocal == null) return false;

            // Bloqueamos nuevos envíos/polling
            if (_rutina != null) { StopCoroutine(_rutina); _rutina = null; }
            finPartida = true;
            ultimoGanadorLocal = ganoLocal.Value;
            ActualizarEstadoBotonDesdeModelo(); // desactiva botones

            // Actualizamos barras según el resultado. Requisito: fijar a 10.
            if (ultimoGanadorLocal)
            {
                if (progresoAciertosPropios != null) progresoAciertosPropios.SetProgress(10);
            }
            else
            {
                if (progresoAciertosRival != null) progresoAciertosRival.SetProgress(10);
            }

            // Transición diferida al panel de resultado online
            StartCoroutine(TransicionAPanelResultadoOnline());
            return true;
        }

        private IEnumerator TransicionAPanelResultadoOnline()
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, segundosAntesDeResultado));
            if (uiManager != null)
            {
                uiManager.MostrarPanelResultadoOnline(ultimoGanadorLocal); // pasa bool con ganador local
            }
            else
            {
                Debug.LogWarning("[OnlineUI] uiManager no asignado. No se pudo mostrar panelResultadoOnline.");
            }
        }
    }
}
