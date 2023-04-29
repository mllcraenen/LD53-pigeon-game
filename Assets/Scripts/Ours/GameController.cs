using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject pigeon;

    public GameObject endpoint;
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
            
    }

    public void OnGoalReached() {
        SceneManager.LoadScene("End");
    }
}
