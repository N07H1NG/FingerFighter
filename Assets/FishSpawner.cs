using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class FishSpawner : MonoBehaviour
{
    AudioSource drone;
    AudioSource thump;
    [SerializeField] GameObject fishPerfab;
    [SerializeField] float speed;
    [SerializeField] float frequency;
    [SerializeField] float area;
    [SerializeField] float howfaraway;
    public MyAudioCue thumpCue;
    // Start is called before the first frame update

    void Start(){
        drone = GetComponents<AudioSource>()[0];
        thump = GetComponents<AudioSource>()[1];
    }
    void OnEnable(){
        StartCoroutine(SpawnFish());
    }
    void OnDisable(){
        StopAllCoroutines();
    }

    IEnumerator SpawnFish(){
        
        while(true){
            GetComponent<AudioSource>().Play();
            Vector3 dir = Quaternion.Euler(0,Random.Range(0f,360),0)*Vector3.forward;
            transform.forward = dir;
            float timer = 0f;
            float spawnTimer = 0f;
            transform.position = dir*(-howfaraway)+Vector3.up*100f;
            
            while (timer<howfaraway*2/speed){
                yield return null;
                transform.position += dir*Time.deltaTime*speed;
                transform.localScale = Vector3.one*(1f-math.clamp((2f*math.abs(0.5f-timer/(howfaraway*2/speed))-(area/howfaraway))/(1-area/howfaraway),0,1));
                timer += Time.deltaTime;
                spawnTimer += Time.deltaTime;
                if (spawnTimer>=60f/frequency && new Vector2(transform.position.x,transform.position.z).magnitude<area){
                    GameObject newfish = Instantiate(fishPerfab,transform.position,Random.rotation);
                    thump.PlayOneShot(thumpCue.GetRandomClip());
                    spawnTimer = 0f;
                }
            }
            GetComponent<AudioSource>().Stop();
            yield return new WaitForSeconds(Random.Range(8f,30f));
        }
    }
}
