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
    private static Attachable Core = null;

    private Rigidbody rb;

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

    // Use this for initialization
    void Start() {
        if (Core == null) {
            Core = this;
            this.isCore = true;
        }

        this.IsAttached = false;
        this.IndexRelativeToParent = 0; // initialize.. to prevent null ref
    }

    // Update is called once per frame
    void Update() {
        if (this.isCore && Input.GetKeyDown("space")) {
            DetachMostRecentBlock();
        }
    }

    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "Block") {

            Debug.Log("collided with block");

            // what attach direction are we closest to? (within a tolerance)
            Vector3 attachPoint = ClosestAttachPoint(coll.gameObject);
            int index = CalculateAttachPointIndex(attachPoint);

            if (index == -1) {
                // don't attach
                return;
            }

            Attach(index, coll.gameObject);
        }
    }

    void AttachChild(Attachable block)
    {

    }

    void AttachParent(Attachable block)
    {

    }

    // only performed by the core in Update()
    private void DetachMostRecentBlock()
    {
        if (!isCore) {
            return;
        }

        if (Core.history.Count == 0) {
            return;
        }
        var mostRecentAddition = Core.history.Pop();

        mostRecentAddition.DetachFromParent();
    }

    private void DetachFromParent()
    {
        this.Parent.DetachChildAtIndex(this.IndexRelativeToParent);
        this.transform.SetParent(null);
        this.Parent = null;
        this.gameObject.GetComponent<BlockBehavior>().SetDetached();
        this.GetComponent<Rigidbody2D>().AddForce(ValidAttachPoints[IndexRelativeToParent] * DetachThrust);
        this.IndexRelativeToParent = -1;
    }

    void DetachChildAtIndex(int index)
    {
        attachedBlocks[index] = null;
    }

    void Attach(int index, GameObject block) {

        block.GetComponent<BlockBehavior>().SetAttached(this.gameObject);

        if (index < 0) {
            Debug.Log("Invalid attach point");
            return;
        }

        if (attachedBlocks[index] != null) {
            // there's already a block in that spot!
            return;
        }

        Core.history.Push(attachedBlock);
        attachedBlocks[index] = block;

        // snap to that position
        block.transform.position = this.transform.position - (Vector3)ValidAttachPoints[index];
        block.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        block.transform.SetParent(this.transform);
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
