using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Screenshooter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("SADas");
            ScreenCapture.CaptureScreenshot("Screenshotik"+Hash128.Compute(Time.deltaTime)+".png");
        }
    }
}
