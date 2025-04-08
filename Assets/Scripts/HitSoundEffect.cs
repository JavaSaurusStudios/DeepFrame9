using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HitSoundEffect : MonoBehaviour
{
    public AudioClip[] bumpSound;
    private AudioSource soundSource;
    // Start is called before the first frame update
    void Start()
    {
        soundSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        if (soundSource.isPlaying) return;
        soundSource.pitch = Random.Range(.9f, 1.1f);
        soundSource.PlayOneShot(bumpSound[Random.Range(0, bumpSound.Length)]);
    }
}
