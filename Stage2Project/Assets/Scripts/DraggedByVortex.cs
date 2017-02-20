using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class DraggedByVortex : NetworkBehaviour
{
    [SyncVar]
    private bool mIsEnabled;

    private Vortex mVortex;
    private Rigidbody mBody;

    void Awake()
    {
        mVortex = FindObjectOfType<Vortex>();
        mBody = GetComponent<Rigidbody>();
    }

    public override void OnStartServer()
    {
        mIsEnabled = true;
    }

    void Update ()
    {
		if (mIsEnabled && mVortex.IsEnabled && GameManager.GameState == GameManager.State.Playing)
        {
            Vector3 force = mVortex.transform.position - transform.position;
            force.Normalize();
            mBody.AddForce(force * mVortex.DragForce);
        }
	}

    [ServerCallback]
    public void SetEnabled(bool value)
    {
        mIsEnabled = value;
    }
}
