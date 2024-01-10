using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager thisInstance = null;

    public static char[,] map;

    public static Vector3 worldSpaceToMap = new Vector3(0.2f, -0.2f, 1.0f);
    public static Vector3 mapToWorldSpace = new Vector3(5.0f, -5.0f, 1.0f);

    public static int money = 5;
    public static int vehicleHealth = 100;
    public static string infoText = "";

    public static float fuel = 100.0f;
    public static float maxFuel = 100.0f;

    public static int maxVehicleHealth = 100;

    public static int[,] crossings;
    public static int[,] shops;
    public static int[,] workshops;

    public static List<int[]> pendingPos;

    public static bool hasPassenger;
    public static int[] destination = {0, 0};
    public static int[] start = {0, 0};
    public static float currPriceTag;

    public static float globalTimescale = 1.0f;

    public static bool nuked = false;

    public static bool boughtSmth = false;

    // wydaje mi się, że kod do końca tego pliku jest niepotrzebny, ale boję się go usuwać
    private static GameManager _instance;
    public static GameManager Instance {
        get {
            if (_instance == null) {
                Debug.Log("GameManager is null");
            }
            return _instance;
        }
    }

    private void Awake(){
         if (!thisInstance)
         {
              thisInstance = this;
              DontDestroyOnLoad(gameObject);
         }
         else
         {
              //Duplicate GameManager created every time the scene is loaded
              Destroy(gameObject);
         }
    }
    /* public char[,] map; */
}
