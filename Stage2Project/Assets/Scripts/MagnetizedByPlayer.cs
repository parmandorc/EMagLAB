using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagnetizedByPlayer : MonoBehaviour
{
    public enum Type { Attract, Repel }

    [SerializeField]
    private float RepelForce = 1000.0f;

    [SerializeField]
    private float MinimumDistance = 1.0f;

    [SerializeField]
    private Type MagnetizeType = Type.Repel;

    private Player mPlayer;
    private Rigidbody mBody;

    public Player ActivePlayer { get { return mPlayer; } }
    public bool isScore { get { return MagnetizeType == Type.Attract; } }

    void Awake()
    {
        mBody = GetComponent<Rigidbody>();
    }

	void Update()
    {
        if (GameManager.GameState != GameManager.State.Playing) return;

        // Not optimal to compute the closest player every frame. Potential optimization.
        FindClosestPlayer();

        if( mPlayer != null)
        {
            Vector3 difference = MagnetizeType == Type.Repel ? transform.position - mPlayer.transform.position : mPlayer.transform.position - transform.position;
            if( difference.magnitude <= MinimumDistance )
            {
                mBody.AddForce(difference * RepelForce * Time.deltaTime);
            }
        }		
	}

    // In multiplayer, the magnetized object will be affected by the closest player
    Player FindClosestPlayer()
    {
        Player player = null;
        Player[] players = FindObjectsOfType<Player>();
        if (players.Length > 0)
        {
            float minDistance = Mathf.Infinity;
            for (int i = 0; i < players.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, players[i].transform.position);
                if (distance <= MinimumDistance && distance < minDistance)
                {
                    minDistance = distance;
                    mPlayer = players[i];
                }
            }
        }
        return player;
    }
}
