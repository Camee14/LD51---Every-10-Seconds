using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Idol
{
    private GameObject m_idolObj;
    private IdolMono m_mono;
    private Transform m_transform;
    private Transform m_meshTransform;
    private NavMeshAgent m_agent;
    
    private AudioBank m_bank;
    private AudioSource m_hissAudio;
    private AudioSource m_chaseAudio;
    private AudioSource m_deathAudio;
    
    private bool m_lookStunLocked;
    private bool m_keyStunLocked;
    private Coroutine m_stunLockBehaviour;

    private int m_threatLevel = 0;
    
    private bool m_canTeleport;
    private float m_teleportTimer;
    private float m_nextTeleport;
    
    public Idol(GameObject idolObj)
    {
        m_idolObj = idolObj;
        m_transform = idolObj.transform;
        m_mono = idolObj.GetComponent<IdolMono>();
        m_agent = idolObj.GetComponent<NavMeshAgent>();
        m_bank = idolObj.GetComponent<AudioBank>();

        m_meshTransform = m_transform.Find("Mesh");
        
        m_chaseAudio = m_transform.Find("ChaseAudioSrc").GetComponent<AudioSource>();
        m_hissAudio = m_transform.Find("HissAudioSrc").GetComponent<AudioSource>();
        m_deathAudio = m_transform.Find("DeathAudioSrc").GetComponent<AudioSource>();

        m_nextTeleport = Random.Range(m_mono.m_teleporTimerMin, m_mono.m_teleporTimerMax);
    }

    public void IncreaseThreat(int level)
    {
        m_threatLevel = level;
        
        m_agent.speed = m_mono.m_normalSpeed * (m_threatLevel > 0 ? m_mono.m_speedMultiplier : 1f);

        m_canTeleport = m_threatLevel > 1;
    }

    public bool Move(Transform player, bool canMove, bool playerLooking)
    {
        float dist = math.distance(player.position, m_transform.position);
        m_chaseAudio.volume = 1f - dist / (10f * 1.5f);

        if (m_mono.m_turnOffChasing)
            return false;
        
        if(m_keyStunLocked)
            return false;
        
        if (playerLooking && canMove && !m_lookStunLocked)
            StartLookStunLock();

        if ((!playerLooking || !canMove) && m_lookStunLocked)
            m_lookStunLocked = false;

        if(!canMove && m_agent.hasPath)
            m_agent.ResetPath();

        if (m_canTeleport)
            m_teleportTimer += Time.deltaTime;

        if (!canMove)
        {
            if (m_canTeleport)
                Teleport(player.position, dist);
            
            return false;
        }

        if (dist < 2f)
        {
            if(m_stunLockBehaviour != null)
                m_mono.StopCoroutine(m_stunLockBehaviour);
            
            m_chaseAudio.Stop();
            m_hissAudio.Stop();

            m_agent.ResetPath();
            return true;
        }

        m_agent.SetDestination(player.position);
        return false;
    }

    private void Teleport(Vector3 playerPos, float distToPlayer)
    {
        if(m_teleportTimer < m_nextTeleport || distToPlayer < 15f)
            return;
        
        if(Physics.Raycast(m_transform.position, playerPos - m_transform.position, out RaycastHit hit))
            if(hit.transform.CompareTag("Player"))
                return;
        
        m_teleportTimer = 0f;
        m_nextTeleport = Random.Range(m_mono.m_teleporTimerMin, m_mono.m_teleporTimerMax);
        
        Vector2 offset = Random.insideUnitCircle * 25f;
        Vector3 tpPos = playerPos + new Vector3(offset.x, 0, offset.y);
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(tpPos, out navHit, 10.0f, NavMesh.AllAreas))
        {
            tpPos = navHit.position;
        }

        m_agent.Warp(tpPos);
    }

    private void StartLookStunLock()
    {
        m_lookStunLocked = true;
        
        if(m_stunLockBehaviour != null)
            m_mono.StopCoroutine(m_stunLockBehaviour);
        
        m_stunLockBehaviour = m_mono.StartCoroutine(StunLockState());
    }

    IEnumerator StunLockState()
    {
        if(!m_hissAudio.isPlaying)
            m_hissAudio.PlayOneShot(m_bank.Clips[0]);
        
        m_agent.speed = m_mono.m_stunnedSpeed * (m_threatLevel > 0 ? m_mono.m_speedMultiplier : 1f);
        
        while (m_hissAudio.volume < 1)
        {
            m_hissAudio.volume += 5f * Time.deltaTime;
            yield return null;
        }
        
        Vector3 originalPos = m_meshTransform.localPosition;
        while (m_lookStunLocked)
        {
            if(!m_hissAudio.isPlaying)
                m_hissAudio.PlayOneShot(m_bank.Clips[1]);
            
            var jitter = Random.insideUnitCircle;
            m_meshTransform.localPosition = originalPos + new Vector3(jitter.x, 0, jitter.y) * Random.Range(0.01f, 0.1f);
            yield return null;
        }
        
        m_meshTransform.localPosition = originalPos;

        while (m_hissAudio.volume > 0)
        {
            m_hissAudio.volume -= 5f * Time.deltaTime;
            yield return null;
        }
        
        m_agent.speed = m_mono.m_normalSpeed * (m_threatLevel > 0 ? m_mono.m_speedMultiplier : 1f);
    }
    
    public void StartKeyCeremony()
    {
        m_agent.ResetPath();
        
        if(m_stunLockBehaviour != null)
            m_mono.StopCoroutine(m_stunLockBehaviour);
        
        m_stunLockBehaviour = m_mono.StartCoroutine(KeyStunLockState());
    }
    
    IEnumerator KeyStunLockState()
    {
        m_keyStunLocked = true;
        
        if(!m_hissAudio.isPlaying)
            m_hissAudio.PlayOneShot(m_bank.Clips[0]);
        
        while (m_hissAudio.volume < 1)
        {
            m_hissAudio.volume += 5f * Time.deltaTime;
            yield return null;
        }
        
        Vector3 originalPos = m_transform.position;
        float timer = 0;
        
        while (timer < 3f)
        {
            if(!m_hissAudio.isPlaying)
                m_hissAudio.PlayOneShot(m_bank.Clips[1]);
            
            var jitter = Random.insideUnitCircle;
            m_transform.position = originalPos + new Vector3(jitter.x, 0, jitter.y) * Random.Range(0.01f, 0.1f);
            
            yield return null;
            
            timer += Time.deltaTime;
        }
        
        m_transform.position = originalPos;

        while (m_hissAudio.volume > 0)
        {
            m_hissAudio.volume -= 5f * Time.deltaTime;
            yield return null;
        }

        m_keyStunLocked = false;
    }

    public void StartWinCeremony(Vector3 deathPos)
    {
        m_agent.ResetPath();
        
        if(m_stunLockBehaviour != null)
            m_mono.StopCoroutine(m_stunLockBehaviour);

        m_transform.position = deathPos;
        
        m_mono.StartCoroutine(WinCeremony());
    }

    IEnumerator WinCeremony()
    {
        if(!m_hissAudio.isPlaying)
            m_hissAudio.PlayOneShot(m_bank.Clips[0]);
        
        m_deathAudio.PlayOneShot(m_bank.Clips[3]);
        
        while (m_hissAudio.volume < 1)
        {
            m_hissAudio.volume += 5f * Time.deltaTime;
            yield return null;
        }

        Vector3 originalPos = m_transform.position;
        float timer = 0f;
        
        while (timer < 10f)
        {
            if(!m_hissAudio.isPlaying)
                m_hissAudio.PlayOneShot(m_bank.Clips[1]);
            
            var jitter = Random.insideUnitCircle;
            m_transform.position = originalPos + new Vector3(jitter.x, 0, jitter.y) * Random.Range(0.01f, 0.1f);
            
            originalPos += Vector3.down * 0.5f * Time.deltaTime;
            
            if(m_hissAudio.volume > 0)
                m_hissAudio.volume -= 0.13f * Time.deltaTime;
            yield return null;
            
            timer += Time.deltaTime;
        }
        
        m_idolObj.SetActive(false);
    }
}
