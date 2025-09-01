using UnityEngine;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    [DisallowMultipleComponent]
    public class OnlineUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private SkipOnlineUI skipButton;      // referencia al botón (para SetInteractable)

        [Header("Límite local de skips")]
        [SerializeField] private int skipsMaxCliente = 3;      // tope local (3)

        private void OnEnable()
        {
            MostrarDatosSesion(); // tu método de logs/diagnóstico

            // Ajusta el estado inicial del botón según el skips actual
            ActualizarEstadoBotonDesdeModelo();
        }

        /// <summary>
        /// Llamado por SkipOnlineUI en cada click.
        /// Incrementa el contador local de skips y, si llega al límite, desactiva el botón.
        /// No se contacta con el servidor.
        /// </summary>
        public void OnSkipClicked()
        {
            // 1) Lee el estado actual (ya almacenado por tu flujo previo)
            if (!SesionController.TryObtenerDatosJuego(
                out int boardId, out int op1, out int op2,
                out int exNum, out int puntaje, out int skipsActuales, out int rival))
            {
                Debug.LogWarning("[OnlineUI] No hay datos de juego para actualizar skips.");
                return;
            }

            // 2) Calcula el nuevo valor local
            int despuesDeClick = skipsActuales + 1;

            // 3) Actualiza el valor en el modelo (local) a través del Controller
            //    (si no tienes un setter dedicado, puedes reusar RegistrarSesionDesdeJson con un JSON parcial)
            //    Aquí hacemos un update simple: construimos un JSON con el nuevo 'skips'
            var jsonParcial = "{\"skips\":" + Mathf.Min(despuesDeClick, skipsMaxCliente) + "}";
            SesionController.RegistrarSesionDesdeJson(jsonParcial);

            // 4) Si alcanzó el límite con esta pulsación, desactiva el botón
            if (despuesDeClick >= skipsMaxCliente)
            {
                if (skipButton != null) skipButton.SetInteractable(false);
                Debug.Log($"[OnlineUI] Skip #{despuesDeClick}/{skipsMaxCliente}. Límite alcanzado -> botón desactivado.");
            }
            else
            {
                // Si aún no llegaste al límite, se mantiene activo
                if (skipButton != null) skipButton.SetInteractable(true);
                Debug.Log($"[OnlineUI] Skip #{despuesDeClick}/{skipsMaxCliente}. Botón sigue activo.");
            }
        }

        /// <summary>
        /// Ajusta el estado inicial del botón al entrar al panel (por si ya venías con 2 de servidor).
        /// </summary>
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
                // Si no hay datos, por seguridad desactivar hasta que lleguen
                if (skipButton != null) skipButton.SetInteractable(false);
            }
        }

        private void MostrarDatosSesion()
        {
            Debug.Log("=== 📋 Datos de Sesión (via Controller) ===");

            // Credenciales básicas
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

            // Datos de juego (op1, op2, ex_num, puntaje, skips, rival)
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
