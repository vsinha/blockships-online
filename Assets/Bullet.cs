using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Photon.MonoBehaviour {

	public Vector3 movement;

	public float speed = 3f;

	public int maxLifetime = 1000;
	public int lifetime = 0;
	
	// Update is called once per frame
	void FixedUpdate() {

		Debug.Log(speed * movement);
		this.transform.position += speed * movement;

		lifetime += 1;
		if (lifetime > maxLifetime) {
			Destroy (this.gameObject);
		}
	}
}
