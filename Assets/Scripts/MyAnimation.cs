using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MyAnimation : MonoBehaviour
{
    public Sprite[] sprites;

    private SpriteRenderer sr;
    public int frameUpdate = 60;
    public bool loop = true;
    private int spawnFrame;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        spawnFrame = Time.frameCount;
    }

    // Update is called once per frame
    void Update()
    {
        int spriteId = (int)Math.Floor((double)((Time.frameCount - spawnFrame) / frameUpdate));

        if(loop) {
             spriteId %= sprites.GetLength(0);
        }

        if (spriteId < sprites.GetLength(0)){
            sr.sprite = sprites[spriteId];
        } else {
            UnityEngine.Object.Destroy(this);
        }
    }
}
