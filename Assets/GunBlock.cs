using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBlock : BlockBehavior {

	Bullet bulletPrefab;

	// Use this for initialization
	public void Start () {
		base.Start();

		bulletPrefab = (Bullet)Resources.Load("Bullet", typeof(Bullet)) as Bullet;
	}

	public override void Fire() {
		Debug.Log ("gun block firing!");

		Bullet b;
		if (PhotonNetwork.connected) {
			var obj = PhotonNetwork.Instantiate ("Bullet", this.transform.position, Quaternion.identity, 1);
			b = obj.GetComponent<Bullet>();
		} else {
			b = Instantiate (bulletPrefab, this.transform.position, Quaternion.identity);
		}

		b.movement = this.transform.right + (Vector3)base.rb.velocity;
	}
}
