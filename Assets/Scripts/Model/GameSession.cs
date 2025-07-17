using System;

namespace MultiplicationGame.Model
{
    public class GameSession
    {
        public int MaxSkips { get; } = 3;
        public int Table { get; private set; }
        public bool TablaAleatoria { get; private set; }
        public int CorrectAnswers { get; private set; }
        public MultiplicationExercise CurrentExercise { get; private set; }

        private Random rnd;

        public GameSession(int table, bool tablaAleatoria = false)
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
                CorrectAnswers++;

                if (!IsFinished)
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
            int multiplicador = TablaAleatoria ? rnd.Next(1, 10) : Table;
            int multiplicando = rnd.Next(1, 10); // ambos del 1 al 9

            CurrentExercise = new MultiplicationExercise(multiplicador, multiplicando);
        }

        public bool IsFinished => CorrectAnswers >= 10;
    }
}
