using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour
{
    [SerializeField]
    private float Speed;

    private Rigidbody mBody;

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
    }

    void Update()
    {
        if (!isLocalPlayer || GameManager.GameState == GameManager.State.GameOver) return;

        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
            direction = -Vector3.right;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            direction = Vector3.right;
        }

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction += -Vector3.forward;
        }

        mBody.AddForce(direction * Speed * Time.deltaTime);
    }
}
