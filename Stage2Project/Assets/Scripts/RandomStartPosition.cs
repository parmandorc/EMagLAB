using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RandomStartPosition : NetworkBehaviour
{
    [SerializeField]
    private float innerRadius;

    [SerializeField]
    private float outterRadius;

    /* This should only execute on the Server's Start, since if not it will cause several weird behaviours.
     * If it runs on the client, whenever said client connects to the game, all existing objects with this component will
     *  calculate a random position, which will then be overridden by the server's position, causing all the objects
     *  in the client's screen to sweep to their final position.
     * If it runs on the authority, the previous problem is solved. However, whenever a player joins the game,
     *  the host (server) sees the player for a moment at (0,0), and then the player's objects is swept to their final
     *  position (after the new client's Start is executed and their random position calculated and transmitted across the network).
     */
    public override void OnStartServer()
    {
        float angle = Random.Range(0.0f, Mathf.PI * 2);
        float radius = Random.Range(innerRadius, outterRadius);
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;
        transform.position = new Vector3(x, 0.5f, z);
    }
}
