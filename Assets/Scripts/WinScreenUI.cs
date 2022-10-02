using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class WinScreenUI : MonoBehaviour
{
    public TMP_Text m_text;
    public Button m_exit, m_retry;
        
    private Image m_exitBtnImage;
    private TMP_Text m_exitBtnText;
    
    private Image m_retryBtnImage;
    private TMP_Text m_retryBtnText;
    
    public float m_initialPause;
    public float m_revealDuration;
    private float m_timer;
    
    void Start()
    {
        m_exitBtnImage = m_exit.GetComponent<Image>();
        m_exitBtnText = m_exit.transform.GetChild(0).GetComponent<TMP_Text>();
        
        m_retryBtnImage = m_retry.GetComponent<Image>();
        m_retryBtnText = m_retry.transform.GetChild(0).GetComponent<TMP_Text>();
    }
    
    public void OnExitPressed()
    {
        Application.Quit();
    }

    public void OnRetryPressed()
    {
        SceneManager.LoadScene(0);
    }
    
    public void StartReveal()
    {
        StartCoroutine(RevealState());
    }

    IEnumerator RevealState()
    {
        yield return new WaitForSeconds(m_initialPause);
        
        Color textStart = m_text.color;
        Color exitTextStart = m_exitBtnText.color;
        Color retryTextStart = m_retryBtnText.color;
        Color exitImageStart = m_exitBtnImage.color;
        Color retryImageStart = m_retryBtnImage.color;
        
        Color textEnd = m_text.color + Color.black;
        Color exitTextEnd = m_exitBtnText.color + Color.black;
        Color retryTextEnd = m_retryBtnText.color + Color.black;
        Color exitImageEnd = m_exitBtnImage.color + Color.black;
        Color retryImageEnd = m_retryBtnImage.color + Color.black;
        
        while (m_timer < m_revealDuration)
        {
            float t = math.clamp(m_timer / m_revealDuration, 0f, 1f);
            
            t = EaseUtilityFunctions.easeInQuart(t);

            m_text.color = Color.Lerp(textStart, textEnd, t);
            m_exitBtnText.color = Color.Lerp(exitTextStart, exitTextEnd, t);
            m_retryBtnText.color = Color.Lerp(retryTextStart, retryTextEnd, t);
            m_exitBtnImage.color = Color.Lerp(exitImageStart, exitImageEnd, t);
            m_retryBtnImage.color = Color.Lerp(retryImageStart, retryImageEnd, t);
            
            yield return null;
            
            m_timer += Time.deltaTime;
        }
        
        Cursor.visible = true;
        m_exit.interactable = m_retry.interactable = true;
    }
}
