using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    public class PanelJuegoMultijugador : MonoBehaviour
    {
        [Header("Referencias a Paneles de Jugadores")]
        public JuegoUI panelJugador1;
        public JuegoUI panelJugador2;

        [Header("Controlador de Turnos")]
        public TurnManagerController turnManager;

        [Header("Referencias UI")]
        public UIManager uiManager;

        public void ConectarControlador(TurnManagerController controlador, PlayerGameController jugador1, PlayerGameController jugador2)
        {
            turnManager = controlador;

            if (panelJugador1 != null)
            {
                panelJugador1.turnManager = controlador;
                panelJugador1.esMultijugador = true;
                panelJugador1.IniciarJuego(jugador1, 1, true); // jugador 1
            }

            if (panelJugador2 != null)
            {
                panelJugador2.turnManager = controlador;
                panelJugador2.esMultijugador = true;
                panelJugador2.IniciarJuego(jugador2, 2, true); // jugador 2 ← corregido aquí
            }

            // Suscribe eventos (previene duplicados)
            turnManager.OnEstadoJugadorActualizado -= ActualizarEstadoJugador;
            turnManager.OnEstadoJugadorActualizado += ActualizarEstadoJugador;

            turnManager.OnJuegoFinalizado -= MostrarResultadoFinal;
            turnManager.OnJuegoFinalizado += MostrarResultadoFinal;
        }

        private void ActualizarEstadoJugador(int jugador, string pregunta, int aciertos, bool puedeSaltar)
        {
            if (jugador == 1)
            {
                panelJugador1.Actualizar(pregunta, aciertos, puedeSaltar);
            }
            else if (jugador == 2)
            {
                panelJugador2.Actualizar(pregunta, aciertos, puedeSaltar);
            }
        }

        private void MostrarResultadoFinal(int ganador)
        {
            if (panelJugador1 != null) panelJugador1.gameObject.SetActive(false);
            if (panelJugador2 != null) panelJugador2.gameObject.SetActive(false);

            if (uiManager != null)
            {
                uiManager.MostrarPanelResultadoMultijugador(ganador);
            }
        }
    }
}
