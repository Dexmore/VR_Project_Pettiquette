using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    // Object Pooling=> SFXData Àç»ý
    private AudioSource audioSource;
    private AudioManager manager;
    private Coroutine coroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Init(AudioManager audioManager)
    {
        manager = audioManager;
    }

    public void Play(AudioClip clip, float volume, Vector3 pos)
    {
        transform.position = pos;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f;
        audioSource.Play();

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(ReturnAfter(clip.length));
    }
    
    private IEnumerator ReturnAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        manager.ReturnSFXPlayer(this);
    }
}
