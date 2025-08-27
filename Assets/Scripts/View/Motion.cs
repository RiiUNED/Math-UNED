using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Motion : MonoBehaviour
{
    [Header("Puntos de animación")]
    [SerializeField] private List<Image> puntos = new List<Image>();

    [Header("Colores")]
    [SerializeField] private Color colorReposo;
    [SerializeField] private Color colorActivo;

    [Header("Tiempo")]
    [Min(0.01f)]
    [SerializeField] private float intervalo;

    [SerializeField] private bool autocompletarConHijos = true;

    private int indiceActual = -1;
    private Coroutine rutina;

    // Rellena automáticamente con los hijos que tienen Image
    [ContextMenu("Autocompletar con hijos")]
    private void Autocompletar()
    {
        puntos.Clear();
        foreach (Transform t in transform)
        {
            var img = t.GetComponent<Image>();
            if (img != null) puntos.Add(img);
        }
    }

    // Llamado cuando se agrega el script
    private void Reset()
    {
        Autocompletar();
    }

    private void OnEnable()
    {
        if (autocompletarConHijos && (puntos == null || puntos.Count == 0))
            Autocompletar();

        // Evitar errores si no hay puntos
        if (puntos == null || puntos.Count == 0) return;

        // Inicializa todos con el color de reposo
        SetAll(colorReposo);
        indiceActual = -1;

        rutina = StartCoroutine(Animar());
    }

    private void OnDisable()
    {
        if (rutina != null) StopCoroutine(rutina);
        rutina = null;
    }

    private IEnumerator Animar()
    {
        while (true)
        {
            // Avanza al siguiente punto (con loop)
            indiceActual = (indiceActual + 1) % puntos.Count;

            // Pone todos en reposo y destaca el actual
            SetAll(colorReposo);
            if (puntos[indiceActual] != null)
                puntos[indiceActual].color = colorActivo;

            yield return new WaitForSeconds(intervalo);
        }
    }

    private void SetAll(Color c)
    {
        for (int i = 0; i < puntos.Count; i++)
            if (puntos[i] != null) puntos[i].color = c;
    }
}
