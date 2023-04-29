using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Pigeon : MonoBehaviour {
    // Movement speed
    public float speed = 2;

    // Flap force
    public float force = 300;

    // Current collision layer
    public int collisionLayer = 0;

    private int upperBound = 0;

    public GameObject grid;

    // Use this for initialization
    void Start() {
        // Fly towards the right
        GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
        upperBound = grid.GetComponent<Collision>().layers.Length;
        
    }

    // Update is called once per frame
    void Update() {
        // Flap
        if (Input.GetKeyDown(KeyCode.Space))
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * force);

        // Move to a layer up
        if (Input.GetKeyDown(KeyCode.W) && collisionLayer + 1 < upperBound)
            collisionLayer++;

        // Move to a layer down 
        if (Input.GetKeyDown(KeyCode.S) && collisionLayer > 0)
            collisionLayer--;
    }
}