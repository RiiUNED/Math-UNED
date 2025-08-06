using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking; // Necesario para UnityWebRequest
using System.Collections;

namespace MultiplicationGame.View
{
    public class ModoUI : MonoBehaviour
    {
        [SerializeField] private Button botonLocal;
        [SerializeField] private Button botonOnline;
        [SerializeField] private UIManager uiManager;

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
            StartCoroutine(ConectarAlServidor());
        }

        private IEnumerator ConectarAlServidor()
        {
            string url = "http://localhost/test/index.php";
            UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Respuesta del servidor: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error al conectar con el servidor: " + request.error);
            }
        }
    }
}
