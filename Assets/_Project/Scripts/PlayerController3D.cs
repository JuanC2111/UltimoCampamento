using UnityEngine;

// [RequireComponent]: garantiza que el objeto SIEMPRE tenga CharacterController.
// Por qué: sin él, GetComponent devolvería null y el juego fallaría en silencio.
[RequireComponent(typeof(CharacterController))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float walkSpeed = 3.5f;     // en terror se camina lento: da tensión
    [SerializeField] float sprintSpeed = 6f;     // correr se siente como escapar
    [SerializeField] float jumpHeight = 1.0f;    // altura en METROS, no una fuerza
    [SerializeField] float gravity = -18f;       // rango recomendado -15 a -20: caída firme

    [Header("Mouse")]
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float pitchLimit = 80f;     // evita dar la vuelta con la cámara

    CharacterController controller;
    Transform cameraTransform;
    Vector3 velocity;       // velocidad vertical acumulada (salto y caída)
    float pitch;            // ángulo vertical actual de la cámara
    bool enSuelo;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        // La cámara es hija del Player: no hace falta asignarla a mano en el Inspector.
        cameraTransform = GetComponentInChildren<Camera>().transform;
        // Cursor oculto y bloqueado: el mouse no se sale de la ventana.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Mirar();
        Moverse();
    }

    // Girar: el cuerpo gira en horizontal (yaw) y SOLO la cámara en vertical (pitch).
    // Si rotáramos el cuerpo entero hacia arriba, el personaje se inclinaría.
    void Mirar()
    {
        if (Time.timeScale == 0f) return; // en pausa o fin de juego, la cámara no gira
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0f, mx, 0f);
        pitch = Mathf.Clamp(pitch - my, -pitchLimit, pitchLimit);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Moverse()
    {
        // 1) Estado del suelo. Un valor pequeño negativo pega al personaje al terreno.
        enSuelo = controller.isGrounded;
        if (enSuelo && velocity.y < 0f) velocity.y = -2f;

        // 2) Movimiento RELATIVO a hacia dónde mira el personaje (right/forward).
        //    Con Vector3(x,0,z) iría siempre al norte del mundo.
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        if (move.sqrMagnitude > 1f) move.Normalize();   // en diagonal no va más rápido

        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        // Time.deltaTime: convierte "metros por segundo" en "metros este fotograma",
        // así el juego corre igual a 30 que a 144 FPS.
        controller.Move(move * speed * Time.deltaTime);

        // 3) Salto exacto: v = sqrt(h * -2 * g). Se escribe la altura deseada y la
        //    física despeja la velocidad inicial, sin probar números al azar.
        if (Input.GetButtonDown("Jump") && enSuelo)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // 4) Gravedad manual: el Character Controller es cinemático y no la trae.
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}