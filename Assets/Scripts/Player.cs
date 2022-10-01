using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Player
{
    private GameObject m_playerObj;
    private Transform m_transform;
    private CharacterController m_charController;

    private Vector3 m_p1, m_p2;
    private RaycastHit[] m_moveHits;

    public Transform GetHoldTransform() => m_transform.GetChild(0);
    
    public Player(GameObject playerObj)
    {
        m_playerObj = playerObj;
        m_transform = playerObj.transform;
        m_charController = playerObj.GetComponent<CharacterController>();
        
        m_p1 = m_transform.position + m_charController.center + Vector3.up * -m_charController.height * 0.5F;
        m_p2 = m_p1 + Vector3.up * m_charController.height;
        
        m_moveHits = new RaycastHit[1];
    }

    public void Move(float movespeed)
    {
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        moveDir = m_transform.TransformDirection(moveDir) * movespeed;
        
        // int count = Physics.CapsuleCastNonAlloc(m_p1,m_p2, m_charController.radius, moveDir.normalized, m_moveHits, 5f, LayerMask.GetMask("Scenery"), QueryTriggerInteraction.Ignore);
        // if (count > 0)
        // {
        //     foreach (RaycastHit hit in m_moveHits)
        //     {
        //         if(hit.transform.tag.Equals("Ground") || hit.transform.tag.Equals("Player"))
        //             continue;
        //         
        //         if(math.lengthsq(moveDir) > hit.distance * hit.distance)
        //             moveDir = Vector3.zero;
        //     }
        // }
        
        m_charController.SimpleMove(moveDir);
    }

    public void Look()
    {
        float horizontal = Input.GetAxis("Mouse X");
        horizontal *= 5f;
        m_transform.rotation *= Quaternion.Euler(0, horizontal, 0);
    }

    public bool LookingAtIdol(float maxDistance)
    {
        Ray r = new Ray(m_transform.TransformPoint(m_charController.center), m_transform.forward);
        return Physics.SphereCast(r, 0.25f , maxDistance, LayerMask.GetMask("Idol"));
    }
}
