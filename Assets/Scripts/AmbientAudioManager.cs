using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class AmbientAudioManager
{
    private AudioSource[] m_sources;
    private AudioClip[] m_clips;
    
    private float m_timer;
    private float m_timeToNextSound;

    private float m_maxTimer = 30f, m_minTimer = 9f;
    
    public AmbientAudioManager(GameObject parentObj)
    {
        m_sources = new AudioSource[parentObj.transform.childCount];
        m_clips = parentObj.GetComponent<AudioBank>().Clips;
        
        int index = 0;
        foreach (Transform child in parentObj.transform)
        {
            m_sources[index] = child.GetComponent<AudioSource>();
            index++;
        }

        m_timeToNextSound = Random.Range(m_minTimer, m_maxTimer);
    }

    public void UpdateAmbientSounds(Vector3 playerPos)
    {
        if (m_timer < m_timeToNextSound)
        {
            m_timer += Time.deltaTime;
            return;
        }
        
        m_timer = 0;
        m_timeToNextSound = Random.Range(m_minTimer, m_maxTimer);

        float dist = 0f;
        AudioSource furthest = m_sources[0];
        foreach (AudioSource source in m_sources)
        {
            if (math.distancesq(playerPos, source.transform.position) > dist)
                furthest = source;
        }
        
        furthest.PlayOneShot(m_clips[Random.Range(0, m_clips.Length)]);
    }
}
