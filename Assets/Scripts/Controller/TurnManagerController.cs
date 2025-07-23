using System;
using UnityEngine; // Necesario para usar Debug.Log
using MultiplicationGame.Controller;

namespace MultiplicationGame.Controller
{
    public class TurnManagerController
    {
        private PlayerGameController controller1;
        private PlayerGameController controller2;

        public event Action<int, string, int, bool> OnEstadoJugadorActualizado;
        public event Action<int> OnJuegoFinalizado; // int = índice del ganador (1 o 2)

        private bool juegoFinalizado = false;
        private int ganador = 0;

        /// <summary>
        /// Inicializa el juego para dos jugadores en modo aleatorio.
        /// Devuelve las instancias de cada controlador de jugador.
        /// </summary>
        public (PlayerGameController, PlayerGameController) IniciarJuegoMultijugador()
        {
            controller1 = new PlayerGameController();
            controller2 = new PlayerGameController();

            Debug.Log($"[TurnManager] Controlador J1: {controller1.GetHashCode()}");
            Debug.Log($"[TurnManager] Controlador J2: {controller2.GetHashCode()}");

            controller1.IniciarJuego(tabla: 1, tablaAleatoria: true);
            controller2.IniciarJuego(tabla: 1, tablaAleatoria: true);

            EmitirEstadoJugador(1, controller1);
            EmitirEstadoJugador(2, controller2);

            return (controller1, controller2);
        }

        public void ProcesarRespuestaJugador(int jugadorIndex, int respuesta)
        {
            var controlador = ObtenerControlador(jugadorIndex);

            Debug.Log($"[TurnManager] Jugador {jugadorIndex} responde: {respuesta} → usando controlador ID: {controlador.GetHashCode()}");

            controlador.EnviarRespuesta(respuesta);
            EmitirEstadoJugador(jugadorIndex, controlador);
            VerificarFinDeJuego();
        }

        public void ProcesarSkipJugador(int jugadorIndex)
        {
            var controlador = ObtenerControlador(jugadorIndex);

            Debug.Log($"[TurnManager] Jugador {jugadorIndex} SALTA → usando controlador ID: {controlador.GetHashCode()}");

            controlador.RegistrarSkip();
            controlador.SaltarEjercicio();

            EmitirEstadoJugador(jugadorIndex, controlador);
            VerificarFinDeJuego();
        }

        private void EmitirEstadoJugador(int jugadorIndex, PlayerGameController ctrl)
        {
            Debug.Log($"[TurnManager] Emitiendo estado J{jugadorIndex}: pregunta='{ctrl.ObtenerPreguntaActual()}', aciertos={ctrl.ObtenerAciertos()}, puedeSaltar={ctrl.PuedeSaltar()}");

            OnEstadoJugadorActualizado?.Invoke(
                jugadorIndex,
                ctrl.ObtenerPreguntaActual(),
                ctrl.ObtenerAciertos(),
                ctrl.PuedeSaltar()
            );
        }

        private void VerificarFinDeJuego()
        {
            if (juegoFinalizado)
                return;

            if (controller1.ObtenerAciertos() >= 10)
            {
                ganador = 1;
                juegoFinalizado = true;
                OnJuegoFinalizado?.Invoke(ganador);
            }
            else if (controller2.ObtenerAciertos() >= 10)
            {
                ganador = 2;
                juegoFinalizado = true;
                OnJuegoFinalizado?.Invoke(ganador);
            }
        }

        private PlayerGameController ObtenerControlador(int index)
        {
            return (index == 1) ? controller1 : controller2;
        }
    }
}
