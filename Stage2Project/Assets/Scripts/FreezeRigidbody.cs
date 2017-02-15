using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FreezeRigidbody : MonoBehaviour {

    private Rigidbody mBody;

    void Awake()
    {
        mBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
		if (GameManager.GameState == GameManager.State.GameOver)
        {
            if (mBody.velocity.sqrMagnitude > 0.0f)
            {
                mBody.velocity = Vector3.zero;
            }

            if (mBody.angularVelocity.sqrMagnitude > 0.0f)
            {
                mBody.angularVelocity = Vector3.zero;
            }
        }
	}

    public void Freeze()
    {
        mBody.velocity = Vector3.zero;
        mBody.angularVelocity = Vector3.zero;
    }
}
