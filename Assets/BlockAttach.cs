using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAttach : MonoBehaviour {

	/* 
	 * 4 cardinal directions for attachment points
	 * 
	 *	  0
	 *  3 X 1
	 *    2
	 * 
	 */

	private Stack<GameObject> attachHistory = new Stack<GameObject>();

	private GameObject[] attachedBlocks = new GameObject[4];

	private Vector2[] validAttachPoints = new Vector2[] {
		new Vector2( 0,  1), // 0 north
		new Vector2( 1,  0), // 1 east
		new Vector2( 0, -1), // 2 south
		new Vector2(-1,  0)  // 3 west
	};

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space")) {
			Detach ();
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Block") {
			// what attach direction are we closest to? (within a tolerance)
			Vector3 attachPoint = ClosestAttachPoint (coll.gameObject);

			if (!IsValidAttachPoint(attachPoint)) {
				// don't attach
				return;
			}

			// snap to that position
			coll.gameObject.transform.position = this.transform.position - attachPoint;

			// create 2d joint
			Attach (coll.gameObject);
		}
	}

	void Attach(GameObject block) {
		attachHistory.Push(block);
		block.transform.SetParent(this.transform);
	}

	void Detach() {
		var block = attachHistory.Pop();
		if (block == null) { return; }
		block.transform.SetParent(null);
	}

	Vector2 ClosestAttachPoint(GameObject block) {

		var rel = this.transform.position - block.transform.position;
		var p = new Vector2();

		if (rel.x > -0.7 && rel.x < 0.7) {
			p.x = 0.0f;
		} else {
			p.x = rel.x; // -1 or 1
		}

		if (rel.y > -0.7 && rel.y < 0.7) {
			p.y = 0.0f;
		} else {
			p.y = rel.y; // -1 or 1
		}
			
		Debug.Log (p.ToString("F4"));

		p = Round (p);

		Debug.Log (p.ToString("F4"));

		return p;
	}

	private Vector2 Round(Vector2 vec) {
		return new Vector2(Mathf.Round(vec.x), Mathf.Round(vec.y));
	}

	private bool IsValidAttachPoint(Vector2 testPoint) {
		foreach( Vector2 validPoint in validAttachPoints) {
			if (validPoint.Equals (testPoint)) { 
				return true;
			}
		}
		return false;
	}

//	void ConnectToBlock(GameObject block1, GameObject block2) {
//		var joint = block1.AddComponent<FixedJoint2D> ();
//		joint.connectedBody = block2.GetComponent<Rigidbody2D> ();
//	}
}
