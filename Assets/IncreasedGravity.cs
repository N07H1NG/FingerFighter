using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedGravity : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.useGravity) rb.AddForce(Physics.gravity*32f*rb.mass);
    }
}
