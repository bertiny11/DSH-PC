using UnityEngine;
using System.Net.Sockets;
using System.IO;

public class MockSender : MonoBehaviour {
    private UdpClient _udp;
    private byte[] _imageBytes;

    void Start() {
        _udp = new UdpClient();
        // Busca la imagen en la carpeta principal del proyecto
        string imagePath = Path.Combine(Application.dataPath, "../test.jpg");
        
        if (File.Exists(imagePath)) {
            _imageBytes = File.ReadAllBytes(imagePath);
            InvokeRepeating(nameof(SendMockFrame), 1f, 1f / 20f); 
            Debug.Log("Enviando imagen de prueba...");
        } else {
            Debug.LogError("¡No se encuentra test.jpg! Pon una foto JPG en la misma carpeta que la carpeta Assets.");
        }
    }

    void SendMockFrame() {
        _udp.Send(_imageBytes, _imageBytes.Length, "127.0.0.1", 47776);
    }
}