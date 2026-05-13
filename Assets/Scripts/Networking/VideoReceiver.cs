using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System;

public class VideoReceiver : MonoBehaviour {
    [SerializeField] private RawImage display;
    private UdpClient _server;
    private Texture2D _tex;

    void Start() {
        _tex = new Texture2D(640, 360);
        _server = new UdpClient(47776);
        _server.BeginReceive(OnFrame, null);
    }

    void OnFrame(IAsyncResult ar) {
        IPEndPoint ep = null;
        byte[] data = _server.EndReceive(ar, ref ep);
        
        MainThreadDispatcher.Enqueue(() => {
            _tex.LoadImage(data);
            display.texture = _tex;
        });
        
        _server.BeginReceive(OnFrame, null);
    }
    
    void OnApplicationQuit() {
        if (_server != null) _server.Close();
    }
}