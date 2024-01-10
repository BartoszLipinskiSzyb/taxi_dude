using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIUpdater : MonoBehaviour
{
    public GameObject canvas;
    public Text money;
    public Text vehicleHealt;
    public Text infoText;
    public Image navArrow;
    public Text fuel;
    public int redHealthFrom = 40;

    public string welcomeMessage;

    private AudioManager am;

    void Start()
    {
        GameManager.infoText = welcomeMessage;
        InvokeRepeating("clearText", 0.0f, 2.0f);
        am = GameObject.Find("Main Camera").GetComponent<AudioManager>();
    }

    void Update(){
        updateInfo();
        updateMoney();
        updateHealth();
        updateNavArrow();
        updateFuel();
    }

    public void clearText() {
        GameManager.infoText = "";
    }

    public void updateFuel() {
        int currFuel = (int)Math.Round(GameManager.fuel);
        int maxFuel = (int)Math.Round(GameManager.maxFuel);
        fuel.text = currFuel.ToString() + "/" + maxFuel.ToString();
    }

    private bool justBecameBroken = true;
    public void updateHealth(){
        int healt = GameManager.vehicleHealth;


        vehicleHealt.text = healt.ToString() + "/" + GameManager.maxVehicleHealth.ToString();

        if (healt < redHealthFrom) {
            if (justBecameBroken) {
                am.PlaySound(am.onBreak);
            }
            justBecameBroken = false;

            vehicleHealt.color = Color.red;
            if (healt < 0) {
                healt = 0;
                GameManager.vehicleHealth = healt;
            }
        } else {
            justBecameBroken = true;
            vehicleHealt.color = Color.green;
        }
    }

    private bool justBecamePoor = true;
    public void updateMoney(){
        int moneyVal = GameManager.money;
        money.text = moneyVal.ToString() + "$";
        
        if (moneyVal == 0) {
            if(justBecamePoor) {
                justBecamePoor = false;
                am.PlaySound(am.noCash);
            }
        } else {
            justBecamePoor = true;
        }
    }

    public void updateInfo(){
        string info = GameManager.infoText;
        infoText.text = info;
    }

    //Vector3 currentNavEulerAngles;
    public void updateNavArrow(){
        if (!GameManager.hasPassenger) {
            navArrow.enabled = false;
            return;
        }

        navArrow.enabled = true;

        int[] dest = GameManager.destination;
        Vector3 carPos = GameObject.Find("vehicles_yellow_taxi_0").transform.position;

        double angle = Math.Atan2((carPos.y - dest[0]), (carPos.x - dest[1]));
        float degrees = (float)(angle * (180 / Math.PI));

        navArrow.rectTransform.eulerAngles = new Vector3(0.0f, 0.0f, degrees);
    }
}
