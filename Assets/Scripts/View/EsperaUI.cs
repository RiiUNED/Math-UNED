using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    [DisallowMultipleComponent]
    public class Espera : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;

        [Header("Servidor")]
        [SerializeField] private string pollingURL = "";          // Endpoint de polling

        [Header("Timing")]
        [SerializeField] private float intervaloSegundos = 1.5f;  // Intervalo entre polls
        [SerializeField] private int timeoutSegundos = 10;        // Timeout por request
        [SerializeField] private float esperaInicial = 5f;        // ⏳ Tiempo antes del primer polling

        private Coroutine _rutina;

        [System.Serializable]
        private class PollDTO
        {
            public string status;       // "en espera"
            public int session_id;
            public int player_id;
            public int numero_jugador;

            // Cuando hay match, llegan estos:
            public int board_id;
            public int op1, op2, ex_num, puntaje, skips, rival;
        }

        private void OnEnable()
        {
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

        private IEnumerator LoopPolling()
        {
            if (!SesionController.TryObtenerCredencialesPolling(
                    out int sessionId, out int playerId, out int numeroJugador))
            {
                Debug.LogError("No hay sesión registrada. Abre Modo Online primero.");
                yield break;
            }

            string cuerpo = SesionController.ConstruirPayloadPollingJson();
            var wait = new WaitForSecondsRealtime(intervaloSegundos);

            yield return new WaitForSecondsRealtime(esperaInicial);

            while (true)
            {
                using (var req = new UnityWebRequest(pollingURL, "POST"))
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

                        // ✅ Guardamos todo en el modelo
                        SesionController.RegistrarSesionDesdeJson(respuesta);

                        // 🔍 Intentamos mapear para validar el estado
                        PollDTO dto = null;
                        try { dto = JsonUtility.FromJson<PollDTO>(respuesta); } catch { }

                        bool enEspera = dto != null &&
                                        !string.IsNullOrEmpty(dto.status) &&
                                        dto.status.Trim().ToLower() == "en espera";

                        bool hayMatch = (!enEspera && dto != null && dto.board_id != 0);

                        if (hayMatch)
                        {
                            Debug.Log("📦 JSON de inicio de partida (match detectado):\n" + respuesta);

                            // 🔑 Confirmamos que el modelo ya tiene datos completos
                            if (SesionController.TryObtenerDatosJuego(out int boardId, out int op1,
                                                                      out int op2, out int exNum,
                                                                      out int puntaje, out int skips, out int rival))
                            {
                                Debug.Log($"✅ Sesión lista: board {boardId}, rival {rival}, op1={op1}, op2={op2}");
                            }
                            else
                            {
                                Debug.LogWarning("⚠️ Match detectado pero faltan datos en la sesión.");
                            }

                            // 🚀 Transición a panel online
                            uiManager.MostrarPanelOnline();
                            yield break;
                        }
                    }
                    else
                    {
                        Debug.LogError("Error en polling: " + req.error);
                        // Aquí podrías decidir reintentar o notificar al jugador
                    }
                }

                yield return wait;
            }
        }
    }
}
