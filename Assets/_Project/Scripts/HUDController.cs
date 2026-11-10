using TMPro;
using UnityEngine;

// Muestra el tiempo y las pertenencias. Solo ESCUCHA los eventos del GameManager (Observer):
// el GameManager no sabe que este HUD existe, así que se puede cambiar la UI sin tocarlo.
public class HUDController : MonoBehaviour
{
    [SerializeField] private TMP_Text textoTiempo;
    [SerializeField] private TMP_Text textoPertenencias;

    private void Start()
    {
        var gm = GameManager.Instance;
        gm.OnTiempoCambiado += ActualizarTiempo;
        gm.OnPertenenciaRecogida += ActualizarPertenencias;

        ActualizarTiempo(gm.TiempoRestante);
        ActualizarPertenencias(gm.Recogidas, 4);
    }

    // Importante: nos damos de baja al destruirnos para evitar errores con eventos "fantasma".
    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnTiempoCambiado -= ActualizarTiempo;
        GameManager.Instance.OnPertenenciaRecogida -= ActualizarPertenencias;
    }

    private void ActualizarTiempo(float segundos)
    {
        int min = Mathf.FloorToInt(segundos / 60f);
        int seg = Mathf.FloorToInt(segundos % 60f);
        textoTiempo.text = $"{min:00}:{seg:00}";
        // Rojo en el último minuto para meter presión.
        textoTiempo.color = segundos <= 60f ? new Color(0.8f, 0.1f, 0.1f) : new Color(0.91f, 0.88f, 0.82f);
    }

    private void ActualizarPertenencias(int recogidas, int necesarias)
    {
        textoPertenencias.text = $"Pertenencias: {recogidas}/{necesarias}";
    }
}