using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject pigeon;
    public GameObject boidSpawn;

    public bool debug = false;

    public GameObject boid;
    public int boidCount = 100;

    public GameObject endpoint;
    
    void Start() {
        spawnBoids(boidCount);
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
        SceneManager.LoadScene("End");
    }
}

/*
 * using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject boid;
    public int boidCount = 100;
    public Text boidCounterUI;
    public bool debug = false;

    void Start()
    {
        for (int i = 0; i < boidCount; i++) {
            GameObject obj = Instantiate(boid, new Vector3(Random.Range(-5,5), Random.Range(-5,5)), transform.rotation);

            BoidMovement b = obj.GetComponent<BoidMovement>();
            //Set last boid as debug boid
            if (i > boidCount - 2) {
                b.isDebugBoid = true;
                b.SetColor(Color.red);
            }
        }
        boidCounterUI.color = Color.gray;
        boidCounterUI.fontStyle = FontStyle.Bold;
    }

    void Update()
    {
		if (debug) {
            debugDisplayVisibleBoidCount();
        }
    }

    void debugDisplayVisibleBoidCount() {
        GameObject[] actors = GameObject.FindGameObjectsWithTag("Boid");
        int count = 0;
        foreach (GameObject actor in actors) {
            if (actor.GetComponent<Renderer>().isVisible) count++;
        }

        boidCounterUI.text = "Visible boids: " + count;
    }
}*/