using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public Pigeon pigeon;
    public GameObject boidSpawn;

    public bool debug = false;

    public GameObject boid;
    public int boidCount = 100;

    public GameObject endpoint;

    public Time time;
    
    void Start() {
        //spawnBoids(boidCount);
    }

    void spawnBoids(int n) {
        for (int i = 0; i < n; i++) {
            GameObject obj = Instantiate(boid, boidSpawn.transform.position, boidSpawn.transform.rotation);

            //Set last boid as debug boid
            if (i > n - 2) {
                BoidMovement b = obj.GetComponent<BoidMovement>();
                b.isDebugBoid = true;
                b.SetColor(Color.red);
            }
        }
    }

    void Update() {
        
    }

    public void OnGoalReached() {
        pigeon.controlsEnabled = true;
        PlayerPrefs.SetFloat("timePassed", Time.timeSinceLevelLoad);
        PlayerPrefs.SetString("level", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("End");
    }
}
