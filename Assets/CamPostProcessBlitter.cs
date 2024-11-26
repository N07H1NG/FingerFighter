using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CamPostProcessBlitter : MonoBehaviour
{
    public Material pp;

    public Material ppLayer;
    public float aboba;

    RenderTexture tempSrc;
    
    void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }

    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //tempSrc = RenderTexture.GetTemporary (src.width, src.height, src.depth, src.format);
        //Graphics.Blit(src,tempSrc,pp);
        Graphics.Blit(src,dest,ppLayer);
        //RenderTexture.ReleaseTemporary (tempSrc);
    }
   
    
}
