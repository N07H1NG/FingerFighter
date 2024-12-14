using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SplitScreenAudioSource : MonoBehaviour
{
    float vol;
    AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        
        audioSrc = GetComponent<AudioSource>();
        vol = audioSrc.volume;
    }

    // Update is called once per frame
    void Update()
    {
        audioSrc.volume = vol*(1-math.clamp((transform.position-SplitScreenAudioSrcManager.singleton.average_location).magnitude/audioSrc.maxDistance,0,1));
    }
}
