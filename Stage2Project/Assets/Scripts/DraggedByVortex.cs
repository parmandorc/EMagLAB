using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DraggedByVortex : MonoBehaviour
{
    private Vortex mVortex;
    private Rigidbody mBody;

    void Awake()
    {
        mVortex = FindObjectOfType<Vortex>();
        mBody = GetComponent<Rigidbody>();
    }

    void Update ()
    {
		if (mVortex.IsEnabled && GameManager.GameState == GameManager.State.Playing)
        {
            Vector3 force = mVortex.transform.position - transform.position;
            force.Normalize();
            mBody.AddForce(force * mVortex.DragForce);
        }
	}
}
