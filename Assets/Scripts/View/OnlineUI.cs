using UnityEngine;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    [DisallowMultipleComponent]
    public class PanelOnline : MonoBehaviour
    {
        private void OnEnable()
        {
            MostrarDatosSesion();
        }

        private void MostrarDatosSesion()
        {
            Debug.Log("=== 📋 Datos de Sesión (via Controller) ===");

            // 🔹 Datos básicos (para polling y estado)
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

            // 🔹 Intentamos obtener los datos de juego
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
                Debug.Log($"Skips: {skips}");
                Debug.Log($"Rival: {rival}");
            }
            else
            {
                Debug.Log("ℹ️ Aún no hay datos de juego disponibles (probablemente la sesión está en espera).");
            }
        }
    }
}
