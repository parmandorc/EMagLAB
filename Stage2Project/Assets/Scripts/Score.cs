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

    // The Server must have authority on the Scores, so only the server can update the score values.
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
    }
}
