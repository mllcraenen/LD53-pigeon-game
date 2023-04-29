using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

public class Pigeon : MonoBehaviour {
    // Movement speed
    public float speed = 2;

    Vector3 mPos;

    // Flap force
    public float force = 300;
    public float rotationSpeed = 5f;

    public int collisionLayer = 0;

    private int upperBound = 0;
    public GameObject grid;


    // Use this for initialization
    void Start() {
        // Fly towards the right
        GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
        upperBound = grid.GetComponent<Collision>().layers.Length;
        
    }

    void Update() {
        // Flap
        if (Input.GetMouseButtonDown(0))
			GetComponent<Rigidbody2D>().AddForce(transform.up * force);

        mPos = Input.mousePosition;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(-90, 90, Mathf.InverseLerp(0, Screen.height, mPos.y)));


        // Move to a layer up
        if (Input.GetKeyDown(KeyCode.W) && collisionLayer + 1 < upperBound)
            collisionLayer++;

        // Move to a layer down 
        if (Input.GetKeyDown(KeyCode.S) && collisionLayer > 0)
            collisionLayer--;
    }
    //void OnDrawGizmos() {
    //    Debug.DrawRay(transform.position, transform.up, Color.yellow);
    //    GUIStyle biggus = new GUIStyle();
    //    biggus.fontSize = 30;
    //    biggus.normal.textColor = Color.white;
    //    Gizmos.color = Color.white;
    //    Handles.Label(transform.position, mPos.ToString(), biggus);
    //}
}