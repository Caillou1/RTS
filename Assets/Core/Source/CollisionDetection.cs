using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public Action<Collider> TriggerEnterEvent;
    public Action<Collider> TriggerExitEvent;
    public Action<Collision> CollisionEnterEvent;
    public Action<Collision> CollisionExitEvent;

    private void OnTriggerEnter(Collider other) {
        TriggerEnterEvent.Invoke(other);
    }

    private void OnTriggerExit(Collider other) {
        TriggerExitEvent.Invoke(other);
    }

    private void OnCollisionEnter(Collision other) {
        CollisionEnterEvent.Invoke(other);
    }

    private void OnCollisionExit(Collision other) {
        CollisionExitEvent.Invoke(other);
    }
}
