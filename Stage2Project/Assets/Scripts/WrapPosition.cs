using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WrapPosition : NetworkBehaviour
{	
	void Update ()
    {
        /* This should only execute on the authority, since if not, a client without authority might incorrectly wrap
         *  this object's position, which will be then overriden by their correct position (if the authority didnt wrap),
         *  hence causing the former to see the object sweep to their final position.
         */ 
        if (!hasAuthority) return;

        Vector3 position = transform.position;

        if (position.x < Arena.Width * -0.5f)
        {
            position.x += Arena.Width;
        }
        else if (position.x > Arena.Width * 0.5f)
        {
            position.x -= Arena.Width;
        }

        if (position.z < Arena.Height * -0.5f)
        {
            position.z += Arena.Height;
        }
        else if (position.z > Arena.Height * 0.5f)
        {
            position.z -= Arena.Height;
        }

        transform.position = position;
    }
}
