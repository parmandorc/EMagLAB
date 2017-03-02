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

    [SerializeField]
    private bool AffectsScore = false;

    private Player mPlayer;
    private Rigidbody mBody;
    
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
    void FindClosestPlayer()
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
                    player = players[i];
                }
            }
        }

        if (player != mPlayer)
        {
            // If player changed, update the scores (if this units affects the score).
            if (AffectsScore && mPlayer != null) mPlayer.GetComponent<Score>().DecrementScore(tag);
            mPlayer = player;
            if (AffectsScore && mPlayer != null) mPlayer.GetComponent<Score>().IncrementScore(tag);
        }
    }

    // When the particle is destroyed, update player's score
    void OnDestroy()
    {
        if (AffectsScore && mPlayer != null)
        {
            Score score = mPlayer.GetComponent<Score>();
            if (score != null)
            {
                score.DecrementScore(tag);
            }
        }
    }
}
