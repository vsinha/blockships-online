using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBlock : BlockBehavior {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Fire() {
		Debug.Log("gun block firing!");
	}
}
