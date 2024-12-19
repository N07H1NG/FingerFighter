using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

public class LittleGuy : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    Vector3 lastdir = Vector3.zero;
    RaycastHit downhit = new RaycastHit();
    [SerializeField] BublikiGameMode gameMode;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        agent.updateUpAxis = false;
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(Roam());
        gameMode.RegisterGuy(this);
        
        startPos = transform.position;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Physics.Raycast(transform.position,Vector3.down,out downhit);
        transform.rotation = Quaternion.FromToRotation(transform.up,downhit.normal)*transform.rotation;        
      
    }

    IEnumerator Roam(){
        int i = 0;
        while(true){
            print("COROUTINE BABY");
            ChooseTarget(i);
            yield return null;
            animator.SetBool("Running",true);
           
            yield return new WaitUntil(IsPathComplete);
            animator.SetBool("Running",false);
            if (i<3){
                yield return null;
            }
            else{
                yield return new WaitForSeconds(Random.Range(10f,20f));
                
            }
            i = (i+1)%5;
        }
    }

    bool IsPathComplete(){
        return agent.remainingDistance<=agent.stoppingDistance;
    }

    void ChooseTarget(int farther){
        NavMeshHit hit = new NavMeshHit();
        bool found = false;
        int i =0;
        while(!found){
            float a = Random.Range(0f,360f);
            Vector3 trg = transform.position+30f*new Vector3(math.cos(a),0,math.sin(a))+lastdir*5f*farther;
            found = NavMesh.SamplePosition(trg,out hit, 25f,NavMesh.AllAreas);
            i++;
            if (i>100){
                found = true;
                print("NotFound");
            }
            
        }
        lastdir = (hit.position-transform.position).normalized;
        agent.SetDestination(hit.position);
    }

    IEnumerator Finish(){
        NavMeshHit hit = new NavMeshHit();
        GetComponentInChildren<Pond>().GetResults(out ulong fav,out int scr, out bool draw);
        
        agent.ResetPath();
        NavMesh.SamplePosition(startPos,out hit, 15f,NavMesh.AllAreas);
        agent.SetDestination(hit.position);
        animator.SetBool("Running",true);

        float ang = 0f;
        float speed = agent.speed;
        float angspeed = 12f/speed;
        bool sent = false;
        while(true){
            speed += Time.deltaTime;
            agent.speed = speed;
            agent.stoppingDistance = 0f;
            NavMesh.SamplePosition(Quaternion.AngleAxis(ang,Vector3.up)*startPos,out hit, 25f,NavMesh.AllAreas);
            agent.SetDestination(hit.position);
            ang += Time.deltaTime*angspeed*speed;
            if(!sent && (transform.position-startPos).magnitude <20f){
                gameMode.ReceiveResult(new BublikResult(this,fav,scr,draw));
                sent = true;
            }
            
            yield return null;
        }
    }

    public void Fin(){
        StopAllCoroutines();
        StartCoroutine(Finish());
    }

    public void FlyToward(Vector3 pos,GlobalScore scr,ulong id,bool dr){
        StopAllCoroutines();
        agent.enabled = false;
        StartCoroutine(Fly(pos,scr,id,dr));
    }

    IEnumerator Fly(Vector3 trg,GlobalScore scr,ulong id,bool dr){
        Vector3 speed = Vector3.zero;
        while((transform.position - trg).magnitude >10f){
            transform.position = Vector3.SmoothDamp(transform.position,trg,ref speed,4f);
            
            transform.rotation = Quaternion.AngleAxis(Time.deltaTime*90f,Vector3.up)*transform.rotation;
            yield return null;
        }
        if(!dr){
            scr.score[id]+=1;
            scr.text[scr.order.IndexOf(id)].text = scr.score[id].ToString();
        }
        scr.done+=1;
        if(scr.done == gameMode.guys.Count){
            gameMode.CountingDone();
        }
        Destroy(gameObject);
    }


}
