﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour
{
    [SerializeField]
    private float Speed;

    private Rigidbody mBody;
    private Plane mPlayerPlane;
    private Camera mCamera;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        GameManager manager = FindObjectOfType<GameManager>();
        if (manager != null)
        {
            manager.RegisterLocalPlayer(this);
        }
    }

    void Awake()
    {
        mBody = GetComponent<Rigidbody>();
        mCamera = GameObject.FindGameObjectWithTag("GameCamera").GetComponent<Camera>();
    }

    private void Start()
    {
        mPlayerPlane = new Plane(Vector3.up, transform.position);
    }

    // Moved player force input to FixedUpdate, since its better suited for rigidbody manipulation
    void FixedUpdate()
    {
        if (!isLocalPlayer || GameManager.GameState == GameManager.State.GameOver) return;

        // Look at mouse
        Ray mouseRay = mCamera.ScreenPointToRay(Input.mousePosition);
        float rayDistance = 0.0f;
        if (mPlayerPlane.Raycast(mouseRay, out rayDistance))
        {
            mBody.MoveRotation(Quaternion.LookRotation(mouseRay.GetPoint(rayDistance) - transform.position));
        }

        // Thurst
        float thrust = Input.GetAxis("Thrust");
        if (thrust > 0.0f)
        {
            mBody.AddForce(transform.forward * thrust * Speed * Time.deltaTime);
        }
    }
}
