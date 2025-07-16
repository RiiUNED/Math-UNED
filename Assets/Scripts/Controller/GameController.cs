using System;
using MultiplicationGame.Model;

namespace MultiplicationGame.Controller
{
    public class GameController
    {
        private GameSession _session;

        public event Action<string> OnPreguntaCambiada;
        public event Action OnJuegoFinalizado;
        public event Action<int> OnAciertoRegistrado;

        public void IniciarJuego(int tabla, bool tablaAleatoria = false)
        {
            _session = new GameSession(tabla, tablaAleatoria);
            EmitirPregunta();
        }

        public void EnviarRespuesta(int respuesta)
        {
            if (_session == null || _session.IsFinished)
                return;

            int aciertosAntes = _session.CorrectAnswers;

            _session.SubmitAnswer(respuesta);

            int aciertosAhora = _session.CorrectAnswers;

            if (aciertosAhora > aciertosAntes)
            {
                OnAciertoRegistrado?.Invoke(aciertosAhora - 1);
            }

            if (_session.IsFinished)
            {
                OnJuegoFinalizado?.Invoke();
            }
            else
            {
                EmitirPregunta();
            }
        }

        private void EmitirPregunta()
        {
            var ejercicio = _session.CurrentExercise;
            string texto = $"¿Cuánto es {ejercicio.Multiplicando1} × {ejercicio.Multiplicando2}?";
            OnPreguntaCambiada?.Invoke(texto);
        }

        public int ObtenerAciertos()
        {
            return _session?.CorrectAnswers ?? 0;
        }
    }
}
