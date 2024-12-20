using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using System.Linq;

public class BublikiGameMode : MonoBehaviour
{
    AudioSource audioSource;
    AudioSource bell;
    [SerializeField] AudioClip shortbell, longbell,fireworkSFX;
    [SerializeField] MyAudioCue clockCue;

    //Dictionary<LittleGuy,ulong>
    public UnityEvent ActualStart;
    [SerializeField] GlobalScore scoreGlobal;

    public GameEvent fin;
    [SerializeField] GameObject fireworks;
    ulong[] players;
    public List<LittleGuy> guys = new List<LittleGuy>();
    List<BublikResult> results = new List<BublikResult>();
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        audioSource = GetComponents<AudioSource>()[0];
        bell = GetComponents<AudioSource>()[1];
        players = NetworkManager.Singleton.ConnectedClientsIds.ToArray();
        
    }
    // Start is called before the first frame update
    public IEnumerator Game(){
        yield return new WaitForSeconds(1f);
        for(int i=0;i<5;i++){
            audioSource.PlayOneShot(clockCue.GetRandomClip());
            yield return new WaitForSeconds(1f);
            
        }
        bell.clip = shortbell;
        bell.Play();
        yield return new WaitForSeconds(22f);
        ActualStart.Invoke();
        audioSource.volume = 0.2f;
        for(int i=0;i<12*60-5;i++){
            audioSource.PlayOneShot(clockCue.GetRandomClip());
            yield return new WaitForSeconds(1f);
        }
        audioSource.volume = 1f;
        for(int i=0;i<5;i++){
            audioSource.PlayOneShot(clockCue.GetRandomClip());
            yield return new WaitForSeconds(1f);
        }
        bell.clip = longbell;
        bell.Play();
        fin.Raise();

    }

    public void StartGame(){
        StartCoroutine(Game());
    }

    IEnumerator EndGameCount(){
        StartCoroutine(scoreGlobal.Rise());
        yield return new WaitUntil(scoreGlobal.InPlace);
        foreach(BublikResult res in results){
            if(!res.draw){
                int index = scoreGlobal.order.IndexOf(res.ID);
                res.guy.FlyToward(scoreGlobal.text[index].transform.position,scoreGlobal,res.ID,false);
            }
            else{
                
                res.guy.FlyToward(scoreGlobal.transform.position,scoreGlobal,res.ID,true);
            }
        }
    }

    public void ReceiveResult(BublikResult res)
    {
        results.Add(res);
        if (results.Count == guys.Count){
            StartCoroutine(EndGameCount());
        }
    }

    public void RegisterGuy(LittleGuy guy){
        guys.Add(guy);
    }

    public void CountingDone(){
        Instantiate(fireworks);
    }

    IEnumerator Fireworks(){
        audioSource.clip =fireworkSFX;
        audioSource.loop = true;
        audioSource.Play();
        while (true){
            Instantiate(fireworks,Random.insideUnitSphere*100f+Vector3.up*200f,Quaternion.identity);
            yield return new WaitForSeconds(10f);
        }
    }
}
