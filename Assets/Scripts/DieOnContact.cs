using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DieOnContact : MonoBehaviour
{
    public Sprite blood1;
    public Sprite blood2;

    private MyAnimation ma;
    private AudioManager am;
    private BoxCollider2D bc2D;

    public void Start(){
        ma = GetComponent<MyAnimation>();
        am = GameObject.Find("Main Camera").GetComponent<AudioManager>();
        bc2D = GetComponent<BoxCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision) 
    { 
        if (collision.gameObject.CompareTag("car"))
        {
            ma.sprites[0] = blood1;
            ma.sprites[1] = blood2;
            
            bc2D.enabled = false;

            am.PlaySoundImmediate(am.splash);
            am.PlaySound(am.onKill);
        }
    }
}
