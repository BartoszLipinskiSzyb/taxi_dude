using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkshopHandler : MonoBehaviour
{
    public float maxWorkshopRange;
    
    public Canvas ui;
    public Button speed;
    public Button health;
    public Button repair;

    private Image speedImg;
    private Image healthImg;
    private Image repairImg;

    public Sprite speedButtonDisabled;
    public Sprite speedButtonEnabled;
    public Sprite healthButtonDisabled;
    public Sprite healthButtonEnabled;
    public Sprite repairButtonDisabled;
    public Sprite repairButtonEnabled;

    public float speedUpgradeAmount = 3;
    public float vehicleHealthUpgradeAmount = 3;
    public float fuelCost = 6.34f;

    private AudioManager am;

    public GameObject car;
    private Drive carScript;

    // Start is called before the first frame update
    void Start()
    {
        am = GameObject.Find("Main Camera").GetComponent<AudioManager>();
        ui.enabled = false;

        speed.onClick.AddListener(upgradeSpeed);
        health.onClick.AddListener(upgradeHealth);
        repair.onClick.AddListener(upgradeRepair);

        carScript = car.GetComponent<Drive>();

        speedImg = speed.GetComponent<Image>();
        repairImg = repair.GetComponent<Image>();
        healthImg = health.GetComponent<Image>();
    }

    // Update is called once per frame
    private Vector3 wsPos = Vector3.zero;

    private float lastToggle = 0.0f;
    void FixedUpdate()
    {
        bool workshopAvailable = false;
        for (int ws = 0; ws < GameManager.workshops.GetLength(0); ++ws) {
            wsPos.x = GameManager.workshops[ws, 1];
            wsPos.y = GameManager.workshops[ws, 0];

            double distanceToWorkshop = Vector3.Distance(Vector3.Scale(wsPos, GameManager.mapToWorldSpace), transform.position);
            if (distanceToWorkshop < maxWorkshopRange) {
                workshopAvailable = true;
                break;
            }
        }

        bool gasPumpAvailable = false;
        for (int ws = 0; ws < GameManager.shops.GetLength(0); ++ws) {
            wsPos.x = GameManager.shops[ws, 1];
            wsPos.y = GameManager.shops[ws, 0];

            double distanceToWorkshop = Vector3.Distance(Vector3.Scale(wsPos, GameManager.mapToWorldSpace), transform.position);
            if (distanceToWorkshop < maxWorkshopRange) {
                gasPumpAvailable = true;
                break;
            }
        }
        
        if (gasPumpAvailable && GameManager.fuel < GameManager.maxFuel) {
            int cost = (int)Math.Floor((GameManager.maxFuel - GameManager.fuel) * fuelCost);
            GameManager.infoText = "Press F to refuel: " + cost.ToString() + "$";

            if (Input.GetKey("f") && GameManager.money >= cost) {
                GameManager.fuel = GameManager.maxFuel;
                GameManager.money -= cost;
            }
        } else if (workshopAvailable){
            //Debug.Log("Acessing workshop");
            GameManager.infoText = "Press R to visit workshop";

            if (Input.GetKey("r") && Time.time - lastToggle > 0.2f) {
                lastToggle = Time.time;
                toggleUI();
            }
        }

        if (GameManager.money < 100) {
            speedImg.sprite = speedButtonDisabled;
            repairImg.sprite = repairButtonDisabled;
            healthImg.sprite = healthButtonDisabled;
        } else {
            speedImg.sprite = speedButtonEnabled;
            repairImg.sprite = repairButtonEnabled;
            healthImg.sprite = healthButtonEnabled;
        }
    }

    private void upgradeSpeed(){
        if (GameManager.money >= 100) {
            GameManager.boughtSmth = true;
            am.PlaySound(am.habibGiveMoney);
            carScript.maxSpeed += speedUpgradeAmount / carScript.maxSpeed;
            GameManager.money -= 100;
        } else {
            am.PlaySound(am.habibNoMoney);
            am.PlaySound(am.noCash);
        }
    }


    private void upgradeHealth(){
        if (GameManager.money >= 100) {
            GameManager.boughtSmth = true;
            am.PlaySound(am.habibGiveMoney);
            carScript.maxVehicleHealth += (int)Math.Floor(((float)vehicleHealthUpgradeAmount * 100) / (float)carScript.maxVehicleHealth);
            GameManager.maxVehicleHealth = carScript.maxVehicleHealth;
            GameManager.money -= 100;
        } else {
            am.PlaySound(am.habibNoMoney);
            am.PlaySound(am.noCash);
        }
    }


    private void upgradeRepair(){
        if (GameManager.money >= 100) {
            GameManager.boughtSmth = true;
            am.PlaySound(am.habibGiveMoney);
            GameManager.vehicleHealth = carScript.maxVehicleHealth;
            carScript.vehicleHealth = carScript.maxVehicleHealth;
            GameManager.money -= 100;
        } else {
            am.PlaySound(am.habibNoMoney);
            am.PlaySound(am.noCash);
        }
    }

    private bool uiEnabled = false;
    private void toggleUI() {
        uiEnabled = !uiEnabled;
        ui.enabled = uiEnabled;

        if (uiEnabled)
        {
            am.PlaySound(am.workshopEnter);
            am.PlaySound(am.habibHello);
            if(GameManager.vehicleHealth < 40) {
                am.PlaySound(am.iWantToBuy);
            }
            GameManager.globalTimescale = 0.0f;
            GameManager.boughtSmth = false;
        }
        else {
            if (GameManager.boughtSmth) {
                am.PlaySound(am.habibThankYou);
            }
            GameManager.globalTimescale = 1.0f;
        }
    }

    
}
