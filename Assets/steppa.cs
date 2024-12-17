using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class steppa : MonoBehaviour
{
    [SerializeField] MyAudioCue cue;
    AudioSource src;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        src = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    public void Step(){
        src.PlayOneShot(cue.GetRandomClip());
    }
}
