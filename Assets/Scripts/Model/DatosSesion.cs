namespace MultiplicationGame.Model
{
    [System.Serializable]
    public class DatosSesion
    {
        // --- Siempre presentes ---
        public int session_id;
        public int player_id;
        public int numero_jugador;
        public string status;

        // --- En estado "en espera" ---
        public string message; // ej: "La sesión no está activa."

        // --- En estado "listo para jugar" ---
        public int board_id;
        public int op1;
        public int op2;
        public int ex_num;
        public int puntaje;
        public int skips;
        public int rival;
    }

    public static class SesionActual
    {
        public static DatosSesion datos;
    }
}
