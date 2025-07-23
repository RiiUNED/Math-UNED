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

        private void Start()
        {
            if (turnManager != null)
            {
                turnManager.OnEstadoJugadorActualizado += ActualizarEstadoJugador;
                turnManager.OnJuegoFinalizado += MostrarResultadoFinal;
            }
        }
        public void ConectarControlador(TurnManagerController controlador, PlayerGameController jugador1, PlayerGameController jugador2)
        {
            turnManager = controlador;

            if (panelJugador1 != null)
                panelJugador1.IniciarJuego(jugador1, 1, true);

            if (panelJugador2 != null)
                panelJugador2.IniciarJuego(jugador2, 1, true);

            ConfigurarModoMultijugador(); // <-- Llama aquí

            turnManager.OnEstadoJugadorActualizado += ActualizarEstadoJugador;
            turnManager.OnJuegoFinalizado += MostrarResultadoFinal;
        }
        public void ConfigurarModoMultijugador()
        {
            if (panelJugador1 != null)
                panelJugador1.esMultijugador = true;
            if (panelJugador2 != null)
                panelJugador2.esMultijugador = true;
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
            // Notifica a UIManager para mostrar el panel de resultado multijugador
            if (uiManager != null)
            {
                uiManager.MostrarPanelResultadoMultijugador(ganador);
            }
        }
    }
}
