using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

public class Pigeon : MonoBehaviour {
    private Vector3 mPos;
    private Rigidbody2D rb;
    private Vector3 wind;
    private bool isSwitching = false;

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
    public AnimationCurve scaleCurve;
    public AnimationCurve rotationCurve;

    [Header("Scale settings")]
    public float scaleDuration = .3f;
    private float scalar = .2f;
    public float rotateDuration = .3f;
    public int rotation = 50;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        upperBound = grid.GetComponent<Collision>().layers.Length;
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

    }

    void Update() {
        keyInputs();
        layerColour();

        // Clamp max speed
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        // Rotate to face mouse
        mPos = Input.mousePosition;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(-90, 90, Mathf.InverseLerp(0, Screen.height, mPos.y)));

        // Add lift TODO:: Make this not suck
        rb.AddForce(transform.up * liftCoefficient);

        // Add wind if present
        rb.AddForce(wind);
    }

    void keyInputs() {
        // Flap
        if (Input.GetMouseButtonDown(0))
            Flap();
        // Move to a layer up
        if (Input.GetKeyDown(KeyCode.W) && collisionLayer + 1 < upperBound && !isSwitching) {
            collisionLayer++;
            StartCoroutine(ScaleCoroutine(scaleCurve, true));
            StartCoroutine(RotateCoroutine(rotationCurve));
        }

        // Move to a layer down 
        if (Input.GetKeyDown(KeyCode.S) && collisionLayer > 0 && !isSwitching) {
            collisionLayer--;
            StartCoroutine(ScaleCoroutine(scaleCurve, false)); 
            StartCoroutine(RotateCoroutine(rotationCurve));
        }
            
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
        spriteRenderer.sprite = sprites[0];
    }

    private IEnumerator ScaleCoroutine(AnimationCurve scaleCurve, bool upscale) {
        isSwitching = true;
        Vector3 currentScale = GetComponent<Pigeon>().transform.localScale;
        float timer = 0f;
        while (timer < scaleDuration) {
            if (upscale)
                GetComponent<Pigeon>().transform.localScale = currentScale + currentScale * scaleCurve.Evaluate(timer / scaleDuration) * scalar;
            else 
                GetComponent<Pigeon>().transform.localScale = currentScale - currentScale * scaleCurve.Evaluate(timer / scaleDuration) * (1 - (1 / (1 + scalar)));

            timer += Time.deltaTime;
            yield return null;
        }
        isSwitching = false;
    }

    private IEnumerator RotateCoroutine(AnimationCurve rotationCurve) {
        isSwitching = true;
        Quaternion currentRotation = GetComponent<Pigeon>().transform.rotation;
        float timer = 0f;
        while (timer < rotateDuration) {
            GetComponent<Pigeon>().transform.rotation = Quaternion.Euler(rotation * rotationCurve.Evaluate(timer / rotateDuration), currentRotation.y, currentRotation.z);
            timer += Time.deltaTime;
            yield return null;
        }
        isSwitching = false;
    }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Wind") {
            Wind collidedWind = collision.gameObject.GetComponent<Wind>();
            wind += collidedWind.transform.right * collidedWind.strength;
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "Wind") {
            Wind collidedWind = collision.gameObject.GetComponent<Wind>();
            wind -= collidedWind.transform.right * collidedWind.strength;
        }
    }
}