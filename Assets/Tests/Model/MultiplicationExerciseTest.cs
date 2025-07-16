using NUnit.Framework;
using MultiplicationGame.Model;

namespace Tests.Model
{
    public class MultiplicationExerciseTests
    {
        [Test]
        public void Constructor_AsignaValoresCorrectamente()
        {
            for (int m1 = 1; m1 <= 9; m1++)
            {
                for (int m2 = 1; m2 <= 9; m2++)
                {
                    var ejercicio = new MultiplicationExercise(m1, m2);

                    Assert.AreEqual(m1, ejercicio.Multiplicando1, $"Multiplicando1 incorrecto para {m1} × {m2}");
                    Assert.AreEqual(m2, ejercicio.Multiplicando2, $"Multiplicando2 incorrecto para {m1} × {m2}");
                    Assert.AreEqual(m1 * m2, ejercicio.ResultadoCorrecto, $"Resultado incorrecto para {m1} × {m2}");
                }
            }
        }

        [Test]
        public void EsRespuestaCorrecta_DevuelveTrue_ParaTodasLasCombinacionesDel1Al9()
        {
            for (int m1 = 1; m1 <= 9; m1++)
            {
                for (int m2 = 1; m2 <= 9; m2++)
                {
                    var ejercicio = new MultiplicationExercise(m1, m2);
                    int respuesta = m1 * m2;
                    Assert.IsTrue(ejercicio.EsRespuestaCorrecta(respuesta), 
                        $"Fallo para operación {m1} × {m2} con respuesta {respuesta}");
                }
            }
        }

        [Test]
        public void EsRespuestaCorrecta_DevuelveFalse_ParaRespuestaIncorrecta()
        {
            var ejercicio = new MultiplicationExercise(7, 8);
            Assert.IsFalse(ejercicio.EsRespuestaCorrecta(50));
        }

        [Test]
        public void EsRespuestaCorrecta_DevuelveFalse_SiRespuestaEsNegativa()
        {
            var ejercicio = new MultiplicationExercise(3, 4);
            Assert.IsFalse(ejercicio.EsRespuestaCorrecta(-12));
        }

        [Test]
        public void ResultadoCorrecto_EsConsistente_EnMultiplesConsultas()
        {
            var ejercicio = new MultiplicationExercise(5, 6);
            var resultado1 = ejercicio.ResultadoCorrecto;
            var resultado2 = ejercicio.ResultadoCorrecto;
            Assert.AreEqual(resultado1, resultado2);
        }

        [Test]
        public void Constructor_AceptaMultiplicandosDel1Al9()
        {
            for (int i = 1; i <= 9; i++)
            {
                var ejercicio = new MultiplicationExercise(i, i);
                Assert.AreEqual(i * i, ejercicio.ResultadoCorrecto);
            }
        }

        [Test]
        public void NingunMultiplicandoDebeSerCero()
        {
            for (int i = 0; i < 100; i++)
            {
                var session = new GameSession(5);
                var e = session.CurrentExercise;

                Assert.AreNotEqual(0, e.Multiplicando1);
                Assert.AreNotEqual(0, e.Multiplicando2);
            }
        }
    }
}
