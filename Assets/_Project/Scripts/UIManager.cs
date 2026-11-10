using UnityEngine;
using UnityEngine.SceneManagement;

// Controla los paneles (victoria, derrota, pausa) y los botones.
// Escucha al GameManager (Observer) para no depender de él.
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private GameObject panelDerrota;
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private string escenaMenu = "MenuPrincipal";

    private bool enPausa;

    private void Start()
    {
        // Los paneles empiezan ocultos; en el editor los dejamos activos solo para diseñarlos.
        panelVictoria.SetActive(false);
        panelDerrota.SetActive(false);
        panelPausa.SetActive(false);

        GameManager.Instance.OnVictoria += MostrarVictoria;
        GameManager.Instance.OnDerrota += MostrarDerrota;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnVictoria -= MostrarVictoria;
        GameManager.Instance.OnDerrota -= MostrarDerrota;
    }

    private void Update()
    {
        // Esc alterna la pausa, pero no si el juego ya terminó.
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.JuegoTerminado)
        {
            if (enPausa) Reanudar();
            else Pausar();
        }
    }

    private void MostrarVictoria() => panelVictoria.SetActive(true);
    private void MostrarDerrota() => panelDerrota.SetActive(true);

    public void Pausar()
    {
        enPausa = true;
        panelPausa.SetActive(true);
        Time.timeScale = 0f;             // congela el juego
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;           // para poder hacer clic
    }

    public void Reanudar()
    {
        enPausa = false;
        panelPausa.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Reintentar()
    {
        Time.timeScale = 1f; // sin esto la escena nueva arrancaría congelada
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenu);
    }
}