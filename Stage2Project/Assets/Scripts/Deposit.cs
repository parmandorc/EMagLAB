using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deposit : MonoBehaviour
{
    [SerializeField]
    private string AffectedTag;

    private Player mPlayer;
    private Score mScore;
    private List<GameObject> mObjects;

    public Player Player { get { return mPlayer; } }

	void Start ()
    {
        mObjects = new List<GameObject>();
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(AffectedTag))
        {
            mObjects.Add(other.gameObject);
            if (mScore != null)
            {
                mScore.IncrementScore(other.gameObject.tag);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(AffectedTag))
        {
            mObjects.Remove(other.gameObject);
            if (mScore != null)
            {
                mScore.DecrementScore(other.gameObject.tag);
            }
        }
    }

    public void SetPlayer(Player player)
    {
        mPlayer = player;
        mScore = null;
        if (player != null)
        {
            mScore = player.GetComponent<Score>();
        }
    }
}
