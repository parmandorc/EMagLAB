using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* This class serves as a central point of information about the vortex.
 * However, it is easier that each object handles their own dragging behaviour with the DraggedByVortex component,
 *  since if not, the Vortex object would have to get references to all draggable objects (what if a new object
 *  is spawned after the vortex has been activated?).
 */ 
public class Vortex : NetworkBehaviour
{
    [SerializeField]
    private float Force = 1000.0f;

    [SerializeField]
    private GameObject Dome;

    [SerializeField]
    private GameObject DomeBolts;

    public float DragForce { get { return Force; } }

    private float[] mBoltsRescalings;

    void Awake()
    {
        if (Dome == null)
        {
            Dome = GameObject.Find("Dome");
        }

        if (DomeBolts == null)
        {
            DomeBolts = GameObject.Find("DomeBolts");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isServer & enabled)
        {
            Destroy(other.gameObject);
        }
    }

    void OnEnable()
    {        
        Dome.SetActive(false);

        // Rescale dome bolts out
        mBoltsRescalings = new float[DomeBolts.transform.childCount];
        for (int i = 0; i < mBoltsRescalings.Length; i++)
        {
            mBoltsRescalings[i] = Random.Range(0.0f, 1.0f);
            DomeBolts.transform.GetChild(i).localScale += new Vector3(0.0f, 0.0f, mBoltsRescalings[i]);
        }

        // Activate all child gameobjects
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        Dome.SetActive(true);

        // Rescale dome bolts back in
        if (mBoltsRescalings != null)
        {
            for (int i = 0; i < mBoltsRescalings.Length; i++)
            {
                DomeBolts.transform.GetChild(i).localScale -= new Vector3(0.0f, 0.0f, mBoltsRescalings[i]);
            }
        }

        // Deactivate all child gameobjects
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
