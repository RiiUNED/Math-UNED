using NUnit.Framework;
using MultiplicationGame.Model;

namespace Tests.Editor
{
    public class MultiplicationExerciseTests
    {
        [Test]
        public void Constructor_ValoresValidos_NoLanzaWarning()
        {
            var exercise = new MultiplicationExercise(3, 4);
            Assert.AreEqual(3, exercise.Multiplicando1);
            Assert.AreEqual(4, exercise.Multiplicando2);
        }

        [Test]
        public void ResultadoCorrecto_CalculoEsCorrecto()
        {
            var exercise = new MultiplicationExercise(6, 7);
            Assert.AreEqual(42, exercise.ResultadoCorrecto);
        }

        [Test]
        public void EsRespuestaCorrecta_RespuestaCorrecta_DevuelveTrue()
        {
            var exercise = new MultiplicationExercise(2, 5);
            Assert.IsTrue(exercise.EsRespuestaCorrecta(10));
        }

        [Test]
        public void EsRespuestaCorrecta_RespuestaIncorrecta_DevuelveFalse()
        {
            var exercise = new MultiplicationExercise(2, 5);
            Assert.IsFalse(exercise.EsRespuestaCorrecta(11));
        }

        [Test]
        public void ToString_FormatoCorrecto()
        {
            var exercise = new MultiplicationExercise(8, 3);
            string expected = "¿Cuánto es 8 × 3?";
            Assert.AreEqual(expected, exercise.ToString());
        }

        [Test]
        public void Equals_DosInstanciasIguales_DevuelveTrue()
        {
            var a = new MultiplicationExercise(3, 3);
            var b = new MultiplicationExercise(3, 3);
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void Equals_InstanciasDiferentes_DevuelveFalse()
        {
            var a = new MultiplicationExercise(2, 3);
            var b = new MultiplicationExercise(3, 2);
            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void GetHashCode_DiferentesMultiplicaciones_GeneranHashDiferente()
        {
            var a = new MultiplicationExercise(2, 3);
            var b = new MultiplicationExercise(3, 2);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}
