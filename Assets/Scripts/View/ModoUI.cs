using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using MultiplicationGame.Controller;

namespace MultiplicationGame.View
{
    public class ModoUI : MonoBehaviour
    {
        [SerializeField] private Button botonLocal;
        [SerializeField] private Button botonOnline;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private string servidorURL = "";

        private void Start()
        {
            botonLocal.onClick.AddListener(SeleccionarLocal);
            botonOnline.onClick.AddListener(SeleccionarOnline);
        }

        private void SeleccionarLocal()
        {
            uiManager.IniciarModoMultijugadorLocal();
        }

        private void SeleccionarOnline()
        {
            /*
            if (SesionController.SesionYaIniciada())
            {
                Debug.LogWarning("Ya existe una sesión activa. No se iniciará otra.");
                return;
            }
            */
            StartCoroutine(ConectarAlServidor());
        }


        private IEnumerator ConectarAlServidor()
        {
            string cuerpo = "{}";
            using (UnityWebRequest request = new UnityWebRequest(servidorURL, "POST"))
            {
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(cuerpo);
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string respuesta = request.downloadHandler.text;

                    SesionController.RegistrarSesionDesdeJson(respuesta);
                    uiManager.MostrarPanelEspera();
                    //SesionController.MostrarDatosSesion();

                }
                else
                {
                    Debug.LogError("❌ Error en la conexión: " + request.error);
                }
            }
        }
    }
}
