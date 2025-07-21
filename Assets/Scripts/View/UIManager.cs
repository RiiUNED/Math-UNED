using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject panelSeleccionModo;
    public GameObject panel1Jugador;
    public GameObject panelMultijugador;

    public void MostrarPanel1Jugador()
    {
        panelSeleccionModo.SetActive(false);
        panel1Jugador.SetActive(true);
    }

    public void MostrarPanelMultijugador()
    {
        panelSeleccionModo.SetActive(false);
        panelMultijugador.SetActive(true);
    }
}
