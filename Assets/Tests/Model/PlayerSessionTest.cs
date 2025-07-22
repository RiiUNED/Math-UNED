using NUnit.Framework;
using MultiplicationGame.Model;

namespace Tests.Editor
{
    public class GameSessionTests
    {
        [Test]
        public void Constructor_InicializaCorrectamenteConTablaFija()
        {
            var session = new PlayerSession(3);

            Assert.AreEqual(3, session.Table);
            Assert.IsFalse(session.TablaAleatoria);
            Assert.AreEqual(0, session.CorrectAnswers);
            Assert.IsNotNull(session.CurrentExercise);
            Assert.AreEqual(3, session.CurrentExercise.Multiplicando1);
        }

        [Test]
        public void Constructor_InicializaCorrectamenteConTablaAleatoria()
        {
            var session = new PlayerSession(3, true);

            Assert.IsTrue(session.TablaAleatoria);
            Assert.IsNotNull(session.CurrentExercise);
            Assert.GreaterOrEqual(session.CurrentExercise.Multiplicando1, 1);
            Assert.LessOrEqual(session.CurrentExercise.Multiplicando1, 9);
        }

        [Test]
        public void SubmitAnswer_RespuestaIncorrecta_NoIncrementaCorrectAnswers()
        {
            var session = new PlayerSession(3, false);
            int respuestaIncorrecta = session.CurrentExercise.ResultadoCorrecto + 1;

            session.SubmitAnswer(respuestaIncorrecta);

            Assert.AreEqual(0, session.CorrectAnswers);
            Assert.AreSame(session.CurrentExercise, session.CurrentExercise); // No cambia
        }

        [Test]
        public void SubmitAnswer_RespuestaCorrecta_IncrementaCorrectAnswers()
        {
            var session = new PlayerSession(2, false);
            int respuesta = session.CurrentExercise.ResultadoCorrecto;

            var anterior = session.CurrentExercise;
            session.SubmitAnswer(respuesta);

            Assert.AreEqual(1, session.CorrectAnswers);
            Assert.AreNotSame(anterior, session.CurrentExercise); // Cambia de ejercicio
        }

        [Test]
        public void SubmitAnswer_NoCambiaEjercicioSiYaFinalizado()
        {
            var session = new PlayerSession(3);
            // Simular 10 respuestas correctas
            for (int i = 0; i < 10; i++)
                session.SubmitAnswer(session.CurrentExercise.ResultadoCorrecto);

            Assert.IsTrue(session.IsFinished);
            var anterior = session.CurrentExercise;

            session.SubmitAnswer(anterior.ResultadoCorrecto);

            Assert.AreSame(anterior, session.CurrentExercise); // No cambia
            Assert.AreEqual(10, session.CorrectAnswers); // No se incrementa más
        }

        [Test]
        public void ForzarNuevoEjercicio_CambiaEjercicioSiNoFinalizado()
        {
            var session = new PlayerSession(5);
            var anterior = session.CurrentExercise;

            session.ForzarNuevoEjercicio();

            Assert.AreNotSame(anterior, session.CurrentExercise);
        }

        [Test]
        public void ForzarNuevoEjercicio_NoHaceNadaSiFinalizado()
        {
            var session = new PlayerSession(4);
            for (int i = 0; i < 10; i++)
                session.SubmitAnswer(session.CurrentExercise.ResultadoCorrecto);

            var anterior = session.CurrentExercise;

            session.ForzarNuevoEjercicio();

            Assert.AreSame(anterior, session.CurrentExercise);
        }
    }
}
