using UnityEngine;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    public class SeleccionTablaUI : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;

        public void SeleccionarTabla(int tabla)
        {
            uiManager.IniciarJuegoConTabla(tabla); // Delega al director
        }

        private void ResetearVista()
        {
            // Quizás después
        }
    }
}
