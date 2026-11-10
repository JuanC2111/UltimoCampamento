using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

// Cinemática de una sola vez: al entrar en la zona, una Cinemachine Camera toma el control
// subiendo su prioridad, muestra la escena unos segundos y devuelve el control al jugador.
public class CameraZoneTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinematicCamera;
    [SerializeField] private float duration = 4f;
    [SerializeField] private int activePriority = 20; // mayor que 0 = esta cámara manda

    private bool played; // evita que la cinemática se repita cada vez que entras

    private void Reset()
    {
        // Al añadir el script deja el collider como Trigger (si ya hay uno).
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void Start()
    {
        // La cámara de la cinemática empieza apagada para no tomar el control al iniciar.
        cinematicCamera.Priority.Value = 0;
        cinematicCamera.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (played || !other.CompareTag("Player")) return;
        played = true;
        StartCoroutine(Reproducir(other));
    }

    private IEnumerator Reproducir(Collider player)
    {
        var control = player.GetComponent<PlayerController3D>();
        Transform cam = Camera.main.transform;

        // Guardamos la posición local de la cámara: Cinemachine la mueve y hay que devolverla.
        Vector3 posLocal = cam.localPosition;
        Quaternion rotLocal = cam.localRotation;

        if (control != null) control.enabled = false; // jugador quieto durante la cinemática

        cinematicCamera.gameObject.SetActive(true);
        cinematicCamera.Priority.Value = activePriority;

        yield return new WaitForSeconds(duration);

        cinematicCamera.Priority.Value = 0;
        cinematicCamera.gameObject.SetActive(false);

        cam.localPosition = posLocal;
        cam.localRotation = rotLocal;
        if (control != null) control.enabled = true;
    }
}