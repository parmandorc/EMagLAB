using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class Dome : MonoBehaviour
{
    private Vortex mVortex;
    private Collider mCollider;
    private MeshRenderer mRenderer;
    private bool mPreviousVortexEnabled;

    void Awake()
    {
        mCollider = GetComponent<Collider>();
        mRenderer = GetComponent<MeshRenderer>();
    }
	
	void Update ()
    {
        if (mVortex == null)
        {
            // Get a reference to the Vortex object
            /* This should usually be done in the Awake or Start function.
             *  However, this wasn't consistent in multiplayer: in some clients, the Vortex
             *  component hadn't been created when reaching this component's Awake/Start.
             *  Until a more elegant solution is found, this will get a reference to the Vortex.
             */ 
            mVortex = FindObjectOfType<Vortex>();
            if (mVortex != null)
            {
                mPreviousVortexEnabled = mVortex.IsEnabled;
                if (!mVortex.IsEnabled) OnVortexDisabled();
            }
            return;
        }

        if (mVortex.IsEnabled && !mPreviousVortexEnabled)
        {
            OnVortexEnabled();
        }
        else if (!mVortex.IsEnabled && mPreviousVortexEnabled)
        {
            OnVortexDisabled();
        }

        mPreviousVortexEnabled = mVortex.IsEnabled;
	}

    void OnVortexEnabled()
    {
        mCollider.enabled = false;
        mRenderer.enabled = false;
    }

    void OnVortexDisabled()
    {
        mCollider.enabled = true;
        mRenderer.enabled = true;
    }
}
