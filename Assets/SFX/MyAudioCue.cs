using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MyAudioCue : ScriptableObject
{
    [SerializeField] AudioClip[] containClips;

    public AudioClip GetRandomClip(){
        int l = containClips.Length;
        return containClips[Random.Range(0,l-1)];
    }
}
