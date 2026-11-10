using UnityEngine;
using UnityEngine.UI;

// Reproduce los efectos: clic en botones, recoger objeto, victoria y derrota.
// Escucha los eventos del GameManager (Observer), así que no hay que tocar los otros scripts.
[RequireComponent(typeof(AudioSource))]
public class SfxManager : MonoBehaviour
{
    [SerializeField] private AudioClip clic;
    [SerializeField] private AudioClip recoger;
    [SerializeField] private AudioClip victoria;
    [SerializeField] private AudioClip derrota;

    private AudioSource fuente;

    private void Awake()
    {
        fuente = GetComponent<AudioSource>();
        fuente.playOnAwake = false;
        fuente.spatialBlend = 0f; // sonido 2D: se oye igual en todo el mapa
    }

    private void Start()
    {
        // Todos los botones de la escena (incluso los de paneles ocultos) suenan al hacer clic.
        foreach (Button b in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            b.onClick.AddListener(() => fuente.PlayOneShot(clic));

        // En el menú principal no hay GameManager, así que se comprueba.
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnPertenenciaRecogida += AlRecoger;
        GameManager.Instance.OnVictoria += AlGanar;
        GameManager.Instance.OnDerrota += AlPerder;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnPertenenciaRecogida -= AlRecoger;
        GameManager.Instance.OnVictoria -= AlGanar;
        GameManager.Instance.OnDerrota -= AlPerder;
    }

    private void AlRecoger(int recogidas, int necesarias) => fuente.PlayOneShot(recoger);
    private void AlGanar() => fuente.PlayOneShot(victoria);
    private void AlPerder() => fuente.PlayOneShot(derrota);
}