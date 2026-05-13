using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour {
    static Queue<Action> _queue = new Queue<Action>();
    
    public static void Enqueue(Action a) { 
        lock(_queue) _queue.Enqueue(a); 
    }
    
    void Update() {
        while (_queue.Count > 0) { 
            Action a; 
            lock(_queue) a = _queue.Dequeue(); 
            a(); 
        }
    }
}