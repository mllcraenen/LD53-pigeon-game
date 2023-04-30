using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PulseUI : MonoBehaviour {

    // Grow parameters
    public float pulseLength = 3.5f;
    public AnimationCurve scaleCurve;
    public float scalar = .2f;

    void Start() {
        StartCoroutine(Pulse());
    }

    IEnumerator Pulse() {
        while (true) {
            Vector3 currentScale = transform.localScale;
            float timer = 0f;
            while (timer < pulseLength) {
                transform.localScale = currentScale + currentScale * scaleCurve.Evaluate(timer / pulseLength) * scalar;
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}