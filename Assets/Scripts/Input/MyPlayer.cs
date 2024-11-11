using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class MyPlayer : MonoBehaviour 

{
    Vector2 dir = new Vector2(0,1);
    bool[] down = new bool[2];
    bool[] downdelay = new bool[2];
    Vector2[] pos = new Vector2[2];
    [SerializeField] Transform[] foot;
    [SerializeField] Transform head;

    Vector3 cameraCatchPos = Vector3.zero;
    Vector3 cameraCatchLook = Vector3.zero;
    Vector3 targetForward;
    Vector3 headSpeed =  Vector3.zero;
    bool lift = true;
    int main = 0;
    int headTouch;
    Vector3 HeadPos = new Vector3(0,0,9f);
    Vector3 HeadTarget;
    
    bool headControlled = false;
    Dictionary<int,int> footTouches = new Dictionary<int, int>();

    Transform Cam;

    void Awake()
    {

        EnhancedTouchSupport.Enable();
    }
    void Start()
    {
        HeadTarget = HeadPos;
        Cam = transform.GetChild(0);
        for(int i=0;i<2;i++){
            pos[i] = Vector2.zero;
            down[i] = false;
            downdelay[i] = false;
        }
        targetForward = Vector3.Cross(Vector3.up,foot[0].position-foot[1].position);
    }
    void Update()
    {
        Vector3 center = (foot[0].position+foot[1].position)/2f;
        center.y+=3f;
        transform.position = Vector3.SmoothDamp(transform.position,center,ref cameraCatchPos,0.3f);
        
        targetForward = -1*(Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up,dir),Vector3.up)*(foot[0].position-foot[1].position));
        
        transform.forward =  Vector3.SmoothDamp(transform.forward,targetForward.normalized,ref cameraCatchLook,0.5f);

        head.localPosition = Vector3.SmoothDamp(head.localPosition,HeadTarget,ref headSpeed,0.2f);
        //head.forward = transform.rotation*HeadTarget;
        
        foreach(Touch touch in Touch.activeTouches){
            HandleTouch(touch);
        }

        MoveHead();
    }

    void HandleTouch(Touch touch){
        if (touch.screenPosition.x>=Screen.width/2 || footTouches.Keys.Contains(touch.touchId))
        {
            if (footTouches.Keys.Contains(touch.touchId)){
                Finger(footTouches[touch.touchId],touch);
                if (touch.phase == TouchPhase.Ended){
                    footTouches.Remove(touch.touchId);
                }
            }
            else if(footTouches.Count<2 && touch.phase == TouchPhase.Began){
                
                footTouches.Add(touch.touchId,main);
                
                Finger(footTouches[touch.touchId],touch);
                
            }
            else{
                print("losing");
            }
        }
        else{
            if ((touch.touchId == headTouch) || !headControlled){
                headTouch = touch.touchId;
                if (touch.phase == TouchPhase.Began){
                    HeadTarget = HeadPos;
                }
                headControlled = true;
                HeadAttack(touch);
                if (touch.phase == TouchPhase.Ended){
                    headControlled = false;
                    HeadTarget = HeadPos;
                }
            }
        }
    }
    




    void Finger(int f, Touch t){
    
        Vector2 newdir = (pos[1-f]-t.screenPosition)*MathF.Pow(-1,f);
        
        float turn = Vector2.SignedAngle(dir,newdir);
        //print(dir+ " "+newdir+" "+turn);
        Quaternion turnQ = Quaternion.AngleAxis(-1*turn, Vector3.up);
        if(t.phase == TouchPhase.Ended){
            down[f] = false;
            if (!down[1-f]){
                main=1-f;
                StartCoroutine(liftDelay(f));

            }
            else{
                main = f;
                
            }
            
        }else if(t.phase == TouchPhase.Began){
            down[f] = true;
            main = 1-f;
            if(down[1-f] || downdelay[1-f]){
                if(!lift){
                    foot[f].position = foot[1-f].position + turnQ*(foot[f].position - foot[1-f].position).normalized*newdir.magnitude/70;
                    dir = newdir;
                }
                else{
                    foot[f].position = foot[1-f].position + (foot[f].position - foot[1-f].position).normalized*newdir.magnitude/70;
                    dir = newdir;
                    lift = false;
                }
            }
            else{
                
                
            }
            StopAllCoroutines();
            downdelay[0] = false;
            downdelay[1] = false;
            
        }else{
            if(down[1-f]){

                dir = newdir;
            }
        }
        
        pos[f] = t.screenPosition;
        
    }

    IEnumerator liftDelay(int f){
        print("delay started");
        downdelay[f] = true;
        yield return new WaitForSeconds(0.1f);
        print("delay_ended");
        downdelay[f] = false;
        if (!(down[0] || down[1])){
            print("lift" + down[0] + " " + down[1]);
            
            lift = true;
        }
    }


    void HeadAttack(Touch touch){
        HeadTarget = Quaternion.AngleAxis(-1f*touch.delta.y/4f,Vector3.right)*HeadTarget;
        HeadTarget = Quaternion.AngleAxis(touch.delta.x/4f,Vector3.up)*HeadTarget;
    }


    void MoveHead(){
        HeadTarget = HeadTarget.normalized*Math.Clamp(HeadTarget.magnitude+20f*Time.deltaTime*(headControlled?1f:0f),0,15f);
    }
}
