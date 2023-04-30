using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;
using UnityEngine.U2D.Animation;

public class Pigeon : MonoBehaviour {
    private Vector3 mPos;
    private Rigidbody2D rb;
    private Vector3 wind;
    private bool isGrounded = false;
    private bool isFlapping = false;
    private bool isSwitching = false;

    public bool controlsEnabled = true;

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
    private SpriteRenderer[] bodyRenderers;
    private SpriteResolver bodySpriteResolver;
    private SpriteResolver letterSpriteResolver;
    private Transform headTransform;

    [Header("Scale settings")]
    public AnimationCurve scaleCurve;
    private float scaleDuration = .3f;
    private float scalar = .2f;
    private float rotateDuration = .3f;
    private float rotation = 30;


    // Use this for initialization
    void Start() {
        //flapSource.clip = Resources.Load<AudioClip>(flapClip.name);

        rb = GetComponent<Rigidbody2D>();
        upperBound = grid.GetComponent<Collision>().layers.Length;

        bodyRenderers = transform.Find("PIGEON_BODIES").gameObject.GetComponentsInChildren<SpriteRenderer>();
        bodySpriteResolver = transform.Find("PIGEON_BODIES").Find("BODY").gameObject.GetComponent<SpriteResolver>();
        headTransform = transform.Find("PIGEON_BODIES").Find("HEADBONE");
        spriteTakeOff();
        //letterSpriteResolver = transform.Find("PIGEON_BODIES").Find("LETTER").gameObject.GetComponent<SpriteResolver>();
    }

    void Update() {
        keyInputs();
        layerColour();
        rotatePigeon();

        // Clamp max speed
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

        // Add lift TODO:: Make this not suck
        rb.AddForce(transform.up * liftCoefficient);

        // Add wind if present
        rb.AddForce(wind);
    }

    void rotatePigeon() {
        if(!isGrounded) {
            //rotate body to mouse
            mPos = Input.mousePosition;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(-90, 90, Mathf.InverseLerp(0, Screen.height, mPos.y)));
            ////rotate head to right
            //Vector3 targetVector = transform.position + new Vector3(1f, 0f, 0f);
            //headTransform.LookAt(targetVector);
        } 
        //else {
        //    //rotate head to mouse
        //    mPos = Input.mousePosition;
        //    headTransform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(-90, 90, Mathf.InverseLerp(0, Screen.height, mPos.y)));
        //}


        if (!isFlapping) {
            if(Vector3.Dot(transform.right, Vector3.down) > 0.3f) {
                bodySpriteResolver.SetCategoryAndLabel("pigeonBody", "dive");

            } else bodySpriteResolver.SetCategoryAndLabel("pigeonBody", "glide");
        } 
    }

    void keyInputs() {
        // Flap
        if (Input.GetMouseButtonDown(0)) {
            Flap();
            GetComponents<AudioSource>()[0].Play();
        }
        // Move to a layer up
        if (Input.GetKeyDown(KeyCode.W) && collisionLayer + 1 < upperBound && !isSwitching) {
            GetComponents<AudioSource>()[1].Play();
            collisionLayer++;
            foreach (SpriteRenderer i in bodyRenderers) {
                i.sortingOrder = collisionLayer + 1;
            }
            StartCoroutine(ScaleCoroutine(scaleCurve, true));
            //StartCoroutine(RotateCoroutine(rotationCurve));
        }

        // Move to a layer down 
        if (Input.GetKeyDown(KeyCode.S) && collisionLayer > 0 && !isSwitching) {
            GetComponents<AudioSource>()[2].Play();
            collisionLayer--;
            foreach (SpriteRenderer i in bodyRenderers) {
                i.sortingOrder = collisionLayer + 1;
            }
            StartCoroutine(ScaleCoroutine(scaleCurve, false));
            //StartCoroutine(RotateCoroutine(rotationCurve));
        }
            
    }

    void layerColour() {
        Color color = grid.GetComponent<Collision>().layers[collisionLayer].GetComponent<Tilemap>().color;
       foreach (SpriteRenderer i in bodyRenderers) {
            i.color = Color.Lerp(color, Color.white, 0.8f);
        }
    }

    public void Flap() {
        PlayerPrefs.SetInt("flaps", PlayerPrefs.GetInt("flaps") + 1);
        flapVector = Quaternion.AngleAxis(flapVectorAngle, Vector3.forward) * transform.up;
        StartCoroutine(FlapCoroutine(forceCurve));
    }

    private IEnumerator FlapCoroutine(AnimationCurve forceCurve) {
        isFlapping = true;
        bodySpriteResolver.SetCategoryAndLabel("pigeonBody", "flap");
        float timer = 0f;
        float maxForce = flapForce;
        float currentForce = 0f;
        while (timer < flapDuration) {
            currentForce = forceCurve.Evaluate(timer / flapDuration) * maxForce;
            rb.AddForce(flapVector * currentForce);
            timer += Time.deltaTime;
            yield return null;
        }
        isFlapping = false;
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
        bodySpriteResolver.SetCategoryAndLabel("pigeonBody", "glide");
        isFlapping = false;
    }

    private IEnumerator RotateCoroutine(AnimationCurve rotationCurve) {
        isSwitching = true;
        float timer = 0f;
        while (timer < rotateDuration) {
            transform.rotation = Quaternion.AngleAxis(rotation * rotationCurve.Evaluate(timer / rotateDuration), transform.right);
            timer += Time.deltaTime;
            yield return null;
        }
        isSwitching = false;
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

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Floor") {
            isGrounded = true;
            spriteGoSit();
		}
    }
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "Floor") {
            isGrounded = false;
            spriteTakeOff();
        }
    }

    private void spriteGoSit() {
        foreach (SpriteRenderer i in bodyRenderers) {
            if (i.gameObject.name == "PIGEON_SIT")
                i.enabled = true;
            else i.enabled = false;
        }
    }
    private void spriteTakeOff() {
        foreach (SpriteRenderer i in bodyRenderers) {
            if (i.gameObject.name != "PIGEON_SIT")
                i.enabled = true;
            else i.enabled = false;
		}
	}

	private void OnDrawGizmos() {
        Debug.DrawRay(transform.position, transform.right, Color.blue);
    }
}