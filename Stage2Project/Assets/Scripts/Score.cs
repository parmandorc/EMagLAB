using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Score : NetworkBehaviour
{
    private int mScoreOne;
    private int mScoreTwo;

    public int TotalScore { get { return mScoreOne + mScoreTwo; } }

	public void IncrementScore(string tag) { UpdateScore(tag, true); }
    public void DecrementScore(string tag) { UpdateScore(tag, false); }

    [ServerCallback]
    private void UpdateScore(string tag, bool doIncrement)
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

        Debug.Log("Score (" + gameObject + "): " + TotalScore);
    }
}
