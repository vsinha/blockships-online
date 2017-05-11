using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior: MonoBehaviour  {

	public Rigidbody2D rb;

	public void Start() {
		rb = this.GetComponent<Rigidbody2D>();
	}

	public virtual int PowerConsumption { 
		get { return 1; }
	}

	public virtual int Mass { 
		get { return 1; }  
	}

	public virtual void RotateLeft() {
	}

	public virtual void RotateRight() { 
	}

	public virtual void Fire() {
	}
}


public struct Blocks {
	public enum Types {
		ThrustBlock,
		BatteryBlock,
		GunBlock
	}
}