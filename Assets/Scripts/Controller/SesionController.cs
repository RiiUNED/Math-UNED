using UnityEngine;

namespace MultiplicationGame.Controller
{
    using MultiplicationGame.Model;

    public static class SesionController
    {
        public static void RegistrarSesion(DatosSesion datos)
        {
            SesionActual.datos = datos;
        }

        public static DatosSesion ObtenerSesion()
        {
            return SesionActual.datos;
        }

        public static void RegistrarSesionDesdeJson(string json)
        {
            //Debug.Log("📦 JSON recibido crudo:\n" + json);
            try
            {
                DatosSesion datos = JsonUtility.FromJson<DatosSesion>(json);
                SesionActual.datos = datos;
            }
            catch
            {
                Debug.LogError("Error al deserializar la sesión.");
            }
        }

        public static void MostrarDatosSesion()
        {
            var datos = SesionActual.datos;

            if (datos != null)
            {
                Debug.Log($"✅ Sesión iniciada (ID: {datos.session_id}, jugador: {datos.numero_jugador}, estado: {datos.status})");
            }
            else
            {
                Debug.LogWarning("⚠️ No hay sesión activa para mostrar.");
            }
        }

        public static bool SesionYaIniciada()
        {
            return SesionActual.datos != null;
        }

    }
}
