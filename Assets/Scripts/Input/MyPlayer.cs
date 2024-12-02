using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using Touch = NetworkTouchData;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class MyPlayer : NetworkBehaviour

{
    
    Vector2 minmax = Vector2.zero;
    float time_since_last_step=0f;
    float last_step_time=0f;
    bool calibrated = false;
    float screenScaler;
    Vector2 dir = new Vector2(0,1);
    bool[] down = new bool[2];
    bool[] downdelay = new bool[2];
    Vector2[] pos = new Vector2[2];
    [SerializeField] Transform[] foot;
    [SerializeField] Transform[] footTargets;
    Vector3[] footSpeeds = {Vector3.zero,Vector3.zero};
    Vector3[] footRots = {Vector3.zero,Vector3.zero};
    Vector3[]steps = {Vector3.forward,Vector3.forward};
    Vector3[]stepstarts = {Vector3.zero,Vector3.forward};
    [SerializeField] Transform head;

    Vector3 cameraCatchPos = Vector3.zero;
    Vector3 cameraCatchLook = Vector3.zero;
    Animator popickAnimator;
    Vector3 targetForward;
    Vector3 headSpeed =  Vector3.zero;
    bool lift = true;
    int main = 0;
    int headTouch;

    int last_step=0;
    float just_stepped=0;
    float just_upped=0;
    Vector3 HeadPos = new Vector3(0,0,4f);
    Vector3 HeadTarget;
    
    bool headControlled = false;
    Dictionary<int,int> footTouches = new Dictionary<int, int>();

    public Transform Cam;
    
    AudioSource audioSrc;
    public MyAudioCue cue;

    public List<Touch> networkActiveTouches = new List<Touch>();

    List<Coroutine> liftCoroutines = new List<Coroutine>();

    bool unhandledTouches;
    float unhandledTouchDelta = 0;
    float unhandledTouchDeltaBuffer = 0;
    //public List<Touch> thisFrameTouches;

    void Awake()
    {

        //EnhancedTouchSupport.Enable();
        //screenScaler = EnhancedTouch.Screen;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        //EnhancedTouchSupport.Enable();
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        //EnhancedTouchSupport.Disable();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner) EnhancedTouchSupport.Enable();
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        EnhancedTouchSupport.Disable();
        base.OnNetworkDespawn();
    }

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        popickAnimator = GetComponentInChildren<Animator>();
        HeadTarget = HeadPos;
        
        for(int i=0;i<2;i++){
            pos[i] = Vector2.zero;
            down[i] = false;
            downdelay[i] = false;
        }
        targetForward = Vector3.Cross(Vector3.up,foot[0].position-foot[1].position);
        StartCoroutine(Calibration());
    }
    void Update()
    {
        popickAnimator.SetBool("suck",headControlled);
        unhandledTouchDelta+=Time.deltaTime;
        if (calibrated){
            time_since_last_step+=Time.deltaTime;
            bool[] truedown = {down[0]||downdelay[0],down[1],downdelay[1]};
            Vector3 center = (foot[0].position*(truedown[0]?5f:1f)+foot[1].position*(truedown[1]?5f:1f))/((truedown[0]?5f:1f)+(truedown[1]?5f:1f));
            center = (4f*center + foot[last_step].position*(truedown[last_step]?just_stepped:0f))/(4f+(truedown[last_step]?just_stepped:0f));
            if (time_since_last_step<last_step_time/3){
                center.y-=just_stepped/2f;
                
            
            }
            else{
                center.y+=just_upped/2f;
                
            }
            if (just_stepped>0){
                just_stepped = math.max(just_stepped-7f*Time.deltaTime,0f);
            }
            if (just_upped>0){
                just_upped = math.max(just_upped-7f*Time.deltaTime,0f);
            }
            
            center.y+=3.2f+((truedown[0]?0f:0.8f)+(truedown[1]?0f:0.8f))- (foot[0].position-foot[1].position).magnitude/6f;
            //center += steps[0]/15f+steps[1]/15f;

            targetForward = -1*(Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up,dir),Vector3.up)*(foot[0].position-foot[1].position));

            transform.forward =  Vector3.SmoothDamp(transform.forward,targetForward.normalized,ref cameraCatchLook,0.5f);

            
            cameraCatchPos.y *= (1f+2f*Time.deltaTime*just_stepped*math.clamp(0.5f/last_step_time,1f,3f));   
            transform.position = Vector3.SmoothDamp(transform.position,center+transform.forward.normalized,ref cameraCatchPos,0.25f);

            head.localPosition = Vector3.SmoothDamp(head.localPosition,HeadTarget,ref headSpeed,0.2f);
            head.up = head.position-transform.position;
            //head.forward = transform.rotation*HeadTarget;
            Cam.forward = transform.forward+2*head.up-Vector3.up;

            

            MoveHead();

            for(int i = 0;i<2;i++){
                Vector3 temptarget = truedown[i]?foot[i].position:transform.position+(foot[i].position-transform.position)/2.5f+Vector3.down/2f;
                footTargets[i].position = Vector3.SmoothDamp(footTargets[i].position,temptarget,ref footSpeeds[i],0.15f);
                Vector3 attempted_dir =(footTargets[i].position-center).normalized+steps[i]+2*transform.forward;
                attempted_dir.y=0;
                attempted_dir.Normalize();
                
                attempted_dir = (Vector3.Dot(attempted_dir,transform.forward)>=-0.3f)?attempted_dir:attempted_dir*-1f;
                footTargets[i].forward = Vector3.SmoothDamp(footTargets[i].forward,attempted_dir,ref footRots[i],0.3f);
            }
        }
    }



    public void SingleFrameOfTouches(List<Touch> l){
        networkActiveTouches = l;
        unhandledTouches = true;
        foreach(Touch touch in l){
                HandleTouch(touch);
        }
        
    }

    void HandleTouch(Touch touch){
        if ((touch.screenPosition.x>=Screen.width/2 && (touch.touchId != headTouch||!headControlled) )|| footTouches.Keys.Contains(touch.touchId))
        {
            if (footTouches.Keys.Contains(touch.touchId)){
                Finger(footTouches[touch.touchId],touch);
                if (touch.phase == TouchPhase.Ended|| touch.phase == TouchPhase.Canceled){
                    
                    footTouches.Remove(touch.touchId);
                }
            }
            else if(footTouches.Count<2 && touch.phase == TouchPhase.Began){
                
                
                footTouches.Add(touch.touchId,main);
                
                Finger(footTouches[touch.touchId],touch);
                
            }
            else{
                Debug.Log("Excess touches");
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
                if (touch.phase == TouchPhase.Ended|| touch.phase == TouchPhase.Canceled){
                    
                    headControlled = false;
                    HeadTarget = HeadPos;
                }
            }
        }
    }
    




    void Finger(int f, Touch t){
    
        Vector2 newdir = (pos[1-f]-t.screenPosition)*MathF.Pow(-1,f);
        
        float turn = Vector2.SignedAngle(dir,newdir);
        
        Quaternion turnQ = Quaternion.AngleAxis(-1*turn, Vector3.up);
        if(t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled){
            stepstarts[f] = foot[f].position;
            down[f] = false;
            just_upped = 3f;
            if (!down[1-f]){
                main=1-f;
                
                liftCoroutines.Add(StartCoroutine(liftDelay(f)));

            }
            else{
                main = f;
                
            }
            
        }else if(t.phase == TouchPhase.Began){
            audioSrc.PlayOneShot(cue.GetRandomClip());
            down[f] = true;
            if (f==0){
                popickAnimator.SetTrigger("step_l");
            }
            else{
                popickAnimator.SetTrigger("step_r");
            }
            main = 1-f;
            if(down[1-f] || downdelay[1-f]){
                just_stepped = 3f;
                last_step_time=time_since_last_step;
                time_since_last_step = 0f;
                
                if(!lift){
                    foot[f].position = foot[1-f].position + turnQ*(foot[f].position - foot[1-f].position).normalized*ScaleScreenDistance(newdir.magnitude);
                    steps[f] = foot[f].position-stepstarts[f];
                    dir = newdir;
                    last_step = f;
                }
                else{
                    print("liftend");
                    foot[f].position = foot[1-f].position + (stepstarts[f] - foot[1-f].position).normalized*ScaleScreenDistance(newdir.magnitude);
                    steps[f] = foot[f].position-stepstarts[f];
                    dir = newdir;
                    lift = false;
                }
            }
            else{
                foot[f].position = stepstarts[f];
                
            }
            foreach(Coroutine c in liftCoroutines){
                StopCoroutine(c);
            }
            liftCoroutines.Clear();
            downdelay[0] = false;
            downdelay[1] = false;
            
        }else{
            if(down[1-f]){

                dir = newdir;
            }
            else{
                Vector2 estimatedpos = pos[1-f]+(pos[f]-t.screenPosition);
                Vector2 estimated_dir = (estimatedpos-t.screenPosition)*MathF.Pow(-1,f);
                float estimatedturn = Vector2.SignedAngle(dir,estimated_dir);
                Quaternion estimatedturnQ = Quaternion.AngleAxis(-1*estimatedturn, Vector3.up);
                foot[1-f].position = foot[f].position + estimatedturnQ*(foot[1-f].position - foot[f].position).normalized*ScaleScreenDistance(estimated_dir.magnitude);
                steps[1-f] = foot[1-f].position-stepstarts[1-f];
                dir = estimated_dir;
                pos[1-f] = estimatedpos;
            }
        }
        
        pos[f] = t.screenPosition;
        
    }

    IEnumerator liftDelay(int f){
        
        downdelay[f] = true;
        yield return new WaitForSeconds(0.1f);
        
        downdelay[f] = false;
        if (!(down[0] || down[1])){
            print("LIFT");
            lift = true;
        }
    }


    void HeadAttack(Touch touch){
        
        Vector2 d = 50f*touch.delta/minmax[0];
        HeadTarget = Quaternion.AngleAxis(-1f*d.y,Vector3.right)*HeadTarget;
        HeadTarget = Quaternion.AngleAxis(d.x,Vector3.up)*HeadTarget;
    }


    void MoveHead(){
        HeadTarget = HeadTarget.normalized*Math.Clamp(HeadTarget.magnitude+10f*Time.deltaTime*(headControlled?1f:0f),0,8.5f);
        HeadTarget.y = Math.Clamp(HeadTarget.y,-5.5f,8.5f);
        
    }

    IEnumerator Calibration(){
        bool down =false;
        int count = 0;
        float calibration_timer = 0f;
        while(calibration_timer<2f||count<3||down||minmax[0]>=minmax[1]/2f){
            //print(calibration_timer);
            if ((networkActiveTouches.Count)==2){
                //print("TwoTouches");
                if (!down){
                    down = true;
                    count +=1;
                }
                
                
                calibration_timer+=Time.deltaTime;
                float d =(networkActiveTouches[0].screenPosition - networkActiveTouches[1].screenPosition).magnitude;
                if (d<minmax[0]||minmax[0]==0){
                    minmax[0] = d;
                }
                if (d>minmax[1]||minmax[1]==0){
                    minmax[1] = d;
                }
            }
            else{
                down = false;
            }
            yield return new WaitUntil(HasUnhandledTouches);
        }    
        calibrated = true;
    }

    float ScaleScreenDistance(float d){
        float p = math.clamp((d-minmax[0])/(minmax[1]-minmax[0]),0f,1f);
        
        return math.lerp(0.8f,9f,p);
    }

    bool HasUnhandledTouches(){
        if (unhandledTouches){
            unhandledTouches = false;
            return true;
        }
        return false;
    }
}
