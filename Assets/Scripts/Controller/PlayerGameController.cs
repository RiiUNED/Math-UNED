using System;
using MultiplicationGame.Model;

namespace MultiplicationGame.Controller
{
    public class PlayerGameController
    {
        private PlayerSession _session;
        private int contadorSkips = 0; // ← contador agregado

        public event Action<string> OnPreguntaCambiada;
        public event Action OnJuegoFinalizado;
        public event Action<int> OnAciertoRegistrado;
        public event Action OnSkipsAgotados;


        public void IniciarJuego(int tabla, bool tablaAleatoria = false)
        {
            _session = new PlayerSession(tabla, tablaAleatoria);
            contadorSkips = 0; // ← reinicia el contador al iniciar juego
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

        public void SaltarEjercicio()
        {
            if (_session == null || _session.IsFinished)
                return;

            _session.ForzarNuevoEjercicio();
            EmitirPregunta();
        }
        public void RegistrarSkip()
        {
            contadorSkips++;

            if (_session != null && contadorSkips >= _session.MaxSkips)
            {
                OnSkipsAgotados?.Invoke();
            }
        }


        private void EmitirPregunta()
        {
            var ejercicio = _session.CurrentExercise;
            string texto = $"¿Cuánto es {ejercicio.Multiplicando1} × {ejercicio.Multiplicando2}?";
            OnPreguntaCambiada?.Invoke(texto);
        }
        public string ObtenerPreguntaActual()
        {
            if (_session == null || _session.IsFinished)
                return "";

            var ejercicio = _session.CurrentExercise;
            return $"¿Cuánto es {ejercicio.Multiplicando1} × {ejercicio.Multiplicando2}?";
        }


        public int ObtenerAciertos()
        {
            return _session?.CorrectAnswers ?? 0;
        }

        public int ObtenerCantidadSkips() // ← método opcional para consultar el contador
        {
            return contadorSkips;
        }

        public bool PuedeSaltar()
        {
            return _session != null && contadorSkips < _session.MaxSkips;
        }

    }
}
