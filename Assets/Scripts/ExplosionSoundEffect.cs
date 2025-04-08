using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExplosionSoundEffect : MonoBehaviour
{
    public AudioClip[] explosions;
    private AudioSource explosionSource;
    // Start is called before the first frame update
    void Start()
    {
        explosionSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        ScreenShake.INSTANCE.TriggerShake();
        explosionSource = GetComponent<AudioSource>();
        explosionSource.pitch = Random.Range(.9f, 1.1f);
        explosionSource.PlayOneShot(explosions[Random.Range(0, explosions.Length)]);
    }

}
