using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWork : MonoBehaviour {
    private GameObject mainCamera;
    private GameObject player;
    private bool following;

    // Use this for initialization
    void Start () {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void LateUpdate() {
        if (following) {
            this.mainCamera.transform.position = player.transform.position;
        }
	}

    internal void OnStartFollowing(GameObject player) {
        this.player = player;
        this.following = true;
    }
}
