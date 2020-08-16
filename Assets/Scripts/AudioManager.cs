using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // References
    public AudioSource source;
    public AudioSource slowSource;
    public AudioSource clickSource;
    public AudioClip click;

    // State
    private bool shouldFade;
    public float fadeAmount = 0.005f;
    public float fadeAmountDelta = 0.001f;

    private void FixedUpdate() {
        if (shouldFade && fadeAmount > 0) {
            source.volume -= fadeAmount;
            fadeAmount += fadeAmountDelta;
            if (fadeAmount < 0) fadeAmount = 0.002f;
            if (source.volume < 0.06) source.volume = 0.06f;
        }
    }

    private void Start() {
        Debug.Log("Playing audio...");
        source.Play();
    }

    public void FadeOutSource() {
        shouldFade = true;
    }

    public void PlayClick() {
        Debug.Log("Playing click...");
        clickSource.PlayOneShot(click, 1);
    }
}
