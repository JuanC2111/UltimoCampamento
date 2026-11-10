using UnityEngine;

// Objeto recogible: una pertenencia de un turista desaparecido.
// Se recoge al tocarlo (el collider es Trigger) y avisa al GameManager.
[RequireComponent(typeof(Collider))]
public class Pertenencia : MonoBehaviour
{
    [SerializeField] private string nombre = "Mochila de Lucía";

    private void Reset()
    {
        // Se ejecuta al añadir el script: deja el collider como Trigger automáticamente.
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.RecogerPertenencia();
        Debug.Log("Recogido: " + nombre);
        Destroy(gameObject);
    }
}