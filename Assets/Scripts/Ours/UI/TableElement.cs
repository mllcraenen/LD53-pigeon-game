using System.Collections;
using UnityEngine;

public class TableElement : MonoBehaviour {
    public GameObject levelSelectPanel;
    public float moveSpeed = 10f;
    public Vector3 startPos;

    private void Start() {
        startPos = levelSelectPanel.transform.position;
        levelSelectPanel.SetActive(false);
    }

    public void OnTableClick() {
        levelSelectPanel.SetActive(true);
        StartCoroutine(MoveIn());
    }

    IEnumerator MoveIn() {
        while (levelSelectPanel.transform.position.y > 0f) {
            levelSelectPanel.transform.position -= new Vector3(0f, moveSpeed * Time.deltaTime, 0f);
            yield return null;
        }
    }
}