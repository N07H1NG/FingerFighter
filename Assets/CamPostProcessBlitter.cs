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
    Camera cam;
    Matrix4x4 cToV = Matrix4x4.identity;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.DepthNormals;
    }

    /// <summary>
    /// OnPreRender is called before a camera starts rendering the scene.
    /// </summary>
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        
        
        //tempSrc = RenderTexture.GetTemporary (src.width, src.height, src.depth, src.format);
        //Graphics.Blit(src,tempSrc,pp);
        
        Graphics.Blit(src,dest,ppLayer);
        //RenderTexture.ReleaseTemporary (tempSrc);
    }
   
    
}
