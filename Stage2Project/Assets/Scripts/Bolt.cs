using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Bolt : MonoBehaviour
{
    [SerializeField]
    private float TimeBetweenFlickers = 0.1f;

    [SerializeField]
    private int NumberOfSegments = 6;

    [SerializeField]
    private float VarianceAmplitude = 0.025f;

    private LineRenderer mLine;

    private float mNextFlicker;

    virtual protected void Awake()
    {
        mLine = GetComponent<LineRenderer>();
        RegenerateSegments();
    }

    virtual protected void Update()
    {
        mNextFlicker -= Time.deltaTime;
        if (mNextFlicker <= 0.0f)
        {
            RegenerateSegments();
            mNextFlicker = TimeBetweenFlickers;
        }
    }

    private void RegenerateSegments()
    {
        Vector3[] segments = new Vector3[NumberOfSegments];
        float step = 1.0f / (NumberOfSegments - 1);
        for (int i = 0; i < NumberOfSegments; i++)
        {
            segments[i] = new Vector3(
                Random.Range(-VarianceAmplitude, VarianceAmplitude),
                Random.Range(0.0f, VarianceAmplitude),
                i * step);
        }

        mLine.numPositions = NumberOfSegments;
        mLine.SetPositions(segments);
    }
}
