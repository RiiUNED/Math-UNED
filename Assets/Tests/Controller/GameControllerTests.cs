using NUnit.Framework;
using MultiplicationGame.Controller;
using System;

namespace Tests.Controller
{
    public class GameControllerTests
    {
        private GameController _controller;
        private string _preguntaRecibida;
        private bool _juegoFinalizado;

        [SetUp]
        public void SetUp()
        {
            _controller = new GameController();

            // Subscribir a eventos
            _controller.OnPreguntaCambiada += p => _preguntaRecibida = p;
            _controller.OnJuegoFinalizado += () => _juegoFinalizado = true;

            _preguntaRecibida = null;
            _juegoFinalizado = false;
        }

        [Test]
        public void IniciarJuego_EmitePreguntaInicial()
        {
            _controller.IniciarJuego(2);
            Assert.IsNotNull(_preguntaRecibida);
            StringAssert.Contains("2 ×", _preguntaRecibida);
        }

        [Test]
        public void EnviarRespuesta_Alcanzar10Aciertos_FinalizaJuego()
        {
            _controller.IniciarJuego(4);

            var sesion = typeof(GameController)
                .GetField("_session", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(_controller) as MultiplicationGame.Model.GameSession;

            for (int i = 0; i < 10; i++)
            {
                int resultado = sesion.CurrentExercise.ResultadoCorrecto;
                _controller.EnviarRespuesta(resultado);
            }

            Assert.IsTrue(_juegoFinalizado);
        }

        [Test]
        public void EnviarRespuesta_Correcta_DisparaEventoOnAciertoRegistradoConIndiceCorrecto()
        {
            int? aciertoRecibido = null;

            _controller.OnAciertoRegistrado += (index) => aciertoRecibido = index;

            _controller.IniciarJuego(3);

            var sesion = typeof(GameController)
                .GetField("_session", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(_controller) as MultiplicationGame.Model.GameSession;

            int resultado = sesion.CurrentExercise.ResultadoCorrecto;

            _controller.EnviarRespuesta(resultado);

            Assert.AreEqual(0, aciertoRecibido);
        }

        [Test]
        public void EnviarRespuesta_Incorrecta_NoDisparaEventoOnAciertoRegistrado()
        {
            bool eventoLlamado = false;

            _controller.OnAciertoRegistrado += (_) => eventoLlamado = true;

            _controller.IniciarJuego(5);
            _controller.EnviarRespuesta(-1); // respuesta claramente incorrecta

            Assert.IsFalse(eventoLlamado);
        }
    }
}
