using UnityEngine;
using System; 

namespace MultiplicationGame.Controller
{
    using MultiplicationGame.Model;

    public static class SesionController
    {
        // Guarda toda la sesión desde el Model
        public static void RegistrarSesion(DatosSesion datos)
        {
            SesionActual.datos = datos;
        }

        // (Se mantiene por compatibilidad con otros módulos que sí referencian Model)
        public static DatosSesion ObtenerSesion()
        {
            return SesionActual.datos;
        }

        // Registra/actualiza sesión a partir de un JSON devuelto por el servidor
        public static void RegistrarSesionDesdeJson(string json)
        {
            // Debug.Log("📦 JSON recibido crudo:\n" + json);
            try
            {
                DatosSesion datos = JsonUtility.FromJson<DatosSesion>(json);
                SesionActual.datos = datos;
            }
            catch
            {
                Debug.LogError("Error al deserializar la sesión.");
            }
        }

        // Solo para depuración
        public static void MostrarDatosSesion()
        {
            var datos = SesionActual.datos;

            if (datos != null)
            {
                var estado = string.IsNullOrEmpty(datos.status) ? "(sin status)" : datos.status;
                Debug.Log($"✅ Sesión iniciada (ID: {datos.session_id}, jugador: {datos.numero_jugador}, estado: {estado})");
            }
            else
            {
                Debug.LogWarning("⚠️ No hay sesión activa para mostrar.");
            }
        }

        public static bool SesionYaIniciada()
        {
            return SesionActual.datos != null;
        }

        // ====== NUEVO: API para la View (sin exponer tipos del Model) ======

        /// <summary>
        /// Entrega los 3 campos necesarios para el polling, sin exponer DatosSesion.
        /// View solo necesita: session_id, player_id, numero_jugador.
        /// </summary>
        public static bool TryObtenerCredencialesPolling(
            out int sessionId, out int playerId, out int numeroJugador)
        {
            var datos = SesionActual.datos;
            if (datos == null)
            {
                sessionId = playerId = numeroJugador = 0;
                return false;
            }

            sessionId = datos.session_id;
            playerId = datos.player_id;
            numeroJugador = datos.numero_jugador;
            return true;
        }

        [Serializable]
        private struct PollingCredsDTO
        {
            public int session_id;
            public int player_id;
            public int numero_jugador;
        }

        /// <summary>
        /// Convenience: devuelve el payload JSON listo para enviar en el POST de polling.
        /// </summary>
        public static string ConstruirPayloadPollingJson()
        {
            if (!TryObtenerCredencialesPolling(out var sid, out var pid, out var num))
                throw new InvalidOperationException("No hay sesión para construir el payload de polling.");

            var dto = new PollingCredsDTO
            {
                session_id = sid,
                player_id = pid,
                numero_jugador = num
            };

            return JsonUtility.ToJson(dto);
        }
    }
}
