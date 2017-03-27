using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachable : MonoBehaviour {

    /* 
	 * 4 cardinal directions for attachment points
	 * 
	 *	  0
	 *  3 X 1
	 *    2
	 * 
	 */

    public float DetachThrust = 50.0f;

    // the blockattach which initialized first is the core of the ship
    private Attachable Core = null;

    private Rigidbody2D rb;

    private Stack<Attachable> history = new Stack<Attachable>();

    // blocks attached to this block
    private Attachable[] attachedBlocks = new Attachable[4];

    private static Vector2[] ValidAttachPoints = new Vector2[] {
        new Vector2( 0,  1), // 0 north
		new Vector2( 1,  0), // 1 east
		new Vector2( 0, -1), // 2 south
		new Vector2(-1,  0)  // 3 west
	};

    private bool isCore;
    public Attachable Parent;

    public bool IsAttached;
    public int IndexRelativeToParent; // where am i attached to my parent block 
    private bool attachedInLastTick;

    // Use this for initialization
    void Start() {
        if (Core == null && this.tag == "Player") {
            Core = this;
            this.isCore = true;
            this.IsAttached = true;
        }

        this.IsAttached = false;
        this.IndexRelativeToParent = 0; // initialize.. to prevent null ref
        this.rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        if (this.isCore && Input.GetKeyDown("space")) {
            DetachMostRecentBlock();
        }


        // do this last
        this.attachedInLastTick = false;
    }

    void OnCollisionEnter2D(Collision2D coll) {
        if (this.isCore == false && this.IsAttached == false && this.attachedInLastTick == false) {
            // if we're a free-floating object that's not the ship's core
            // then don't handle collisions
            Debug.Log("ignoring collision");
            return;
        }

        if (coll.gameObject.tag == "Block") {

            Debug.Log(this.gameObject.name + " has collided with block " + coll.gameObject.name);

            // what attach direction are we closest to? (within a tolerance)
            Vector3 attachPoint = ClosestAttachPoint(coll.gameObject);
            int index = CalculateAttachPointIndex(attachPoint);

            if (index == -1) {
                // don't attach
                return;
            }

            Attachable block = coll.gameObject.GetComponent<Attachable>();

            if (block == null) {
                Debug.LogError("No Attachable component on the collided block");
            }

            AttachChild(index, block, this.Core);
        }
    }

    void AttachChild(int index, Attachable block, Attachable core)
    {
        // check if the attempted attach is valid
        if (index < 0) {
            Debug.Log("Invalid attach point");
            return;
        }
        if (attachedBlocks[index] != null) {
            // there's already a block in that spot!
            return;
        }

        // update the new child
        block.Core = core;
        block.Parent = this;
        block.IsAttached = true;
        block.rb.isKinematic = true;
        block.IndexRelativeToParent = index;
        block.attachedInLastTick = true; // protect against handling the collision twice

        // move it to the correct position
        block.transform.position = this.transform.position - (Vector3)ValidAttachPoints[index];
        block.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        block.transform.SetParent(this.transform);

        // store it
        this.attachedBlocks[index] = block;
        Debug.Log("pushing " + block.gameObject.name + " onto attach stack (" 
                    + Core.history.Count + ")");
        Core.history.Push(block);
    }

    // only performed by the core in Update()
    private void DetachMostRecentBlock()
    {
        if (!isCore) {
            // only call this function if we're the ship's core
            return;
        }
        if (Core.history.Count == 0) {
            return;
        }

        Debug.Log("detaching from stack of length: " + Core.history.Count);

        var mostRecentAddition = Core.history.Pop();
        Debug.Log("detaching: " + mostRecentAddition.gameObject.name);
        mostRecentAddition.DetachFromParent();
    }

    private void DetachFromParent()
    {
        if (this.isCore) {
            return;
        }

        this.Parent.attachedBlocks[IndexRelativeToParent] = null;
        this.transform.SetParent(null);
        this.Parent = null;

        this.Core = null;

        this.rb.isKinematic = false;
        this.rb.AddForce(ValidAttachPoints[IndexRelativeToParent] * -1 * DetachThrust);

        this.IndexRelativeToParent = -1;
        this.IsAttached = false;

        Debug.Log("detached block: " + this.gameObject.name);
    }

    Vector2 ClosestAttachPoint(GameObject block) {

        // relative position from us to the block we've made contact with
        var rel = this.transform.position - block.transform.position; 

        // position output
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
        
        return Round(p);
    }

    private Vector2 Round(Vector2 vec) {
        return new Vector2(Mathf.Round(vec.x), Mathf.Round(vec.y));
    }

    private int CalculateAttachPointIndex(Vector2 testPoint) {
        for (var i = 0; i < ValidAttachPoints.Length; i++) {
            if (ValidAttachPoints[i].Equals(testPoint)) {
                return i;
            }
        }
        return -1;
    }
}
