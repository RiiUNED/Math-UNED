using NUnit.Framework;
using MultiplicationGame.Model;
using System.Collections.Generic;

namespace Tests.Model
{
    public class GameSessionTests
    {
        [Test]
        public void NuevaSesion_ComienzaCon0Aciertos()
        {
            var session = new GameSession(5);
            Assert.AreEqual(0, session.CorrectAnswers);
            Assert.IsFalse(session.IsFinished);
        }

        [Test]
        public void SubmitAnswer_RespuestaCorrecta_CuentaAcierto()
        {
            var session = new GameSession(3);
            int respuesta = session.CurrentExercise.ResultadoCorrecto;
            session.SubmitAnswer(respuesta);

            Assert.AreEqual(1, session.CorrectAnswers);
        }

        [Test]
        public void SubmitAnswer_RespuestaIncorrecta_NoCuentaAcierto_YRepiteEjercicio()
        {
            var session = new GameSession(4);
            var ejercicioAntes = session.CurrentExercise;

            session.SubmitAnswer(-1); // Respuesta incorrecta

            var ejercicioDespues = session.CurrentExercise;

            Assert.AreEqual(0, session.CorrectAnswers);
            Assert.AreEqual(ejercicioAntes.Multiplicando1, ejercicioDespues.Multiplicando1);
            Assert.AreEqual(ejercicioAntes.Multiplicando2, ejercicioDespues.Multiplicando2);
        }

        [Test]
        public void SubmitAnswer_Incorrecta_NoCambiaElEjercicio()
        {
            var session = new GameSession(3);
            var ejercicioAntes = session.CurrentExercise;

            int respuestaIncorrecta = ejercicioAntes.ResultadoCorrecto + 1;
            session.SubmitAnswer(respuestaIncorrecta);

            var ejercicioDespues = session.CurrentExercise;

            Assert.AreEqual(ejercicioAntes.Multiplicando1, ejercicioDespues.Multiplicando1);
            Assert.AreEqual(ejercicioAntes.Multiplicando2, ejercicioDespues.Multiplicando2);
            Assert.AreEqual(0, session.CorrectAnswers);
        }

        [Test]
        public void ElJuegoSigueHastaConseguir10Aciertos_ConRespuestasControladas()
        {
            var session = new GameSession(2);
            int aciertosEsperados = 0;

            for (int i = 0; i < 49; i++)
            {
                if ((i % 5 == 0) && aciertosEsperados < 9)
                {
                    session.SubmitAnswer(session.CurrentExercise.ResultadoCorrecto);
                    aciertosEsperados++;
                }
                else
                {
                    session.SubmitAnswer(-1);
                }

                Assert.IsFalse(session.IsFinished, $"El juego terminó antes de tiempo en i = {i}");
            }

            session.SubmitAnswer(session.CurrentExercise.ResultadoCorrecto);
            aciertosEsperados++;

            Assert.AreEqual(10, aciertosEsperados, "No se alcanzaron 10 aciertos como se esperaba");
            Assert.AreEqual(10, session.CorrectAnswers, "La sesión no registró correctamente los aciertos");
            Assert.IsTrue(session.IsFinished, "El juego no terminó al alcanzar los 10 aciertos");
        }

        [Test]
        public void LaSesionFinalizaExactamenteCon10RespuestasCorrectas()
        {
            var session = new GameSession(2);

            for (int i = 0; i < 10; i++)
            {
                session.SubmitAnswer(session.CurrentExercise.ResultadoCorrecto);
            }

            Assert.IsTrue(session.IsFinished);
            Assert.AreEqual(10, session.CorrectAnswers);
        }

        [Test]
        public void MultiplicandoSiempreEnRangoDel1Al9()
        {
            var session = new GameSession(5);
            for (int i = 0; i < 100; i++)
            {
                session.SubmitAnswer(session.CurrentExercise.ResultadoCorrecto);
                var e = session.CurrentExercise;
                Assert.That(e.Multiplicando2, Is.InRange(1, 9));
            }
        }

        [Test]
        public void TablaEsFija_SiModoNoEsAleatorio()
        {
            var session = new GameSession(7);
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(7, session.CurrentExercise.Multiplicando1);
                session.SubmitAnswer(session.CurrentExercise.ResultadoCorrecto);
            }
        }

        [Test]
        public void TablasAleatorias_SiModoAleatorioActivo()
        {
            var session = new GameSession(0, tablaAleatoria: true);
            var tablasVistas = new HashSet<int>();

            for (int i = 0; i < 50; i++)
            {
                tablasVistas.Add(session.CurrentExercise.Multiplicando1);
                session.SubmitAnswer(session.CurrentExercise.ResultadoCorrecto);
            }

            Assert.That(tablasVistas.Count, Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void ElJuegoNoAvanzaAlAcertarSiYaHaFinalizado()
        {
            var session = new GameSession(6);
            for (int i = 0; i < 10; i++)
            {
                session.SubmitAnswer(session.CurrentExercise.ResultadoCorrecto);
            }

            Assert.IsTrue(session.IsFinished);
            int aciertosAntes = session.CorrectAnswers;

            session.SubmitAnswer(session.CurrentExercise.ResultadoCorrecto);

            Assert.AreEqual(aciertosAntes, session.CorrectAnswers);
        }
    }
}
