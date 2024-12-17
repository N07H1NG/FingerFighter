using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

public class Fish : MonoBehaviour
{
    
    Rigidbody rb;
    bool grounded;
    Light light;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        transform.localScale += new Vector3(Random.Range(0f,3f),Random.Range(0f,1f),Random.Range(0f,3f));
        transform.localScale *= Random.Range(0.6f,1.1f);
        Color c = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);
        
        GetComponentInChildren<SkinnedMeshRenderer>().material.SetColor("_EmissionColor",c*1.3f);
        
        //GetComponentInChildren<SkinnedMeshRenderer>().material.color = c;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(JumpSilly());
        StartCoroutine(Flicker());
    }

    // Update is called once per frame
    

    IEnumerator JumpSilly(){
        while(true){
            yield return new WaitForSeconds(Random.Range(0.2f,3f));
            if (grounded){
                Vector3 f = 6*Vector3.up+2*Random.onUnitSphere;
                f*= Random.Range(1,3);
                rb.AddForce(f,ForceMode.Impulse);
            }
            
        }
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if(other.collider.gameObject.CompareTag("Terrain")){
            grounded = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if(other.collider.gameObject.CompareTag("Terrain")){
           grounded = false;
        }
    }

    IEnumerator Flicker(){
        float timer = 0;
        while(true){
            timer += Time.deltaTime;
            light.intensity = 3+math.sin(timer);
            yield return null;
            
            
        }
    }
}
