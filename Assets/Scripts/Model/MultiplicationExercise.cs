using UnityEngine;

namespace MultiplicationGame.Model
{
    public class MultiplicationExercise
    {
        public int Multiplicando1 { get; }
        public int Multiplicando2 { get; }

        public int ResultadoCorrecto => Multiplicando1 * Multiplicando2;

        public MultiplicationExercise(int m1, int m2)
        {
#if UNITY_EDITOR
            if (m1 < 1 || m2 < 1 || m1 > 9 || m2 > 9)
                Debug.LogWarning("Multiplicandos fuera de rango: " + m1 + " × " + m2);
#endif

            Multiplicando1 = m1;
            Multiplicando2 = m2;
        }

        public bool EsRespuestaCorrecta(int respuesta)
        {
            return respuesta == ResultadoCorrecto;
        }

        public override string ToString()
        {
            return $"¿Cuánto es {Multiplicando1} × {Multiplicando2}?";
        }
    }
}
