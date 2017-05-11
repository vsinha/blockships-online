using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWork : MonoBehaviour {
    private GameObject mainCamera;
    private GameObject player;
    private bool following;

    Vector3 originalCameraHeight;

    // Use this for initialization
    void Start () {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		originalCameraHeight = new Vector3(0, 0, mainCamera.transform.position.z);
	}
	
	// Update is called once per frame
	void LateUpdate() {
        if (following) {
			if (player == null) { 
				Destroy (this); 
			}

			this.mainCamera.transform.position = player.transform.position + originalCameraHeight;
        }
	}

    internal void StartFollowing(GameObject player) {
        this.player = player;
        this.following = true;
    }
}
