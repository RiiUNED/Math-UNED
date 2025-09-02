using UnityEngine;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    [DisallowMultipleComponent]
    public class OnlineUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private SkipOnlineUI skipButton;

        // NUEVO: referencia al AnswerBox (InputRespuesta)
        [SerializeField] private AnswerBox inputRespuesta;

        [Header("Límite local de skips")]
        [SerializeField] private int skipsMaxCliente = 3;

        private void OnEnable()
        {
            MostrarDatosSesion();
            ActualizarEstadoBotonDesdeModelo();
        }

        // NUEVO: botón "Enviar"
        public void OnEnviarClicked()
        {
            int valor = 0;
            if (inputRespuesta != null)
            {
                // Devuelve 0 si está vacío/placeholder, o el número introducido si no
                valor = inputRespuesta.GetCurrentAnswer();
            }
            Debug.Log($"[OnlineUI] Enviar -> {valor}");
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
            Debug.Log("=== 📋 Datos de Sesión (via Controller) ===");

            if (SesionController.TryObtenerCredencialesPolling(
                out int sessionId, out int playerId, out int numeroJugador))
            {
                Debug.Log($"Session ID: {sessionId}");
                Debug.Log($"Player ID: {playerId}");
                Debug.Log($"Número de jugador: {numeroJugador}");
            }
            else
            {
                Debug.LogWarning("⚠️ No hay sesión activa.");
                return;
            }

            if (SesionController.TryObtenerDatosJuego(
                out int boardId, out int op1, out int op2,
                out int exNum, out int puntaje, out int skips, out int rival))
            {
                Debug.Log("=== 🎮 Datos de Juego ===");
                Debug.Log($"Board ID: {boardId}");
                Debug.Log($"Operando 1: {op1}");
                Debug.Log($"Operando 2: {op2}");
                Debug.Log($"Ejercicio #: {exNum}");
                Debug.Log($"Puntaje: {puntaje}");
                Debug.Log($"Skips (actual): {skips}");
                Debug.Log($"Rival: {rival}");
            }
            else
            {
                Debug.Log("ℹ️ Aún no hay datos de juego disponibles (probablemente la sesión está en espera).");
            }
        }
    }
}
