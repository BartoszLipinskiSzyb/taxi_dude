using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PassengerDrive : MonoBehaviour
{
    private GameObject car;
    private Drive carScipt;
    private SpriteRenderer sr;
    
    private GameObject goalMarker;

    private AudioManager am;
    public float minSitInDistance = 2.4f;
    public float minSitOutDistance = 2.4f;

    private BoxCollider2D bc2D;

    // Start is called before the first frame update
    void Start()
    {
        car = GameObject.Find("vehicles_yellow_taxi_0");
        carScipt = car.GetComponent<Drive>();
        sr = GetComponent<SpriteRenderer>();
        
        goalMarker = GameObject.Find("goalMarker");
        am = GameObject.Find("Main Camera").GetComponent<AudioManager>();

        bc2D = GetComponent<BoxCollider2D>();
    }

    public float moneyMultiplier = 1.0f;
    public float priceTag;
    // Update is called once per frame
    void Update()
    {
        if (GameManager.hasPassenger && Vector3.Distance(car.transform.position, new Vector3(GameManager.destination[1], GameManager.destination[0], 0)) < minSitOutDistance && carScipt.speed.magnitude == 0) {
            GameManager.hasPassenger = false;
            Debug.Log((int)Math.Floor(priceTag));
            GameManager.money += (int)Math.Floor(GameManager.currPriceTag);
            GameManager.infoText = "Thanks for the ride!";
            am.PlaySound(am.thanks);
            goalMarker.GetComponent<SpriteRenderer>().enabled = false;

            //Debug.Log("Destroying " + GameManager.start[1].ToString() + " " + GameManager.start[0].ToString());
            //GameManager.pendingPos.Remove(GameManager.start);

            for (int i = 0; i < GameManager.pendingPos.Count; ++i) {
                if (GameManager.pendingPos[i][1] == GameManager.start[1] && GameManager.pendingPos[i][0] == GameManager.start[0]) {
                    GameManager.pendingPos.RemoveAt(i);
                    break;
                }
            }

            UnityEngine.Object.Destroy(this.gameObject);
        }
        if (Vector3.Distance(transform.position, car.transform.position) < minSitInDistance && carScipt.speed.magnitude == 0 && sr.enabled) {
            if (GameManager.hasPassenger) {
                GameManager.infoText = "Finish the drive before taking another passenger";
            } else if (!GameManager.hasPassenger){
                GameManager.infoText = "Press E to take the passenger";
                if (Input.GetKey("e")){
                    // take passenger change later to change state of car and navigation
                    GameManager.infoText = "";
                    GameManager.hasPassenger = true;

                    int crossingId = UnityEngine.Random.Range(0, GameManager.crossings.GetLength(0));

                    GameManager.destination[0] = GameManager.crossings[crossingId,0] * (int)GameManager.mapToWorldSpace.y;
                    GameManager.destination[1] = GameManager.crossings[crossingId,1] * (int)GameManager.mapToWorldSpace.x;
                    Vector3 pos = new Vector3(GameManager.destination[1], GameManager.destination[0], 0);

                    GameManager.start[0] = (int)((transform.position.y + 1.5f) / -5);
                    GameManager.start[1] = (int)((transform.position.x + 1.5f) / 5);
                    Debug.Log(pos);

                    priceTag = moneyMultiplier * Vector3.Distance(car.transform.position, pos);
                    GameManager.currPriceTag = priceTag;

                    sr.enabled = false;
                    bc2D.enabled = false;

                    goalMarker.transform.position = pos + new Vector3(0.5f, 0.5f, 0.0f);
                    goalMarker.GetComponent<SpriteRenderer>().enabled = true;

                    am.PlaySound(am.hi);
                }
            }
        }
    }
}
