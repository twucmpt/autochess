using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSlave : MonoBehaviour {
    AudioSource audioSource;
    public AudioClip sfx;

    // Start is called before the first frame update
    void Start() {
        Init();
    }

    public void Init()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(sfx);
    }

    void Update() 
    {
        if (!audioSource.isPlaying) Destroy(gameObject);
    }

}
