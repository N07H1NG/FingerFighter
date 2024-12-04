using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CamPostProcessBlitter : MonoBehaviour
{
    public Material pp;

    Camera cam;


    
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
        
        
        
        
        Graphics.Blit(src,dest,pp);
        
    }
   
    
}
