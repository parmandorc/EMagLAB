using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Score : NetworkBehaviour
{
    [SerializeField]
    private Vector3 ScoreOffset;

    [SyncVar(hook = "HookScoreOne")]
    private int mScoreOne;

    [SyncVar(hook = "HookScoreTwo")]
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

    private void HookScoreOne(int score)
    {
        mScoreOne = score;
        UpdateScoreText();
    }

    private void HookScoreTwo(int score)
    {
        mScoreTwo = score;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        mInGameScore.text = "Score: " + TotalScore;
    }
}
