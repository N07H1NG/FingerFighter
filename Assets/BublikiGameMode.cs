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
    [SerializeField] AudioClip shortbell, longbell;
    [SerializeField] MyAudioCue clockCue;

    //Dictionary<LittleGuy,ulong>
    public UnityEvent ActualStart;

    ulong[] players;
    List<LittleGuy> guys;
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
        yield return new WaitForSeconds(30f);
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

    }

    public void StartGame(){
        StartCoroutine(Game());
    }

    public void EndGame(){

    }

    public void ReceiveResult(BublikResult res)
    {
        results.Add(res);
        if (results.Count == guys.Count){
            EndGame();
        }
    }

    public void RegisterGuy(LittleGuy guy){
        guys.Add(guy);
    }
}
