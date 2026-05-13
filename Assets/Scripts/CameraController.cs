using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Ajustes de Cámara")]
    public float sensitivity = 3f; // Sensibilidad del ratón

    private float pitch = 0f; // Rotación vertical (arriba/abajo)
    private float yaw = 0f;   // Rotación horizontal (izquierda/derecha)

    void Update()
    {
        // GetMouseButton(1) es el CLIC DERECHO del ratón
        if (Input.GetMouseButton(1))
        {
            // Leer el movimiento del ratón
            yaw += Input.GetAxis("Mouse X") * sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * sensitivity;

            // Limitar la rotación vertical para que la cabeza no dé volteretas de 360 grados
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            // Aplicar la rotación a la cámara
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
    }
}