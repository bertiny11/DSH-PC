using System;
using UnityEngine;

// [Serializable] es crucial para que JsonUtility de Unity pueda convertir esto a texto
[Serializable]
public class StrokeData {
    public Vector3[] points;   // Array con todos los puntos del trazo [cite: 168]
    public Color color;        // Color de la línea [cite: 169]
    public float width;        // Grosor de la línea [cite: 170]
    public string id;          // Un identificador único para poder borrarla luego [cite: 171]
}