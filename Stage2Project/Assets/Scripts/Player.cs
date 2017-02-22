using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Score))]
public class Player : NetworkBehaviour
{
    [SerializeField]
    private float Speed;

    [SyncVar]
    private int mPlayerIndex;

    private Rigidbody mBody;
    private Plane mPlayerPlane;
    private Camera mCamera;
    private Color mColor;

    public Color PlayerColor { get { return mColor; } }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        GameManager manager = FindObjectOfType<GameManager>();
        if (manager != null)
        {
            manager.RegisterLocalPlayer(this);
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        GameManager manager = FindObjectOfType<GameManager>();
        if (manager != null)
        {
            mPlayerIndex = manager.AddPlayer(this);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        GameManager manager = FindObjectOfType<GameManager>();
        if (manager != null)
        {
            manager.SetPlayer(this, mPlayerIndex);
        }
    }

    void OnDestroy()
    {
        GameManager manager = FindObjectOfType<GameManager>();
        if (manager != null)
        {
            manager.RemovePlayer(this);
        }
    }

    void Awake()
    {
        mBody = GetComponent<Rigidbody>();
        mCamera = GameObject.FindGameObjectWithTag("GameCamera").GetComponent<Camera>();
    }

    void Start()
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

    public void SetPlayerColor(Color color)
    {
        mColor = color;
        GetComponent<Renderer>().material.color = mColor;
    }
}
