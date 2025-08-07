namespace MultiplicationGame.Model
{
    [System.Serializable]
    public class DatosSesion
    {
        public int session_id;
        public int player_id;
        public string status;
        public int board_id;
        public int numero_jugador;
    }

    public static class SesionActual
    {
        public static DatosSesion datos;
    }
}
