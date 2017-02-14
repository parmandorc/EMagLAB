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

    void Awake()
    {
        mBody = GetComponent<Rigidbody>();
    }

	void Update()
    {
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
    void FindClosestPlayer()
    {
        mPlayer = null;
        Player[] players = FindObjectsOfType<Player>();
        if (players.Length > 0)
        {
            float minDistance = Vector3.Distance(transform.position, players[0].transform.position);
            mPlayer = players[0];
            for (int i = 1; i < players.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, players[i].transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    mPlayer = players[i];
                }
            }
        }
    }
}
