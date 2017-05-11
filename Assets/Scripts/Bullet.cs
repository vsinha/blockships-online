using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Photon.MonoBehaviour {

	public Vector3 movement;

	public float speed = 3f;

	public int maxLifetime = 1000;
	public int lifetime = 0;

	private Rigidbody2D rb;

	public void Shoot(Vector3 direction) {
		rb = GetComponent<Rigidbody2D> ();

		this.rb.velocity = speed * direction;
	}
	
	void Update() {
		lifetime += 1;
		if (lifetime > maxLifetime) {
			Destroy (this.gameObject);
		}
	}
}
