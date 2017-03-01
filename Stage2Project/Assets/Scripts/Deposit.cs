using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class handles the corner deposits of the game.
 * A player can earn points by putting the appropriate objects in their assigned corner of the level.
 * Units inside the deposit are not affected by the Vortex.
 */
public class Deposit : MonoBehaviour
{
    [SerializeField]
    private string AffectedTag;

    private Player mPlayer;
    private Score mScore;
    private List<GameObject> mObjects;
    private Renderer mRenderer;

    public Player Player { get { return mPlayer; } }

	void Start ()
    {
        mObjects = new List<GameObject>();
        mRenderer = gameObject.GetComponent<Renderer>();
        mRenderer.enabled = false;
	}

    void OnTriggerEnter(Collider other)
    {
        if (mPlayer != null)
        {
            if (other.gameObject.CompareTag(AffectedTag))
            {
                mObjects.Add(other.gameObject);

                // Update score
                if (mScore != null)
                {
                    mScore.IncrementScore(other.gameObject.tag);
                }

                // Update vortex dragging behaviour
                DraggedByVortex dragComponent = other.gameObject.GetComponent<DraggedByVortex>();
                if (dragComponent != null)
                {
                    dragComponent.SetEnabled(false);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (mPlayer != null)
        {
            if (other.gameObject.CompareTag(AffectedTag))
            {
                mObjects.Remove(other.gameObject);

                // Update score
                if (mScore != null)
                {
                    mScore.DecrementScore(other.gameObject.tag);
                }

                // Update vortex dragging behaviour
                DraggedByVortex dragComponent = other.gameObject.GetComponent<DraggedByVortex>();
                if (dragComponent != null)
                {
                    dragComponent.SetEnabled(true);
                }
            }
        }
    }

    // Sets the assigned player for this deposit. This is called by the GameManager.
    public void SetPlayer(Player player)
    {
        mPlayer = player;
        mScore = null;
        if (player != null)
        {
            mScore = player.GetComponent<Score>();
            mRenderer.enabled = true;

            // Set the color of the player
            Color newColor = player.PlayerColor;
            newColor.a = mRenderer.material.color.a;
            mRenderer.material.color = newColor;
        }
        else
        {
            mRenderer.enabled = false;
        }
    }
}
