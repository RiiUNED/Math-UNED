using UnityEngine;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    public class UIManager : MonoBehaviour
    {
        private GameObject panelSeleccionModo;
        private GameObject panel1Jugador;
        private GameObject panelJuego;
        private GameObject panelMultijugador;
        private GameObject panelResultado;

        private PlayerGameController controlador;
        private TurnManagerController controladorMultijugador;

        private void Awake()
        {
            panelSeleccionModo = transform.Find("PanelSeleccionModo")?.gameObject;
            panel1Jugador = transform.Find("Panel1Jugador")?.gameObject;
            panelJuego = transform.Find("PanelJuego1")?.gameObject;
            panelMultijugador = transform.Find("PanelJuegoMultijugador")?.gameObject;
            panelResultado = transform.Find("PanelResultado")?.gameObject;

            if (!panelSeleccionModo || !panel1Jugador || !panelMultijugador || !panelResultado)
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

        public void IniciarModoMultijugador()
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

        public void MostrarPanelSeleccionModo()
        {
            OcultarTodosLosPaneles();
            if (panelSeleccionModo != null) panelSeleccionModo.SetActive(true);
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

        public void MostrarPanelMultijugador()
        {
            OcultarTodosLosPaneles();
            if (panelMultijugador != null) panelMultijugador.SetActive(true);
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

        private void OcultarTodosLosPaneles()
        {
            if (panelSeleccionModo != null) panelSeleccionModo.SetActive(false);
            if (panel1Jugador != null) panel1Jugador.SetActive(false);
            if (panelMultijugador != null) panelMultijugador.SetActive(false);
            if (panelResultado != null) panelResultado.SetActive(false);
        }
    }
}
