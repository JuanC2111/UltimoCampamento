using UnityEngine;
using UnityEngine.SceneManagement;

// Lógica del menú principal: los botones llaman a estos métodos.
public class MenuController : MonoBehaviour
{
    [SerializeField] private string escenaJuego = "Campamento";

    private void Start()
    {
        // Al volver desde el juego el cursor podría seguir bloqueado y el tiempo congelado.
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Jugar()
    {
        SceneManager.LoadScene(escenaJuego);
    }

    public void Salir()
    {
        // En el editor y en WebGL no se puede cerrar la aplicación; solo funciona en el .exe.
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}