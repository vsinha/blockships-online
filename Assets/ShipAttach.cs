using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAttach : MonoBehaviour {

    /* 
	 * 4 cardinal directions for attachment points
	 * 
	 *	  0
	 *  3 X 1
	 *    2
	 * 
	 */

    public float DetachThrust = 10.0f;

    private Stack<AttachedBlock> attachHistory = new Stack<AttachedBlock>();
    private GameObject[] attachedBlocks = new GameObject[4];

    private Vector2[] validAttachPoints = new Vector2[] {
        new Vector2( 0,  1), // 0 north
		new Vector2( 1,  0), // 1 east
		new Vector2( 0, -1), // 2 south
		new Vector2(-1,  0)  // 3 west
	};

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("space")) {
            Detach();
        }
    }

    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "Block") {

            Debug.Log("collided with block");

            // what attach direction are we closest to? (within a tolerance)
            Vector3 attachPoint = ClosestAttachPoint(coll.gameObject);
            int index = AttachPointIndex(attachPoint);

            if (index == -1) {
                // don't attach
                return;
            }

            Attach(index, coll.gameObject);
        }
    }

    void Attach(int index, GameObject block) {

        if (index < 0) {
            Debug.Log("Invalid attach point");
            return;
        }

        if (attachedBlocks[index] != null) {
            // there's already a block in that spot!
            return;
        }

        var attachedBlock = new AttachedBlock(index, block);

        attachHistory.Push(attachedBlock);
        attachedBlocks[index] = block;

        // snap to that position
        block.transform.position = this.transform.position - (Vector3)validAttachPoints[index];
        block.transform.SetParent(this.transform);
        block.GetComponent<BlockBehavior>().SetAttached();
    }

    void Detach() {

        if (attachHistory.Count == 0) {
            return;
        }

        var attached = attachHistory.Pop();
        var localPosition = attached.block.transform.localPosition;

        attachedBlocks[attached.index] = null;

        attached.block.transform.SetParent(null);
        attached.block.GetComponent<Rigidbody2D>().AddForce(localPosition * DetachThrust);
        attached.block.GetComponent<BlockBehavior>().SetDetached();
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

        return Round(p);
    }

    private Vector2 Round(Vector2 vec) {
        return new Vector2(Mathf.Round(vec.x), Mathf.Round(vec.y));
    }

    private int AttachPointIndex(Vector2 testPoint) {
        for (var i = 0; i < validAttachPoints.Length; i++) {
            if (validAttachPoints[i].Equals(testPoint)) {
                return i;
            }
        }
        return -1;
    }

    struct AttachedBlock {
        public int index;
        public GameObject block;

        public AttachedBlock(int index, GameObject block)  {
            this.index = index;
            this.block = block;
        }
    }
}
