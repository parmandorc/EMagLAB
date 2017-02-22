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
    private float RotatingSpeed = 100.0f;

    [SerializeField]
    private GameObject Dome;

    [SerializeField]
    private GameObject DomeBolts;

    public float DragForce { get { return Force; } }

    private float[] mBoltsRescalings;
    private Projector mProjector;
    private ParticleSystem mParticleEmitter;

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

        mProjector = gameObject.GetComponentInChildren<Projector>();
        mParticleEmitter = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        mParticleEmitter.transform.Rotate(0.0f, RotatingSpeed * Time.deltaTime, 0.0f, Space.World);
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
        mProjector.gameObject.SetActive(true);
        mParticleEmitter.gameObject.SetActive(true);

        // Rescale dome bolts out
        mBoltsRescalings = new float[DomeBolts.transform.childCount];
        for (int i = 0; i < mBoltsRescalings.Length; i++)
        {
            mBoltsRescalings[i] = Random.Range(0.0f, 1.0f);
            DomeBolts.transform.GetChild(i).localScale += new Vector3(0.0f, 0.0f, mBoltsRescalings[i]);
        }
    }

    void OnDisable()
    {
        Dome.SetActive(true);
        mProjector.gameObject.SetActive(false);
        mParticleEmitter.gameObject.SetActive(false);

        // Rescale dome bolts back in
        if (mBoltsRescalings != null)
        {
            for (int i = 0; i < mBoltsRescalings.Length; i++)
            {
                DomeBolts.transform.GetChild(i).localScale -= new Vector3(0.0f, 0.0f, mBoltsRescalings[i]);
            }
        }
    }
}
