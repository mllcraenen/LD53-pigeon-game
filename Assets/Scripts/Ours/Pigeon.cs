using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

public class Pigeon : MonoBehaviour {
    private Vector3 mPos;
    private Rigidbody2D rb;

    [Header("Flight settings")]
    public AnimationCurve forceCurve;
    public float flapForce = 200;
    public float flapDuration = .3f;
    public float rotationSpeed = 5f;
    public float flapVectorAngle = -45f;
    private Vector3 flapVector;

    public float maxSpeed = 5f;
    public float liftCoefficient = 1f;


    [Header("Tiling settings")]
    public int collisionLayer = 0;
    private int upperBound = 0;
    public GameObject grid;

    [Header("Sprite settings")]
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;


    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        upperBound = grid.GetComponent<Collision>().layers.Length;
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

    }

    void Update() {
        keyInputs();
        layerColour();

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        mPos = Input.mousePosition;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(-90, 90, Mathf.InverseLerp(0, Screen.height, mPos.y)));

        rb.AddForce(transform.up * liftCoefficient);
    }

    void keyInputs() {
        // Flap
        if (Input.GetMouseButtonDown(0))
            Flap();
        // Move to a layer up
        if (Input.GetKeyDown(KeyCode.W) && collisionLayer + 1 < upperBound)
            collisionLayer++;

        // Move to a layer down 
        if (Input.GetKeyDown(KeyCode.S) && collisionLayer > 0)
            collisionLayer--;
    }

    void layerColour() {
        Color color = grid.GetComponent<Collision>().layers[collisionLayer].GetComponent<Tilemap>().color;
        spriteRenderer.color = color;
    }

    public void Flap() {
        flapVector = Quaternion.AngleAxis(flapVectorAngle, Vector3.forward) * transform.up;
        StartCoroutine(FlapCoroutine(forceCurve));
        
    }

    private IEnumerator FlapCoroutine(AnimationCurve forceCurve) {
        spriteRenderer.sprite = sprites[1];
        float timer = 0f;
        float maxForce = flapForce;
        float currentForce = 0f;
        while (timer < flapDuration) {
            currentForce = forceCurve.Evaluate(timer / flapDuration) * maxForce;
            rb.AddForce(flapVector * currentForce);
            timer += Time.deltaTime;
            yield return null;
        }
        //timer = 0f;
		//while (timer < flapDuration) {
		//	currentForce = forceCurve.Evaluate(timer / flapDuration) * maxForce;
		//	rb.AddForce(flapVector * currentForce);
		//	timer += Time.deltaTime;
		//	yield return null;
		//}
        spriteRenderer.sprite = sprites[0];
	}
}