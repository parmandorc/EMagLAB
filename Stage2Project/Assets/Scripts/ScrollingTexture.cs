using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ScrollingTexture : MonoBehaviour
{
    [SerializeField]
    private float ScrollingSpeed = 1.0f;

    private Renderer mRenderer;

	void Start ()
    {
        mRenderer = gameObject.GetComponent<Renderer>();
        mRenderer.material.mainTextureOffset = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }
	
	void Update ()
    {
        Vector2 currentOffset = mRenderer.material.mainTextureOffset;
        mRenderer.material.mainTextureOffset = new Vector2(currentOffset.x, currentOffset.y + ScrollingSpeed * Time.deltaTime);
    }
}
