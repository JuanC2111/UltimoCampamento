using System;
using UnityEngine;

// Singleton: solo existe UN GameManager y cualquier script lo alcanza con GameManager.Instance.
// Usa eventos (Observer) para que la UI y el audio reaccionen sin que este script los conozca.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float tiempoTotal = 300f;       // 5 minutos
    [SerializeField] private int pertenenciasNecesarias = 4; // una por turista

    public event Action<float> OnTiempoCambiado;             // segundos restantes
    public event Action<int, int> OnPertenenciaRecogida;     // recogidas, necesarias
    public event Action OnVictoria;
    public event Action OnDerrota;

    public float TiempoRestante { get; private set; }
    public int Recogidas { get; private set; }
    public bool JuegoTerminado { get; private set; }
    public bool TodasRecogidas => Recogidas >= pertenenciasNecesarias;

    private void Awake()
    {
        // Si ya hay otro GameManager, este sobra: se destruye.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f; // por si veníamos de una pausa
        TiempoRestante = tiempoTotal;
    }

    private void Update()
    {
        if (JuegoTerminado) return;

        TiempoRestante -= Time.deltaTime;
        OnTiempoCambiado?.Invoke(TiempoRestante);

        if (TiempoRestante <= 0f)
        {
            TiempoRestante = 0f;
            Terminar(false);
        }
    }

    public void RecogerPertenencia()
    {
        if (JuegoTerminado) return;
        Recogidas++;
        OnPertenenciaRecogida?.Invoke(Recogidas, pertenenciasNecesarias);
    }

    // La radio llama a esto cuando el jugador la usa.
    public void IntentarUsarRadio()
    {
        if (JuegoTerminado) return;
        if (TodasRecogidas) Terminar(true);
    }

    private void Terminar(bool victoria)
    {
        JuegoTerminado = true;
        Time.timeScale = 0f;                 // congela el juego
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;               // para poder hacer clic en los paneles

        if (victoria) OnVictoria?.Invoke();
        else OnDerrota?.Invoke();
    }
}