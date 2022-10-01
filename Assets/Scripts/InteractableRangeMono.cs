using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableRangeMono : MonoBehaviour
{
    [NonSerialized] public bool m_interactable = false;
    [NonSerialized] public bool m_interactionsEnabled = true;

    public void StopInteractions()
    {
        m_interactable = false;
        m_interactionsEnabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!m_interactionsEnabled)
            return;
        
        if(!other.CompareTag("Player"))
            return;

        m_interactable = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Player"))
            return;

        m_interactable = false;
    }
}
