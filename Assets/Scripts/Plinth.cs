using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Plinth
{
    public enum Colour
    {
        BLUE,
        YELLOW,
        GREEN
    }

    private InteractableRangeMono[] m_plinths;
    private InteractableRangeMono[] m_keys;
    private AudioSource[] m_plinthAudioSrc;
    private AudioBank[] m_banks;
    
    private Transform m_playerHold;
    
    private float[] m_animOffsets;

    private int m_keysPlaced = 0;
    private int m_carriedKeyIndex = -1;

    public int KeysPlaced => m_keysPlaced;
    
    public Plinth(Transform playerHold, GameObject bluePlinth, GameObject yellowPlinth, GameObject greenPlinth, GameObject blueKey, GameObject yellowKey, GameObject greenKey)
    {
        m_playerHold = playerHold;
        
        m_plinths = new [] {
            bluePlinth.GetComponent<InteractableRangeMono>(), 
            yellowPlinth.GetComponent<InteractableRangeMono>(), 
            greenPlinth.GetComponent<InteractableRangeMono>()
        };
        m_plinthAudioSrc = new [] {
            bluePlinth.GetComponent<AudioSource>(), 
            yellowPlinth.GetComponent<AudioSource>(), 
            greenPlinth.GetComponent<AudioSource>()
        };
        m_banks = new [] {
            bluePlinth.GetComponent<AudioBank>(), 
            yellowPlinth.GetComponent<AudioBank>(), 
            greenPlinth.GetComponent<AudioBank>()
        };
        m_keys = new[]
        {
            blueKey.GetComponent<InteractableRangeMono>(), 
            yellowKey.GetComponent<InteractableRangeMono>(), 
            greenKey.GetComponent<InteractableRangeMono>()
        };

        m_animOffsets = new[] { Random.Range(0f, 10f), Random.Range(0f, 10f), Random.Range(0f, 10f) };
    }

    public void UpdatePlinths()
    {
        if (m_carriedKeyIndex == -1)
            return;

        for (int i = 0; i < m_plinths.Length; i++)
        {
            if (m_plinths[i].m_interactable && m_carriedKeyIndex == i)
            {
                m_carriedKeyIndex = -1;
                MoveKeyToPlinth(i);
                m_keysPlaced++;
                
                m_plinthAudioSrc[i].PlayOneShot(m_banks[i].Clips[0]);
                
                m_plinths[i].StopInteractions();
                m_keys[i].StopInteractions();
            }
        }
    }

    public void UpdateKeys()
    {
        if(m_carriedKeyIndex != -1)
            return;
        
        for(int i = 0; i < m_keys.Length; i++)
        {
            if (m_keys[i].m_interactable)
            {
                m_carriedKeyIndex = i;
                MoveKeyToHoldingPos(i);
                return;
            }
        }
    }

    private void MoveKeyToHoldingPos(int index)
    {
        m_keys[index].transform.SetParent(m_playerHold);
        m_keys[index].transform.position = m_playerHold.position;
    }

    private void MoveKeyToPlinth(int index)
    {
        m_keys[index].transform.SetParent(null);
        m_keys[index].transform.position = m_plinths[index].transform.position + Vector3.up * 2f;
    }

    public void AnimateKeys()
    {
        int index = 0;
        foreach (InteractableRangeMono key in m_keys)
        {
            if(index == m_carriedKeyIndex)
                continue;
            key.transform.position += new Vector3(0, 0.33f * math.sin(m_animOffsets[index] + Time.realtimeSinceStartup * 1.45f), 0) * Time.deltaTime;
            index++;
        }
    }

}
