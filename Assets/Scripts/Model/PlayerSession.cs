using System;

namespace MultiplicationGame.Model
{
    public class PlayerSession
    {
        public int MaxSkips { get; } = 3;
        public int Table { get; private set; }
        public bool TablaAleatoria { get; private set; }
        public int CorrectAnswers { get; private set; }
        public MultiplicationExercise CurrentExercise { get; private set; }

        private Random rnd;

        public PlayerSession(int table, bool tablaAleatoria = false)
        {
            Table = table;
            TablaAleatoria = tablaAleatoria;
            CorrectAnswers = 0;
            rnd = new Random();
            GenerateNewExercise();
        }

        public void SubmitAnswer(int answer)
        {
            if (IsFinished)
                return;

            if (CurrentExercise.EsRespuestaCorrecta(answer))
            {
                bool estabaFinalizadoAntes = IsFinished; // ← capturás antes del incremento

                CorrectAnswers++;

                if (!estabaFinalizadoAntes)
                    GenerateNewExercise();
            }

            // Si se falla, el ejercicio actual permanece
        }

        public void ForzarNuevoEjercicio()
        {
            if (!IsFinished)
                GenerateNewExercise();
        }

        private void GenerateNewExercise()
        {
            MultiplicationExercise nuevoEjercicio;
            do
            {
                int multiplicador = TablaAleatoria ? rnd.Next(1, 10) : Table;
                int multiplicando = rnd.Next(1, 10);

                nuevoEjercicio = new MultiplicationExercise(multiplicador, multiplicando);
            }
            while (CurrentExercise != null && CurrentExercise.Equals(nuevoEjercicio));

            CurrentExercise = nuevoEjercicio;
        }


        public bool IsFinished => CorrectAnswers >= 10;
    }
}
