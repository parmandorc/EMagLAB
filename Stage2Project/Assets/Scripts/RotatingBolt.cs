using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBolt : Bolt
{
    [SerializeField]
    private float TimeBetweenSnaps = 1.0f;

    [SerializeField]
    private float RotatingSpeed = 100.0f;

    private float mNextSnap;
    private Vector3 mRotation;

	protected override void Awake ()
    {
        base.Awake();

        Snap();
	}
	
	protected override void Update ()
    {
        base.Update();

        if (GameManager.GameState == GameManager.State.GameOver) return;

        mNextSnap -= Time.deltaTime;
        if (mNextSnap <= 0.0f)
        {
            Snap();
            mNextSnap = TimeBetweenSnaps;
        }

        // Rotate the bolt around the origin.
        // Use custom rotation instead of Slerp since we don't want linear trayectories, but curved and chaotic ones.
        transform.Rotate(mRotation[0] * RotatingSpeed * Time.deltaTime, 0.0f, 0.0f, Space.Self);
        transform.Rotate(0.0f, mRotation[1] * RotatingSpeed * Time.deltaTime, 0.0f, Space.World);
        if (transform.eulerAngles[0] < 180.0f)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles[0] = 0.0f;
            transform.rotation = Quaternion.Euler(eulerAngles);
        }
    }

    void Snap()
    {
        transform.rotation = Quaternion.Euler(Random.Range(0.0f, -90.0f), Random.Range(0, 360.0f), 0.0f);
        mRotation = new Vector3(Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f), 0.0f).normalized;
    }
}
