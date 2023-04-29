using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Collision : MonoBehaviour
{
    public GameObject pigeon;

    // Array of layers 
    public GameObject[] layers;

    // Start is called before the first frame update
    void Start() {
        setCollisionScript();
    }

    // Update is called once per frame
    void Update() {
        setCollisionScript();
    }

    void setCollisionScript() {
        for (int i = 0; i < layers.Length; i++) {
            if (i == pigeon.GetComponent<Pigeon>().collisionLayer) 
                layers[i].GetComponent<TilemapCollider2D>().enabled = true;
            else 
                layers[i].GetComponent<TilemapCollider2D>().enabled = false;
        }
        
    }
}
