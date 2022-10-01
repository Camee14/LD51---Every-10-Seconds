using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Idol
{
    private readonly Director m_director;
    
    private GameObject m_idolObj;
    private Transform m_transform;
    private NavMeshAgent m_agent;
    private AudioBank m_bank;

    private bool m_stunLocked;
    private Coroutine m_stunLockBehaviour;
    
    public Idol(GameObject idolObj, Director director)
    {
        m_director = director;
        
        m_idolObj = idolObj;
        m_transform = idolObj.transform;
        m_agent = idolObj.GetComponent<NavMeshAgent>();
        m_bank = idolObj.GetComponent<AudioBank>();
    }

    public bool Move(Transform player, bool canMove, bool playerLooking)
    {
        if (playerLooking && canMove && !m_stunLocked)
            StartStunLock();

        if ((!playerLooking || !canMove) && m_stunLocked)
            m_stunLocked = false;
        
        if(m_stunLocked)
            return false;
                
        if(!canMove && m_agent.hasPath)
            m_agent.ResetPath();
        
        if(!canMove)
            return false;

        if (math.distancesq(player.position, m_transform.position) < 4f)
        {
            m_agent.ResetPath();
            return true;
        }

        m_agent.SetDestination(player.position);
        return false;
    }

    private void StartStunLock()
    {
        m_stunLocked = true;
        m_agent.ResetPath();
        
        if(m_stunLockBehaviour != null)
            m_director.StopCoroutine(m_stunLockBehaviour);
        
        m_stunLockBehaviour = m_director.StartCoroutine(StunLockState());
    }

    IEnumerator StunLockState()
    {
        if(!m_bank.Source.isPlaying)
            m_bank.Play(0);
        
        while (m_bank.Source.volume < 1)
        {
            m_bank.Source.volume += 5f * Time.deltaTime;
            yield return null;
        }
        
        Vector3 originalPos = m_transform.position;
        while (m_stunLocked)
        {
            if(!m_bank.Source.isPlaying)
                m_bank.Play(1);
            
            var jitter = Random.insideUnitCircle;
            m_transform.position = originalPos + new Vector3(jitter.x, 0, jitter.y) * Random.Range(0.01f, 0.1f);
            yield return null;
        }
        
        m_transform.position = originalPos;

        while (m_bank.Source.volume > 0)
        {
            m_bank.Source.volume -= 5f * Time.deltaTime;
            yield return null;
        }
    }
    
    public void StartKeyCeremony()
    {
        
    }

    public void StartWinCeremony()
    {
        m_director.StartCoroutine(WinCeremony());
    }

    IEnumerator WinCeremony()
    {
        Vector3 originalPos = m_transform.position;
        float timer = 0f;
        
        while (timer < 5f)
        {
            var jitter = Random.insideUnitCircle;
            m_transform.position = originalPos + new Vector3(jitter.x, 0, jitter.y) * Random.Range(0.01f, 0.1f);
            
            originalPos += Vector3.down * 0.5f * Time.deltaTime;
            
            yield return null;
            
            timer += Time.deltaTime;
        }
        
        m_idolObj.SetActive(false);
    }
}
