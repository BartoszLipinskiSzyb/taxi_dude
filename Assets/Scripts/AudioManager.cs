using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] hi;
    public AudioClip[] thanks;
    public AudioClip[] annoyYou;
    public AudioClip[] randomFunny;
    public AudioClip[] hmm;
    public AudioClip[] whereIsThat;
    public AudioClip[] workshopEnter;
    public AudioClip[] iWantToBuy;
    public AudioClip[] nuked;
    public AudioClip[] onSave;
    public AudioClip[] onKill;
    public AudioClip[] arrested;
    public AudioClip[] noCash;
    public AudioClip[] onRepair;
    public AudioClip[] start;
    public AudioClip[] onBreak;
    public AudioClip[] onExit;

    public AudioClip[] habibHello;
    public AudioClip[] habibNoMoney;
    public AudioClip[] price;
    public AudioClip[] habibThankYou;
    public AudioClip[] habibGiveMoney;

    public AudioClip[] mapOpen;

    public AudioClip splash;
    public AudioClip explosion;
    public AudioClip carHit;
    public AudioClip carUnderWater;

    public AudioClip fancyCinematic;

    public AudioSource audioSrc;

    public int probability;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        PlaySound(start);
    }

    public List<AudioClip> queue = new List<AudioClip>();
    private AudioClip current;
    // Update is called once per frame
    void Update()
    {
        if (!audioSrc.isPlaying && queue.Count > 0) {
            current = queue[0];
            audioSrc.clip = queue[0];
            audioSrc.Play();
            Debug.Log(current);
            queue.RemoveAt(0);
        }
    }

    public void PlaySound(AudioClip[] ac) { 
        // prevent two similar questions next to each other
        if (queue.Count > 0) {
            for (int i = 0; i < ac.GetLength(0); ++i){
                if (ac[i] == queue[queue.Count - 1] || ac[i] == current) {
                    return;
                }
            }
        }

        AudioClip clip = ac[Random.Range(0, ac.GetLength(0))];
        queue.Add(clip);
    }

    public void PlaySoundImmediate(AudioClip ac) {
        audioSrc.PlayOneShot(ac);
    }
}
