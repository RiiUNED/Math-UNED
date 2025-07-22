using UnityEngine;
using UnityEngine.UI;

namespace MultiplicationGame.View
{
    public class ModoUI : MonoBehaviour
    {
        [SerializeField] private Button boton1Jugador;
        [SerializeField] private Button botonMultijugador;
        [SerializeField] private UIManager uiManager;

        private void Start()
        {
            boton1Jugador.onClick.AddListener(Seleccionar1Jugador);
            botonMultijugador.onClick.AddListener(SeleccionarMultijugador);
        }

        private void Seleccionar1Jugador()
        {
            uiManager.MostrarPanel1Jugador();
        }

        private void SeleccionarMultijugador()
        {
            uiManager.IniciarModoMultijugador();
        }
    }
}
