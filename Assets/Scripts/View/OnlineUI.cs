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
        [SerializeField] private GameSettings gameSettings;
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
            public int session_id;
            public int player_id;
            public int numero_jugador;
            public bool skip;
            public int skips;
            public int aciertos;  // Alineado con 'puntaje' del modelo
            public int rival;
            public int ex_num;
            public int res;       // respuesta del jugador (AnswerBox) — en polling será 0
        }

        [System.Serializable]
        private class FinPartidaDTO_Old
        {
            public bool ganador;
            public string mensaje;
            public int puntaje;
        }

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
            PrepararNuevaPartida();
        }

        private void OnDisable()
        {
            if (_rutina != null)
            {
                StopCoroutine(_rutina);
                _rutina = null;
            }
        }

        // ---------- Preparación de nueva partida ----------

        public void PrepararNuevaPartida()
        {
            finPartida = false;
            ultimoGanadorLocal = false;

            inputRespuesta?.Clear();
            // ❌ No vaciar pregunta manualmente
            progresoAciertosPropios?.SetProgress(0);
            progresoAciertosRival?.SetProgress(0);

            SesionController.ReiniciarEstadoDeJuegoParaNuevaPartida();

            // ✅ Si ya hay op1/op2, esto pinta la pregunta de inmediato
            AplicarUIDesdeModelo();
            skipButton?.Resetear();

            // Skip habilitado con skips=0 por defecto (ya lo tienes en el método)
            ActualizarEstadoBotonDesdeModelo();

            if (_rutina != null) { StopCoroutine(_rutina); _rutina = null; }
            _rutina = StartCoroutine(LoopPolling());
        }

        // ---------- UI Actions ----------

        public void OnEnviarClicked()
        {
            if (finPartida) return;
            StartCoroutine(EnviarRespuestaUnaVez());
        }

        public void OnSkipClicked()
        {
            if (finPartida) return;
            StartCoroutine(EnviarSkipUnaVez());
        }

        // ---------- Helpers UI/Modelo ----------

        // ⬇️ MODIFICADO: si aún no hay datos de juego, asumimos skipsActuales = 0 (inicio de partida)
        private void ActualizarEstadoBotonDesdeModelo()
        {
            if (skipButton == null) return;

            int skipsActuales = 0; // al iniciar una partida siempre es 0

            if (SesionController.TryObtenerDatosJuego(
                out int boardId, out int op1, out int op2,
                out int exNum, out int puntaje, out int skips, out int rival))
            {
                skipsActuales = skips;
            }

            bool puedePulsar = !finPartida && (skipsActuales < skipsMaxCliente);
            skipButton.SetInteractable(puedePulsar);
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
                if (finPartida) yield break;

                if (!SesionController.TryObtenerDatosJuego(
                    out int boardId, out int op1, out int op2,
                    out int exNum, out int puntaje, out int skips, out int rival) ||
                    !SesionController.TryObtenerCredencialesPolling(
                        out int sessionId, out int playerId, out int numeroJugador))
                {
                    yield return wait;
                    continue;
                }

                var dto = new OnlinePollDTO
                {
                    session_id = sessionId,
                    player_id = playerId,
                    numero_jugador = numeroJugador,
                    skip = false,
                    skips = skips,
                    aciertos = puntaje,
                    rival = rival,
                    ex_num = exNum,
                    res = 0
                };

                string cuerpo = JsonUtility.ToJson(dto);
                Debug.Log("[OnlineUI] (Polling) JSON que se envía:\n" + cuerpo);

                using (var req = new UnityWebRequest(gameSettings.ApiURL, "POST"))
                {
                    byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(cuerpo);
                    req.uploadHandler = new UploadHandlerRaw(jsonToSend);
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

                        if (RevisarFinDePartida(respuesta)) yield break;
                    }
                    else
                    {
                        Debug.LogError("[OnlineUI] Error en polling: " + req.error);
                    }
                }

                yield return wait;
            }
        }

        // ---------- Enviar ----------

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
                session_id = sessionId,
                player_id = playerId,
                numero_jugador = numeroJugador,
                skip = false,
                skips = skips,
                aciertos = puntaje,
                rival = rival,
                ex_num = exNum,
                res = respuestaJugador
            };

            string cuerpo = JsonUtility.ToJson(dto);
            Debug.Log("[OnlineUI] (Enviar) JSON que se envía:\n" + cuerpo);

            using (var req = new UnityWebRequest(gameSettings.ApiURL, "POST"))
            {
                byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(cuerpo);
                req.uploadHandler = new UploadHandlerRaw(jsonToSend);
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

                    RevisarFinDePartida(respuesta);
                }
                else
                {
                    Debug.LogError("[OnlineUI] Error al enviar: " + req.error);
                }
            }
        }

        // ---------- Skip ----------

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

            int skipsAEnviar = Mathf.Min(skipsActuales + 1, skipsMaxCliente);

            var dto = new OnlinePollDTO
            {
                session_id = sessionId,
                player_id = playerId,
                numero_jugador = numeroJugador,
                skip = true,
                skips = skipsAEnviar,
                aciertos = puntaje,
                rival = rival,
                ex_num = exNum,
                res = 0
            };

            string cuerpo = JsonUtility.ToJson(dto);
            Debug.Log("[OnlineUI] (Skip) JSON que se envía:\n" + cuerpo);

            using (var req = new UnityWebRequest(gameSettings.ApiURL, "POST"))
            {
                byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(cuerpo);
                req.uploadHandler = new UploadHandlerRaw(jsonToSend);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                req.timeout = timeoutSegundos;

                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    string respuesta = req.downloadHandler.text;
                    Debug.Log("📩 (Skip) Respuesta del servidor:\n" + respuesta);

                    SesionController.RegistrarSesionDesdeJson(respuesta);
                    AplicarUIDesdeModelo();
                    ActualizarEstadoBotonDesdeModelo();

                    RevisarFinDePartida(respuesta);
                }
                else
                {
                    Debug.LogError("[OnlineUI] Error al hacer skip: " + req.error);
                }
            }
        }

        // ---------- Fin de partida ----------

        private bool RevisarFinDePartida(string respuestaJson)
        {
            if (string.IsNullOrEmpty(respuestaJson))
                return false;

            bool? ganoLocal = null;

            if (respuestaJson.Contains("\"resultado\""))
            {
                try
                {
                    var finNew = JsonUtility.FromJson<FinPartidaDTO_New>(respuestaJson);
                    if (finNew != null && !string.IsNullOrEmpty(finNew.resultado))
                    {
                        var res = finNew.resultado.Trim().ToLowerInvariant();
                        ganoLocal = (res == "ganaste");
                        Debug.Log($"[OnlineUI] Fin detectado (nuevo): resultado={finNew.resultado}, puntaje={finNew.puntaje}, rival={finNew.rival}");
                    }
                }
                catch { }
            }

            if (ganoLocal == null && respuestaJson.Contains("\"ganador\""))
            {
                try
                {
                    var finOld = JsonUtility.FromJson<FinPartidaDTO_Old>(respuestaJson);
                    if (finOld != null) ganoLocal = finOld.ganador;
                }
                catch { }
            }

            if (ganoLocal == null) return false;

            if (_rutina != null) { StopCoroutine(_rutina); _rutina = null; }
            finPartida = true;
            ultimoGanadorLocal = ganoLocal.Value;
            ActualizarEstadoBotonDesdeModelo();

            if (ultimoGanadorLocal)
                progresoAciertosPropios?.SetProgress(10);
            else
                progresoAciertosRival?.SetProgress(10);

            StartCoroutine(TransicionAPanelResultadoOnline());
            return true;
        }

        private IEnumerator TransicionAPanelResultadoOnline()
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, segundosAntesDeResultado));
            if (uiManager != null)
                uiManager.MostrarPanelResultadoOnline(ultimoGanadorLocal);
        }
    }
}
