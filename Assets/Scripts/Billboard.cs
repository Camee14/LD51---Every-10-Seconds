using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool m_yOnly;
    private Transform m_cam;

    private void Start()
    {
        m_cam = Camera.main.transform;
    }

    public void FacePlayerCamera()
    {
        Vector3 camPos = m_cam.position;
        
        if (m_yOnly)
        {
            camPos.y = transform.position.y;
        }
        
        transform.forward = -(camPos - transform.position).normalized;
    }
}
