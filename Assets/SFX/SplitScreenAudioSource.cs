using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SplitScreenAudioSource : MonoBehaviour
{
    Dictionary<AudioSource,float> vol = new Dictionary<AudioSource, float>();
    AudioSource[] audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        
        audioSrc = GetComponents<AudioSource>();
        foreach (AudioSource srcComp in audioSrc){
            vol[srcComp] = srcComp.volume;
            srcComp.volume = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (SplitScreenAudioSrcManager.singleton != null)
        {
            SplitScreenAudioSrcManager src = SplitScreenAudioSrcManager.singleton;
            if(src.locations.Length>=1){
                float res = (src.locations[0]-transform.position).magnitude;
                for(int i=1;i<src.locations.Length;i++){
                    res = math.min((src.locations[i]-transform.position).magnitude,res);
                }
                foreach(AudioSource srcComp in audioSrc){
                    srcComp.volume = vol[srcComp]*(1-math.clamp(res/srcComp.maxDistance,0,1));
                }
                
            }
        }
        
    }
}
