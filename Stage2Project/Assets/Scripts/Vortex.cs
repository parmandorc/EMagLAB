﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* This class serves as a central point of information about the vortex.
 * However, it is easier that each object handles their own dragging behaviour with the DraggedByVortex component,
 *  since if not, the Vortex object would have to get references to all draggable objects (what if a new object
 *  is spawned after the vortex has been activated?).
 */ 
public class Vortex : NetworkBehaviour {

    [SerializeField]
    private float Force = 1000.0f;

    public float DragForce { get { return Force; } }

    public bool IsEnabled;

    void OnTriggerEnter(Collider other)
    {
        if (isServer && IsEnabled)
        {
            Destroy(other.gameObject);
        }
    }
}