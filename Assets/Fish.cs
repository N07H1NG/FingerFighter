using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Fish : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(JumpSilly());
    }

    // Update is called once per frame
    

    IEnumerator JumpSilly(){
        while(true){
            yield return new WaitForSeconds(Random.Range(2f,15f));
            rb.AddForce(30*Vector3.up+10*Random.onUnitSphere);
        }
    }
}
