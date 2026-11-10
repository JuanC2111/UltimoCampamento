using UnityEngine;

// La radio de emergencia: el jugador se acerca y pulsa E.
// Solo gana si ya recogió las 4 pertenencias (lo decide el GameManager).
[RequireComponent(typeof(Collider))]
public class Radio : MonoBehaviour
{
    private bool jugadorCerca;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = false;
    }

    // Se revisa la tecla en Update (cada frame) para no perder pulsaciones.
    private void Update()
    {
        if (!jugadorCerca || !Input.GetKeyDown(KeyCode.E)) return;

        if (GameManager.Instance.TodasRecogidas)
            GameManager.Instance.IntentarUsarRadio();
        else
            Debug.Log("Aún faltan pertenencias por encontrar.");
    }
}