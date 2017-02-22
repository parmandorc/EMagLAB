using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class DraggedByVortex : NetworkBehaviour
{
    // Whether the dragging behaviour is enabled or not.
    /* This should usually be done by enabling/disabling the component.
     *  However, since that can only be done by the server (for authority reasons),
     *  clients were trying to apply the force, which was then corrected by the server,
     *  but was causing the objects to flicker (the native 'enabled' state does not get
     *  synchronized across the network automatically).
     */ 
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
		if (mIsEnabled && mVortex.enabled && GameManager.GameState == GameManager.State.Playing)
        {
            Vector3 force = mVortex.transform.position - transform.position;
            force.Normalize();
            mBody.AddForce(force * mVortex.DragForce);
        }
	}

    // Server must have authority over this
    [ServerCallback]
    public void SetEnabled(bool value)
    {
        mIsEnabled = value;
    }
}
