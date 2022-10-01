using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBank : MonoBehaviour
{
    public AudioClip[] Clips;
    public AudioSource Source;

    public void Play(int clip)
    {
        Source.PlayOneShot(Clips[clip]);
    }
}
