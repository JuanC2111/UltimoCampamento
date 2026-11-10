using UnityEngine;
using UnityEngine.UI;

// Barra de volumen general del juego. Se pone en un objeto SIEMPRE activo (el Canvas),
// así el volumen guardado se aplica desde que carga la escena, aunque el panel esté oculto.
public class VolumenMaestro : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private const string Clave = "volumen_juego";

    private void Start()
    {
        float guardado = PlayerPrefs.GetFloat(Clave, 0.8f);
        slider.SetValueWithoutNotify(guardado); // mueve la barra sin disparar el evento
        Aplicar(guardado);
        slider.onValueChanged.AddListener(Aplicar);
    }

    private void OnDestroy()
    {
        if (slider != null) slider.onValueChanged.RemoveListener(Aplicar);
    }

    private void Aplicar(float valor)
    {
        AudioListener.volume = valor;          // volumen de TODO el juego (0 a 1)
        PlayerPrefs.SetFloat(Clave, valor);    // se recuerda aunque cambies de escena
    }
}