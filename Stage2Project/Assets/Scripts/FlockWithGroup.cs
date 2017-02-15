using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlockWithGroup : MonoBehaviour
{
    [SerializeField]
    private GroupTag.Group GroupCode;

    [SerializeField]
    private float Speed;

    [SerializeField]
    private float BuddyDistance = 100.0f;

    [SerializeField]
    private float AvoidDistance = 1.0f;

    [SerializeField]
    private float CheckForBuddiesInterval = 10.0f;

    private List<GroupTag> mCurrentBuddies;
    private Rigidbody mBody;
    private float mCountDownToCheck;

    void Awake()
    {
        mCurrentBuddies = new List<GroupTag>();
        mBody = GetComponent<Rigidbody>();
        mCountDownToCheck = 0.0f;
    }

    void Update()
    {
        if (GameManager.GameState != GameManager.State.Playing) return;

        mCountDownToCheck -= Time.deltaTime;
        if (mCountDownToCheck <= 0.0f)
        {
            UpdateBuddyList();
            mCountDownToCheck = CheckForBuddiesInterval;
        }

        FlockWithBuddies();
    }

    private void UpdateBuddyList()
    {
        GroupTag[] individuals = FindObjectsOfType<GroupTag>();

        for (int count = 0; count < individuals.Length; ++count)
        {
            if (individuals[count].gameObject != gameObject && individuals[count].Affiliation == GroupCode )
            {
                Vector3 difference = individuals[count].transform.position - transform.position;
                if (difference.magnitude <= BuddyDistance)
                {
                    if (!mCurrentBuddies.Contains(individuals[count]))
                    {
                        mCurrentBuddies.Add(individuals[count]);
                    }
                }
                else if (mCurrentBuddies.Contains(individuals[count]))
                {
                    mCurrentBuddies.Remove(individuals[count]);
                }
            }
        }
    }

    /* For gameplay reasons, the flocking behaviour is limited to a simplified version of it, where boids only
     *  avoid each other so the group does not collapse. For the designed gameplay, neither alignment nor
     *  cohesion are needed (or wanted).
     */ 
    private void FlockWithBuddies()
    {
        if (mCurrentBuddies.Count > 0)
        {
            Vector3 avoid = Vector3.zero;
            uint avoidCount = 0; // The separation component must be divided by just the number of boids that affect said component.

            for (int count = 0; count < mCurrentBuddies.Count; ++count)
            {
                Rigidbody body = mCurrentBuddies[count].GetComponent<Rigidbody>();
                if ( ( mCurrentBuddies[count].transform.position - transform.position ).magnitude < AvoidDistance)
                {
                    avoid += mCurrentBuddies[count].transform.position;
                    avoidCount++;
                }
            }

            avoid /= avoidCount;

            avoid = transform.position - avoid;
            avoid.Normalize();

            mBody.AddForce(avoid * Speed * Time.deltaTime);
        }
    }
}
