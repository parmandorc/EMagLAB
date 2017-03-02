using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    [SerializeField]
    private float ShakeAmount;

    [SerializeField]
    private float ShakeFrequency;

    private Vector3 mRestPosition;
    private float mShakeTime;
    private bool mIsShaking;

	void Awake ()
    {
        mRestPosition = transform.position;
	}
	
	void Update ()
    {
        if (GameManager.GameState != GameManager.State.Playing && mIsShaking) // Ensure this only shakes during the game
        {
            EndShake();
        }
        
        if (mIsShaking)
        {
            mShakeTime += Time.deltaTime;
            transform.position = mRestPosition + transform.right * Mathf.Sin(mShakeTime * ShakeFrequency) * ShakeAmount;
        }
	}

    public void BeginShake()
    {
        mIsShaking = true;
        mShakeTime = 0.0f;
    }

    public void EndShake()
    {
        mIsShaking = false;
        transform.position = mRestPosition;
    }
}
