using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followed;
    public Camera cam;
    public int mapCameraSize;
    private AudioManager am;
    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<AudioManager>();
    }

    bool isMapView = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("m")) {
            if (!isMapView) { 
                isMapView = true;
                am.PlaySound(am.mapOpen);
                am.PlaySound(am.whereIsThat);
            }
            cam.orthographicSize = mapCameraSize;
            transform.position = Vector3.Scale(new Vector3(GameManager.map.GetLength(1), GameManager.map.GetLength(0), 0.0f), GameManager.mapToWorldSpace) * 0.5f + new Vector3(0.0f, 0.0f, -10.0f);
            /* float cameraDistance = 2.0f; // Constant factor */

            /* Vector3 objectSizes = new Vector3(-3, -3, 0) - new Vector3(GameManager.map.GetLength(1) * 5, GameManager.map.GetLength(0) * -5, 0); */
            /* float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z); */
            /* float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * cam.fieldOfView); // Visible height 1 meter in front */
            /* float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object */
            /* distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object */
            /* cam.transform.position = transform.position - distance * cam.transform.forward; */
        } else {
            isMapView = false;
            cam.orthographicSize = 5;
            transform.position = followed.position + new Vector3(0, 0, -10);
        }
    }
}
