using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private AudioManager am;
    public string mainGameScene;
    // Start is called before the first frame update
    void Start()
    {
        am = GameObject.Find("Main Camera").GetComponent<AudioManager>();
        am.PlaySound(am.mapOpen);
        am.PlaySoundImmediate(am.fancyCinematic);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !am.audioSrc.isPlaying){
            // z jakiegoś dziwnego powodu po załadowaniu sceny nie usuwa się poprzednia, więc usuwam wszystkie obiekty ręcznie
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
            foreach(GameObject go in allObjects)
                if (go.activeInHierarchy)
                    UnityEngine.Object.Destroy(go);

            SceneManager.LoadScene(mainGameScene, LoadSceneMode.Additive);
        }
    }
}
