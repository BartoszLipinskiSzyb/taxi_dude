using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Drive : MonoBehaviour
{
    public float maxSpeed = 2.0f;
    public int maxVehicleHealth = 100;
    public Vector3 speed = Vector3.zero;
    public float minTurningSpeed = 0.1f;

    public int damageOnGrassProbability = 1000;
    public float grassSlowDown = 0.3f;

    public float shopCollisionDamage = 1.0f;
    public float personCollisionDamage = 0.5f;

    public GameObject nuke;

    private Vector3[] directions = {
        new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(0, -1, 0), new Vector3(-1, 0, 0) // CSS order
    };
    public int startingDirection = 1;

    public int currentDirection;
    public Vector3 currentAcceleration;
    public float fuelConsumption;

    private SpriteRenderer spriteRenderer;
    public Sprite sideSprite;
    public Sprite backSprite;
    public Sprite frontSprite;

    private BoxCollider2D bc2D;

    private AudioManager am;

    // Start is called before the first frame update
    void Start()
    {
        currentDirection = startingDirection;
        spriteRenderer = GetComponent<SpriteRenderer>();
        bc2D = GetComponent<BoxCollider2D>();
        am = GameObject.Find("Main Camera").GetComponent<AudioManager>();
    }

    public float lastDirectionChange = 0.0f;
    public float directionChangeCooldown = 0.2f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.nuked) {
            return;
        }

        if (GameManager.fuel > 0) {
            speed = directions[currentDirection] * Input.GetAxis("Vertical") * maxSpeed * GameManager.globalTimescale;
        } else {
            nukeEm();
        }

        Vector3 mapPos = Vector3.Scale(transform.position, GameManager.worldSpaceToMap);
        if (GameManager.map[(int)Math.Round(mapPos.y), (int)Math.Round(mapPos.x)] == ' ') {
            speed *= grassSlowDown;
            if(UnityEngine.Random.Range(0, damageOnGrassProbability) == 0){
                GameManager.vehicleHealth -= 1;
            }
        }

        transform.Translate(speed * Time.deltaTime);

        GameManager.fuel -= (speed.magnitude * fuelConsumption);

        if (speed.magnitude >= minTurningSpeed) {
            if (Time.time - directionChangeCooldown > lastDirectionChange)
            {
                // Debug.Log("Mo�na wciska�");
                if(Input.GetAxisRaw("Horizontal") != 0.0f)
                {
                    currentDirection += (int)Input.GetAxisRaw("Horizontal") * Math.Sign(Input.GetAxis("Vertical"));
                    lastDirectionChange = Time.time;
                }
            }
            if (currentDirection < 0) currentDirection += 4;
            currentDirection %= 4;

            switch (currentDirection)
            {
                case 0:
                    spriteRenderer.sprite = backSprite;
                    transform.localScale = Vector3.one;
                    /* bc2D.transform.rotation = Quaternion */
                    break;
                case 1:
                    spriteRenderer.sprite = sideSprite;
                    transform.localScale = Vector3.one;
                    break;
                case 2:
                    spriteRenderer.sprite = frontSprite;
                    transform.localScale = Vector3.one; 
                    break;
                case 3:
                    spriteRenderer.sprite = sideSprite;
                    transform.localScale = new Vector3(-1, 1, 1);
                    break;
            }

            // resize BoxCollider2D to fit sprite
            Vector3 v = GetComponent<Renderer>().bounds.size; 
            bc2D.size = v;
        }
    }

    public int vehicleHealth = 100;
    void OnCollisionEnter2D(Collision2D collision){
        double impact = Math.Pow(Math.Max((speed.magnitude - maxSpeed/2), 0), 2);
        Debug.Log(impact);

        if (collision.gameObject.tag == "shop") {
            GameManager.vehicleHealth -= (int)Math.Floor(impact * shopCollisionDamage);
            am.PlaySoundImmediate(am.carHit);
        } else if (collision.gameObject.tag == "person") {
            GameManager.vehicleHealth -= (int)Math.Floor(impact * personCollisionDamage);
        }

        if (collision.gameObject.tag == "water") {
            Debug.Log("pod wodą");
            am.PlaySoundImmediate(am.carUnderWater);
            spriteRenderer.enabled = false;
            maxSpeed = 0;

            Invoke("nukeEm", 3);
        }

        if (GameManager.vehicleHealth <= 0){
            nukeEm();
        }

    }

    private void nukeEm(){
        am.audioSrc.Stop();
        am.queue.Clear();
        am.PlaySoundImmediate(am.explosion);
        GameObject.Find("vehicles_yellow_taxi_0").GetComponent<AudioSource>().Stop();
        UnityEngine.Object.Destroy(GameObject.Find("NPCs"));
        GameManager.nuked = true;
        Instantiate(nuke, transform.position, Quaternion.identity, transform);
        spriteRenderer.enabled = false;
        maxSpeed = 0;
        GameManager.fuel = 0;
        GameManager.vehicleHealth = 0;
        GameManager.money = 0;
        am.PlaySound(am.nuked);
    }
}
