using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float speed = 5.0f;
    private PhotonView player;

    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

   
    // Use this for initialization
    void Start () {
        player = GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //if (player.isMine) {
            InputMovement();
        //}
    }

    // in an "observed" script:
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            Vector3 pos = transform.localPosition;
            stream.Serialize(ref pos);
        } else {
            Vector3 pos = Vector3.zero;
            stream.Serialize(ref pos);  // pos gets filled-in. must be used somewhere
        }
    }

    private void InputMovement() {
		var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

		transform.position += move * speed * Time.deltaTime;
	}
}
