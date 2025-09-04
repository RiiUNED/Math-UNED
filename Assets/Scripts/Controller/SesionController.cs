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
            try
            {
                DatosSesion datos = JsonUtility.FromJson<DatosSesion>(json);

                // 🔄 Si ya había sesión, actualizamos en lugar de reemplazar todo
                if (SesionActual.datos != null)
                {
                    var actual = SesionActual.datos;

                    // Actualizamos campo por campo (solo si el nuevo trae info válida)
                    actual.session_id = datos.session_id != 0 ? datos.session_id : actual.session_id;
                    actual.player_id = datos.player_id != 0 ? datos.player_id : actual.player_id;
                    actual.numero_jugador = datos.numero_jugador != 0 ? datos.numero_jugador : actual.numero_jugador;

                    if (!string.IsNullOrEmpty(datos.status)) actual.status = datos.status;
                    if (!string.IsNullOrEmpty(datos.message)) actual.message = datos.message;

                    if (datos.board_id != 0) actual.board_id = datos.board_id;
                    if (datos.op1 != 0) actual.op1 = datos.op1;
                    if (datos.op2 != 0) actual.op2 = datos.op2;
                    if (datos.ex_num != 0) actual.ex_num = datos.ex_num;

                    actual.puntaje = datos.puntaje; // puede ser 0 legítimo
                    actual.skips = datos.skips;
                    actual.rival = datos.rival;
                }
                else
                {
                    SesionActual.datos = datos;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error al deserializar la sesión: " + ex.Message);
            }
        }

        // Solo para depuración
        public static void MostrarDatosSesion()
        {
            var datos = SesionActual.datos;

            if (datos != null)
            {
                string estado = string.IsNullOrEmpty(datos.status) ? "(sin status)" : datos.status;
                Debug.Log(
                    $"✅ Sesión (ID: {datos.session_id}, jugador: {datos.numero_jugador}, estado: {estado}, board: {datos.board_id})"
                );
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

        // ====== NUEVO: API para la View ======

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

        /// <summary>
        /// Devuelve el payload JSON listo para enviar en el POST de polling.
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

        /// <summary>
        /// Helper para que la View obtenga el último estado "listo para jugar".
        /// </summary>
        public static bool TryObtenerDatosJuego(out int boardId, out int op1, out int op2,
                                                out int exNum, out int puntaje, out int skips, out int rival)
        {
            var datos = SesionActual.datos;
            if (datos == null || datos.board_id == 0)
            {
                boardId = op1 = op2 = exNum = puntaje = skips = rival = 0;
                return false;
            }

            boardId = datos.board_id;
            op1 = datos.op1;
            op2 = datos.op2;
            exNum = datos.ex_num;
            puntaje = datos.puntaje;
            skips = datos.skips;
            rival = datos.rival;
            return true;
        }

        [Serializable]
        private struct PollingCredsDTO
        {
            public int session_id;
            public int player_id;
            public int numero_jugador;
        }

        // Reset
        public static void ReiniciarEstadoDeJuegoParaNuevaPartida()
        {
            var d = SesionActual.datos;
            if (d == null) return;

            // 🔸 NO tocar: d.board_id, d.op1, d.op2, d.ex_num  (los trae el inicio de partida)
            d.puntaje = 0;
            d.skips = 0;
            d.rival = 0;
            d.status = null;
            d.message = null;
        }

    }
}
