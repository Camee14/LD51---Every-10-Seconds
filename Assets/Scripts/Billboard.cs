using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    
    public void FacePlayer(Vector3 playerPos)
    {
        transform.forward = (playerPos - transform.position).normalized;
    }
}
