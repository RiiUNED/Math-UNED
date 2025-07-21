using NUnit.Framework;
using MultiplicationGame.Controller;
using System;
using System.Collections.Generic;

namespace Tests.Editor
{
    public class GameControllerTests
    {
        private GameController controller;
        private List<string> preguntasEmitidas;
        private List<int> aciertosReportados;
        private bool juegoFinalizado;
        private bool skipsAgotados;

        [SetUp]
        public void SetUp()
        {
            controller = new GameController();
            preguntasEmitidas = new List<string>();
            aciertosReportados = new List<int>();
            juegoFinalizado = false;
            skipsAgotados = false;

            controller.OnPreguntaCambiada += texto => preguntasEmitidas.Add(texto);
            controller.OnAciertoRegistrado += acierto => aciertosReportados.Add(acierto);
            controller.OnJuegoFinalizado += () => juegoFinalizado = true;
            controller.OnSkipsAgotados += () => skipsAgotados = true;
        }

        [Test]
        public void IniciarJuego_EmitePrimeraPregunta()
        {
            controller.IniciarJuego(5);

            Assert.AreEqual(1, preguntasEmitidas.Count);
            StringAssert.Contains("×", preguntasEmitidas[0]);
        }

        [Test]
        public void EnviarRespuesta_Correcta_EmiteNuevaPreguntaYRegistraAcierto()
        {
            controller.IniciarJuego(2);
            var pregunta = preguntasEmitidas[^1];
            var partes = pregunta.Split('×', '?');
            int multiplicando1 = int.Parse(partes[0].Split(' ')[2]);
            int multiplicando2 = int.Parse(partes[1].Trim());
            int respuestaCorrecta = multiplicando1 * multiplicando2;

            controller.EnviarRespuesta(respuestaCorrecta);

            Assert.AreEqual(2, preguntasEmitidas.Count); // Se emite una nueva
            Assert.AreEqual(1, aciertosReportados.Count);
            Assert.AreEqual(0, aciertosReportados[0]);
        }

        [Test]
        public void EnviarRespuesta_Incorrecta_NoRegistraAcierto()
        {
            controller.IniciarJuego(3);
            string preguntaAntes = preguntasEmitidas[^1];
            int preguntasAntes = preguntasEmitidas.Count;

            controller.EnviarRespuesta(999); // incorrecta

            Assert.AreEqual(0, aciertosReportados.Count, "No debe registrarse un acierto");
            Assert.AreEqual(preguntaAntes, preguntasEmitidas[^1], "La pregunta debe ser la misma");
            Assert.GreaterOrEqual(preguntasEmitidas.Count, preguntasAntes, "La pregunta puede haberse reemitido");
        }


        [Test]
        public void Juego_Finaliza_AlAcertar10()
        {
            controller.IniciarJuego(1);
            for (int i = 0; i < 10; i++)
            {
                var pregunta = preguntasEmitidas[^1];
                var partes = pregunta.Split('×', '?');
                int m1 = int.Parse(partes[0].Split(' ')[2]);
                int m2 = int.Parse(partes[1].Trim());
                controller.EnviarRespuesta(m1 * m2);
            }

            Assert.AreEqual(10, aciertosReportados.Count);
            Assert.IsTrue(juegoFinalizado);
        }

        [Test]
        public void SaltarEjercicio_CambiaPregunta()
        {
            controller.IniciarJuego(3);
            string anterior = preguntasEmitidas[^1];

            controller.SaltarEjercicio();

            Assert.AreEqual(2, preguntasEmitidas.Count);
            Assert.AreNotEqual(anterior, preguntasEmitidas[^1]);
        }

        [Test]
        public void RegistrarSkip_EmiteEventoCuandoSeAgotan()
        {
            controller.IniciarJuego(5);
            controller.RegistrarSkip();
            controller.RegistrarSkip();
            controller.RegistrarSkip();

            Assert.IsTrue(skipsAgotados);
        }

        [Test]
        public void ObtenerAciertos_DevuelveCantidadCorrecta()
        {
            controller.IniciarJuego(2);
            var pregunta = preguntasEmitidas[^1];
            var partes = pregunta.Split('×', '?');
            int m1 = int.Parse(partes[0].Split(' ')[2]);
            int m2 = int.Parse(partes[1].Trim());

            controller.EnviarRespuesta(m1 * m2);
            Assert.AreEqual(1, controller.ObtenerAciertos());
        }

        [Test]
        public void ObtenerCantidadSkips_DevuelveSkipsRegistrados()
        {
            controller.IniciarJuego(7);
            controller.RegistrarSkip();
            controller.RegistrarSkip();

            Assert.AreEqual(2, controller.ObtenerCantidadSkips());
        }
    }
}
