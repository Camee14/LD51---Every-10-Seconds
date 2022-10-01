using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Director : MonoBehaviour
{
    private enum GameState
    {
        PLAYING,
        LOST,
        WON
    }

    public GameObject m_idolObj;
    public GameObject m_playerObj;
    public GameObject m_bluePlinth, m_yellowPlinth, m_greenPlinth;
    public GameObject m_blueKey, m_yellowKey, m_greenKey;

    public GameObject m_ambientAudioObj;
    
    public GameObject m_uiDeathScreen;

    public float m_fogMax, m_fogMin;
    public float m_fogSpeed;
    public float m_fogDistanceModifier = 20f;

    public float m_playerMoveSpeed;

    public bool m_turnOffChasing;
    public float m_duration = 10f;

    private AudioBank m_audioBank;
    private AmbientAudioManager m_ambient;
    
    private Player m_player;
    private Idol m_idol;
    private Plinth m_plinthManager;

    private Billboard[] m_billboards;

    private float m_timer;
    private bool m_thickFog;
    private int m_lastKeysPlacedCount;

    private GameState m_gameState;
    
    // Start is called before the first frame update
    void Start()
    {
        m_player = new Player(m_playerObj);
        m_idol = new Idol(m_idolObj, this);

        m_ambient = new AmbientAudioManager(m_ambientAudioObj);

        m_audioBank = GetComponent<AudioBank>();
        
        Transform playerHold = m_player.GetHoldTransform();
        m_plinthManager = new Plinth(
            playerHold,
            m_bluePlinth, 
            m_yellowPlinth, 
            m_greenPlinth, 
            m_blueKey, 
            m_yellowKey, 
            m_greenKey
        );

        RenderSettings.fogDensity = m_fogMin;

        m_billboards = FindObjectsOfType<Billboard>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_gameState == GameState.LOST)
            return;

        foreach (Billboard billboard in m_billboards)
        {
            billboard.FacePlayer(m_playerObj.transform.position);
        }
        
        m_player.Look();
        m_player.Move(m_playerMoveSpeed);
        
        m_timer += Time.deltaTime;
        if (m_timer >= m_duration)
        {
            m_thickFog = !m_thickFog;
            StartCoroutine(TransitionFog());
            
            m_timer = 0f;
        }
        
        if(m_gameState == GameState.WON)
            return;
        m_ambient.UpdateAmbientSounds(m_playerObj.transform.position);

        bool playerLookingAtIdol = m_player.LookingAtIdol((1f - RenderSettings.fogDensity) * m_fogDistanceModifier);
        
        bool caught = m_idol.Move(m_playerObj.transform, !m_turnOffChasing && m_thickFog, playerLookingAtIdol);
        if (caught)
        {
            m_gameState = GameState.LOST;
            DoDeathCeremony();
        }
        
        m_plinthManager.UpdateKeys();
        m_plinthManager.UpdatePlinths();

        if (m_plinthManager.KeysPlaced != m_lastKeysPlacedCount)
        {
            if (m_plinthManager.KeysPlaced == 3)
            {
                //win the game
                m_gameState = GameState.WON;
                m_idol.StartWinCeremony();
            }
            else
            {
                //do ceremony
                m_idol.StartKeyCeremony();
            }

            m_lastKeysPlacedCount = m_plinthManager.KeysPlaced;
        }

        m_plinthManager.AnimateKeys();
    }

    private void DoDeathCeremony()
    {
        m_uiDeathScreen.SetActive(true);
        m_audioBank.Play(0);
    }

    IEnumerator TransitionFog()
    {
        float x = 0;
        do
        {
            float fog;
            if (m_thickFog)
            {
                fog = math.lerp(m_fogMin, m_fogMax, EaseUtilityFunctions.easeInOutQuad(x));
            }
            else
            {
                fog = math.lerp(m_fogMax, m_fogMin, EaseUtilityFunctions.easeInOutQuad(x));
            }
            x += m_fogSpeed * Time.deltaTime;
            RenderSettings.fogDensity = fog;
            
            yield return null;
        } while (x < 1f);
    }
}
