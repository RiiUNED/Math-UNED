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
        private GameController controlador;


        private void Awake()
        {
            // Busca los paneles por nombre dentro del Canvas
            panelSeleccionModo = transform.Find("PanelSeleccionModo")?.gameObject;
            panel1Jugador = transform.Find("Panel1Jugador")?.gameObject;
            panelJuego = transform.Find("PanelJuego")?.gameObject;
            panelMultijugador = transform.Find("PanelMultijugador")?.gameObject;
            panelResultado = transform.Find("PanelResultado")?.gameObject;


            if (!panelSeleccionModo || !panel1Jugador || !panelMultijugador || !panelResultado)
            {
                Debug.LogWarning("Algunos paneles no se encontraron en el Canvas. Revisa nombres y jerarquía.");
            }

            MostrarPanelSeleccionModo();
        }

        public void IniciarJuegoConTabla(int tabla)
        {
            controlador = new GameController();

            bool aleatoria = (tabla == 0);
            int tablaFinal = aleatoria ? 1 : tabla;

            panelJuego.GetComponent<JuegoUI>().IniciarJuego(controlador, tablaFinal, aleatoria);

            MostrarPanelJuego();
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
