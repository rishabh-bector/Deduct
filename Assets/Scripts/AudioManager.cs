using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // References
    public AudioSource source;
    public AudioSource slowSource;
    public AudioClip first;

    // State
    private bool shouldFade;
    public float fadeAmount = 0.005f;
    public float fadeAmountDelta = 0.001f;

    private void FixedUpdate() {
        if (shouldFade && fadeAmount > 0) source.volume -= fadeAmount;
        fadeAmount += fadeAmountDelta;
    }

    private void Start() {
        Debug.Log("Playing audio...");
        source.PlayOneShot(first);
    }

    public void FadeOutSource() {
        shouldFade = true;
    }

    public void StartSlow() {
        slowSource.PlayOneShot(first);
    }
}
