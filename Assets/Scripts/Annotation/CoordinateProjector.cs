using UnityEngine;

public class CoordinateProjector : MonoBehaviour {
    [SerializeField] private Camera virtualCamera;   // Réplica parámetros Quest
    [SerializeField] private Transform qrPlane;      // Recibido de Quest 3 por TCP

    void Update() {
        // Detectamos si el usuario hace clic izquierdo con el ratón
        if (Input.GetMouseButtonDown(0)) {
            // Convertimos la posición del ratón en la pantalla a coordenadas 3D
            Vector3 localPos = ScreenToQRLocal(Input.mousePosition);
            Debug.Log($"¡Clic detectado! Coordenadas 3D relativas al QR: {localPos}");
        }
    }

    // Convierte posición en pantalla del PC a Vector3 relativo al QR
    public Vector3 ScreenToQRLocal(Vector2 screenPos) {
        // Lanzamos un rayo invisible desde la cámara hasta donde hemos hecho clic
        Ray ray = virtualCamera.ScreenPointToRay(screenPos);

        // Creamos un plano matemático infinito que representa la orientación del QR
        Plane qrPlaneWorld = new Plane(qrPlane.forward, qrPlane.position);
        float dist;

        // Calculamos dónde choca el rayo con ese plano
        if (qrPlaneWorld.Raycast(ray, out dist)) {
            Vector3 worldHit = ray.GetPoint(dist);
            // Convertimos la coordenada global a coordenada local del QR y la devolvemos
            return qrPlane.InverseTransformPoint(worldHit);
        }

        return Vector3.zero; // Por si el rayo no choca (muy raro)
    }
}