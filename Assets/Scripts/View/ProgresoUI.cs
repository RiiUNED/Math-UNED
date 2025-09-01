using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class TwoColorProgressBar : MonoBehaviour
{
    [Header("Colores")]
    [SerializeField] private Color colorA = Color.red;     // Color para las primeras 'count' imágenes
    [SerializeField] private Color colorB = Color.gray;    // Color para el resto

    [Header("Progreso")]
    [Min(0)]
    [SerializeField] private int count = 0;                // Para pruebas en el inspector
    [Tooltip("Si está activo, pinta desde la izquierda hacia la derecha. Si no, desde la derecha hacia la izquierda.")]
    [SerializeField] private bool leftToRight = true;

    [Header("Opcional")]
    [Tooltip("Refresca automáticamente el array de imágenes al cambiar hijos en el editor/tiempo de ejecución.")]
    [SerializeField] private bool autoRefreshChildren = true;

    private Image[] images;

    // --- API pública ---
    public void SetProgress(int value)
    {
        count = Mathf.Max(0, value);
        ApplyColors();
    }

    public void SetColors(Color first, Color second)
    {
        colorA = first;
        colorB = second;
        ApplyColors();
    }

    public void SetDirection(bool fillLeftToRight)
    {
        leftToRight = fillLeftToRight;
        ApplyColors();
    }

    // --- Ciclo de vida ---
    private void Awake()
    {
        CacheChildren();
        ApplyColors();
    }

#if UNITY_EDITOR
    // Se llama cuando cambias algo en el inspector: útil para ver el resultado al instante
    private void OnValidate()
    {
        CacheChildren();
        ApplyColors();
    }
#endif

    private void OnTransformChildrenChanged()
    {
        if (autoRefreshChildren)
        {
            CacheChildren();
            ApplyColors();
        }
    }

    // --- Internos ---
    private void CacheChildren()
    {
        // Toma todas las imágenes hijas directas o anidadas (activas o inactivas)
        images = GetComponentsInChildren<Image>(true);

        // Opcional: filtrar si la primera imagen es del fondo de la barra u otros
        // (descomenta y ajusta la condición si necesitas excluir alguna)
        // images = images.Where(img => img.gameObject != this.gameObject).ToArray();
    }

    private void ApplyColors()
    {
        if (images == null || images.Length == 0) return;

        int total = images.Length;
        int filled = Mathf.Clamp(count, 0, total);

        for (int i = 0; i < total; i++)
        {
            int logicalIndex = leftToRight ? i : (total - 1 - i);
            images[logicalIndex].color = (i < filled) ? colorA : colorB;
        }
    }
}
