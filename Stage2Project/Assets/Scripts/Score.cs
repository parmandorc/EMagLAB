using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Score : NetworkBehaviour
{
    [SerializeField]
    private Vector3 ScoreOffset;

    [SyncVar]
    private int mScoreOne;

    [SyncVar]
    private int mScoreTwo;

    private TextMesh mInGameScore;

    public int TotalScore { get { return mScoreOne + mScoreTwo; } }

	public void IncrementScore(string tag) { UpdateScore(tag, true); }
    public void DecrementScore(string tag) { UpdateScore(tag, false); }

    // The Server must have authority on the Scores, so only the server can update the score values.
    private void UpdateScore(string tag, bool doIncrement)
    {
        if (isServer)
        {
            int value = doIncrement ? 1 : -1;

            switch (tag)
            {
                case "ScoreOne":
                    mScoreOne += value;
                    break;

                case "ScoreTwo":
                    mScoreTwo += value;
                    break;
            }

            RpcUpdateScoreText();
        }
    }

    void Awake()
    {
        mInGameScore = GetComponentInChildren<TextMesh>();
    }

    void Update()
    {
        mInGameScore.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        mInGameScore.transform.position = transform.position + ScoreOffset;
    }

    [ClientRpc]
    private void RpcUpdateScoreText()
    {
        mInGameScore.text = "Score: " + TotalScore;
    }
}
