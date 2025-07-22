using System;

namespace MultiplicationGame.Controller
{
    public class TurnManagerController
    {
        private PlayerGameController controller1;
        private PlayerGameController controller2;

        public event Action<int, string, int, bool> OnEstadoJugadorActualizado;
        public event Action OnJuegoFinalizado;

        private bool juegoFinalizado = false;

        /// <summary>
        /// Inicializa el juego para dos jugadores en modo aleatorio.
        /// Devuelve las instancias de cada controlador de jugador.
        /// </summary>
        public (PlayerGameController, PlayerGameController) IniciarJuegoMultijugador()
        {
            controller1 = new PlayerGameController();
            controller2 = new PlayerGameController();

            controller1.IniciarJuego(tabla: 1, tablaAleatoria: true);
            controller2.IniciarJuego(tabla: 1, tablaAleatoria: true);

            EmitirEstadoJugador(1, controller1);
            EmitirEstadoJugador(2, controller2);

            return (controller1, controller2);
        }

        public void ProcesarRespuestaJugador(int jugadorIndex, int respuesta)
        {
            var controlador = ObtenerControlador(jugadorIndex);
            controlador.EnviarRespuesta(respuesta);

            EmitirEstadoJugador(jugadorIndex, controlador);
            VerificarFinDeJuego();
        }

        public void ProcesarSkipJugador(int jugadorIndex)
        {
            var controlador = ObtenerControlador(jugadorIndex);
            controlador.RegistrarSkip();
            controlador.SaltarEjercicio();

            EmitirEstadoJugador(jugadorIndex, controlador);
            VerificarFinDeJuego();
        }

        private void EmitirEstadoJugador(int jugadorIndex, PlayerGameController ctrl)
        {
            OnEstadoJugadorActualizado?.Invoke(
                jugadorIndex,
                ctrl.ObtenerPreguntaActual(),
                ctrl.ObtenerAciertos(),
                ctrl.PuedeSaltar()
            );
        }

        private void VerificarFinDeJuego()
        {
            if (!juegoFinalizado &&
                controller1.ObtenerPreguntaActual() == "" &&
                controller2.ObtenerPreguntaActual() == "")
            {
                juegoFinalizado = true;
                OnJuegoFinalizado?.Invoke();
            }
        }

        private PlayerGameController ObtenerControlador(int index)
        {
            return (index == 1) ? controller1 : controller2;
        }
    }
}
