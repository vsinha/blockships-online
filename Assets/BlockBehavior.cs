using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MonoBehaviour {
    private Rigidbody2D rb;
    private RigidbodyConstraints2D originalConstraints;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        originalConstraints = rb.constraints;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetAttached(GameObject parent) {
        rb.isKinematic = true;
        //rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    
    public void SetDetached() {
        rb.isKinematic = false;
        //rb.WakeUp();
        //rb.constraints = originalConstraints;
    }
}
