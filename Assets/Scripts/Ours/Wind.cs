using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour {
	public float strength = 5f;
	private ParticleSystem ps;

	public void Start() {
		ps = GetComponentInChildren<ParticleSystem>();
		var main = ps.main;
		main.simulationSpeed = strength;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
        string path = "wind\\wind.png";
        Gizmos.DrawIcon(transform.position, path, true);
		Debug.DrawRay(transform.position, transform.right * 5, Color.magenta);
	}
#endif
}
