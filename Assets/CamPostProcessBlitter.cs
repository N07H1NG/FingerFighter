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
    Vector3 speed = Vector3.zero;
    Vector3 rotspeed = Vector3.zero;
    [SerializeField] Transform target;

    
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
        transform.position = Vector3.SmoothDamp(transform.position,target.position,ref speed,0.2f);
        transform.forward = Vector3.SmoothDamp(transform.forward,target.forward,ref rotspeed,0.2f);
        
        //tempSrc = RenderTexture.GetTemporary (src.width, src.height, src.depth, src.format);
        //Graphics.Blit(src,tempSrc,pp);
        
        Graphics.Blit(src,dest,ppLayer);
        //RenderTexture.ReleaseTemporary (tempSrc);
    }
   
    
}
