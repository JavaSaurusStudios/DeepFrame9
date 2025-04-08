using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class PowerupSoundEffect : MonoBehaviour
{
    public AudioClip chargeSound;
    public AudioClip releaseSound;
    private AudioSource soundSource;
    private bool isStopped = true;
    // Start is called before the first frame update
    void Start()
    {
        soundSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        if (soundSource.isPlaying) return;
        soundSource.clip = chargeSound;
        soundSource.pitch = Random.Range(.9f, 1.1f);
        soundSource.Play();
        soundSource.loop = true;
        isStopped = false;
    }

    public void Stop()
    {
        if (isStopped) return;
        if (soundSource.isPlaying)
        {
            isStopped = true;
            soundSource.Stop();
            soundSource.pitch = Random.Range(.9f, 1.1f);
            soundSource.PlayOneShot(releaseSound);
        }
    }

}