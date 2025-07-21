using UnityEngine;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    public class SeleccionTablaUI : MonoBehaviour
    {
        [Header("Paneles")]
        public GameObject panelJuego;

        public GameController controlador { get; private set; }

        private void Awake()
        {
            controlador = new GameController();
        }

        public void SeleccionarTabla(int tabla)
        {
            ResetearVista();

            bool usarAleatoria = (tabla == 0);
            int tablaSeleccionada = usarAleatoria ? 1 : tabla;

            panelJuego.GetComponent<JuegoUI>().IniciarJuego(controlador, tablaSeleccionada, usarAleatoria);

            gameObject.SetActive(false);     // Oculta Panel1Jugador
            panelJuego.SetActive(true);      // Muestra panel de juego
        }

        private void ResetearVista()
        {
            // Quizás después
        }
    }
}


