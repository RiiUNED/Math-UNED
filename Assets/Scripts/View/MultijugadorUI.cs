using UnityEngine;
using UnityEngine.UI;

namespace MultiplicationGame.View
{
    public class MultijugadorUI : MonoBehaviour
    {
        [SerializeField] private Button volverProv;
        [SerializeField] private UIManager uiManager;

        private void Start()
        {
            volverProv.onClick.AddListener(VolverAlMenu);
        }

        private void VolverAlMenu()
        {
            uiManager.MostrarPanelSeleccionModo();
        }
    }
}
