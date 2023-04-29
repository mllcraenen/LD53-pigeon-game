using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

public class Pigeon : MonoBehaviour {
    // Movement speed
    public float speed = 2;

    Vector3 mPos;

    [Header("Flight settings")]
    public AnimationCurve forceCurve;
    public float flapForce = 200;
    public float flapDuration = .3f;
    public float rotationSpeed = 5f;
    public float flapVectorAngle = -45f;
    private Vector3 flapVector;
    //private float baseGravity = 1f;
    public float liftCoefficient = 1f;

    public int collisionLayer = 0;

    private int upperBound = 0;
    public GameObject grid;


    // Use this for initialization
    void Start() {
        // Fly towards the right
        GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
        upperBound = grid.GetComponent<Collision>().layers.Length;
        
    }

    //private void calcGlideGravity() {
    //    float angleOfAttack = Vector2.Dot(GetComponent<Rigidbody2D>().velocity.normalized, Vector2.up);
    //    Debug.DrawRay(transform.position, GetComponent<Rigidbody2D>().velocity.normalized);
    //    //Debug.Log(angleOfAttack);
    //    float gravityMultiplier = 1f - Mathf.Clamp01(angleOfAttack);
    //    GetComponent<Rigidbody2D>().gravityScale = baseGravity * gravityMultiplier;
    //    Debug.Log(baseGravity * gravityMultiplier);
    //}

    void Update() {
        //calcGlideGravity();
        // Flap
        if (Input.GetMouseButtonDown(0))
            Flap();
        // Move to a layer up
        if (Input.GetKeyDown(KeyCode.W) && collisionLayer + 1 < upperBound)
            collisionLayer++;

        // Move to a layer down 
        if (Input.GetKeyDown(KeyCode.S) && collisionLayer > 0)
            collisionLayer--;

        mPos = Input.mousePosition;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(-90, 90, Mathf.InverseLerp(0, Screen.height, mPos.y)));

        GetComponent<Rigidbody2D>().AddForce(transform.up * liftCoefficient);
    }

    public void Flap() {
        flapVector = Quaternion.AngleAxis(flapVectorAngle, Vector3.forward) * transform.up;
        StartCoroutine(FlapCoroutine(forceCurve));
    }

    private IEnumerator FlapCoroutine(AnimationCurve forceCurve) {
        float timer = 0f;
        float maxForce = flapForce;
        float currentForce = 0f;
        while (timer < flapDuration) {
            currentForce = forceCurve.Evaluate(timer / flapDuration) * maxForce;
            GetComponent<Rigidbody2D>().AddForce(flapVector * currentForce);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0f;
        while (timer < flapDuration) {
            currentForce = forceCurve.Evaluate(timer / flapDuration) * maxForce;
            GetComponent<Rigidbody2D>().AddForce(flapVector * currentForce);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}