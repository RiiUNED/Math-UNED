using UnityEngine;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    public class UIManager : MonoBehaviour
    {
        [Header("Paneles del Juego")]
        [SerializeField] private GameObject panelSeleccionJugadores;
        [SerializeField] private GameObject panel1Jugador;
        [SerializeField] private GameObject panelJuego;
        [SerializeField] private GameObject panelMultijugador;
        [SerializeField] private GameObject panelResultado;
        [SerializeField] private GameObject panelResultadoMultijugador;
        [SerializeField] private GameObject panelSeleccionModo;
        [SerializeField] private GameObject panelEspera;

        private PlayerGameController controlador;
        private TurnManagerController controladorMultijugador;

        private void Awake()
        {
            if (!panelSeleccionJugadores || !panel1Jugador || !panelJuego || !panelMultijugador || !panelResultado || !panelResultadoMultijugador || !panelSeleccionModo || !panelEspera)
            {
                Debug.LogWarning("Algunos paneles no se encontraron en el Canvas. Revisa nombres y jerarquía.");
            }

            MostrarPanelSeleccionModo();
        }

        public void IniciarJuegoConTabla(int tabla)
        {
            controlador = new PlayerGameController();

            bool aleatoria = (tabla == 0);
            int tablaFinal = aleatoria ? 1 : tabla;

            panelJuego.GetComponent<JuegoUI>().IniciarJuego(controlador, tablaFinal, aleatoria);

            MostrarPanelJuego();
        }

        public void IniciarModoMultijugadorLocal()
        {
            controladorMultijugador = new TurnManagerController();

            var (jugador1, jugador2) = controladorMultijugador.IniciarJuegoMultijugador();

            var panelUI = panelMultijugador.GetComponent<PanelJuegoMultijugador>();
            if (panelUI != null)
            {
                panelUI.ConectarControlador(controladorMultijugador, jugador1, jugador2);
            }

            MostrarPanelMultijugador();
        }

        public void IniciarSeleccionModo()
        {
            OcultarTodosLosPaneles();
            if (panelSeleccionModo != null) panelSeleccionModo.SetActive(true);
        }

        public void MostrarPanelSeleccionModo()
        {
            OcultarTodosLosPaneles();
            if (panelSeleccionJugadores != null) panelSeleccionJugadores.SetActive(true);
        }

        public void MostrarPanel1Jugador()
        {
            OcultarTodosLosPaneles();
            if (panel1Jugador != null) panel1Jugador.SetActive(true);
        }

        public void MostrarPanelJuego()
        {
            OcultarTodosLosPaneles();
            if (panelJuego != null) panelJuego.SetActive(true);
        }

        public void MostrarPanelEspera()
        {
            OcultarTodosLosPaneles();
            if (panelEspera != null) panelEspera.SetActive(true);
        }

        public void MostrarPanelMultijugador()
        {
            OcultarTodosLosPaneles();
            panelMultijugador.SetActive(true);

            // Asegurarse de que los hijos estén activos
            var panelJuego = panelMultijugador.GetComponent<PanelJuegoMultijugador>();
            if (panelJuego != null)
            {
                if (panelJuego.panelJugador1 != null)
                    panelJuego.panelJugador1.gameObject.SetActive(true);

                if (panelJuego.panelJugador2 != null)
                    panelJuego.panelJugador2.gameObject.SetActive(true);
            }
        }
        public void MostrarPanelResultado()
        {
            OcultarTodosLosPaneles();
            if (panelResultado == null) return;

            panelResultado.SetActive(true);

            ResultadoUI resultadoUI = panelResultado.GetComponent<ResultadoUI>();
            if (resultadoUI == null) return;

            resultadoUI.MostrarResultado();
        }

        public void MostrarPanelResultadoMultijugador(int ganador)
        {
            OcultarTodosLosPaneles();
            if (panelResultadoMultijugador == null) return;

            panelResultadoMultijugador.SetActive(true);
            panelResultadoMultijugador
                .GetComponent<ResultadoMultijugadorUI>()
                .MostrarResultado(ganador);
        }

        private void OcultarTodosLosPaneles()
        {
            if (panelSeleccionJugadores != null) panelSeleccionJugadores.SetActive(false);
            if (panel1Jugador != null) panel1Jugador.SetActive(false);
            if (panelMultijugador != null) panelMultijugador.SetActive(false);
            if (panelResultado != null) panelResultado.SetActive(false);
            if (panelResultadoMultijugador != null) panelResultadoMultijugador.SetActive(false);
            if (panelSeleccionModo != null) panelSeleccionModo.SetActive(false);
            if (panelEspera != null) panelEspera.SetActive(false);
        }
    }
}
