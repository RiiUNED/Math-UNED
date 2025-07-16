using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MultiplicationGame.View;

namespace Tests.View
{
    public class GameUITests
    {
        private GameObject _gameObject;
        private GameUI _ui;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject();
            _ui = _gameObject.AddComponent<GameUI>();

            _ui.preguntaText = new GameObject().AddComponent<TextMeshProUGUI>();

            // Configurar TMP_InputField con sus componentes requeridos
            var inputGO = new GameObject("InputField");
            inputGO.AddComponent<RectTransform>();
            inputGO.AddComponent<CanvasRenderer>();
            var text = inputGO.AddComponent<TextMeshProUGUI>();
            var image = inputGO.AddComponent<Image>();
            var input = inputGO.AddComponent<TMP_InputField>();
            input.textComponent = text;
            input.targetGraphic = image;
            _ui.inputField = input;

            _ui.panelInicio = new GameObject();
            _ui.panelJuego = new GameObject();
            _ui.panelResultado = new GameObject();
            _ui.resultadoFinalText = new GameObject().AddComponent<TextMeshProUGUI>();
            _ui.botonEnviar = new GameObject().AddComponent<Button>();
            _ui.botonIniciar = new GameObject().AddComponent<Button>();

            _ui.puntosAcierto = new Image[10];
            for (int i = 0; i < _ui.puntosAcierto.Length; i++)
            {
                _ui.puntosAcierto[i] = new GameObject().AddComponent<Image>();
            }

            _ui.Inicializar();
        }

        [Test]
        public void ActualizarPregunta_ActualizaTextoCorrectamente()
        {
            _ui.ActualizarPregunta("¿Cuánto es 3 × 4?");
            Assert.AreEqual("¿Cuánto es 3 × 4?", _ui.preguntaText.text);
        }

        [Test]
        public void MarcarAcierto_CambiaColorDelPunto()
        {
            _ui.colorAcierto = Color.cyan;
            _ui.MarcarAcierto(2);
            Assert.AreEqual(Color.cyan, _ui.puntosAcierto[2].color);
        }

        [Test]
        public void MarcarAcierto_IgnoraIndiceInvalido()
        {
            Assert.DoesNotThrow(() => _ui.MarcarAcierto(-1));
            Assert.DoesNotThrow(() => _ui.MarcarAcierto(100));
        }

        [Test]
        public void MostrarResultadoFinal_MuestraPanelFinal()
        {
            _ui.panelJuego.SetActive(true);
            _ui.panelResultado.SetActive(false);

            _ui.MostrarResultadoFinal();

            Assert.IsFalse(_ui.panelJuego.activeSelf);
            Assert.IsTrue(_ui.panelResultado.activeSelf);
            StringAssert.Contains("¡Fin del juego!", _ui.resultadoFinalText.text);
        }

        [Test]
        public void AgregarDigito_ConcatenaTextoCorrectamente()
        {
            _ui.inputField.text = "4";
            _ui.AgregarDigito("2");
            Assert.AreEqual("42", _ui.inputField.text);
        }

        [Test]
        public void BorrarUltimoDigito_EliminaUltimoCaracter()
        {
            _ui.inputField.text = "123";
            _ui.BorrarUltimoDigito();
            Assert.AreEqual("12", _ui.inputField.text);
        }

        [Test]
        public void BorrarUltimoDigito_NoLanzaErrorConTextoVacio()
        {
            _ui.inputField.text = "";
            Assert.DoesNotThrow(() => _ui.BorrarUltimoDigito());
        }
    }
}
