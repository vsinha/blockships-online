using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraWork))]
public class ShipBehavior : Photon.MonoBehaviour, IPunObservable {

    private PhotonView playerView;
    private bool IsFiring;

    public float speed = 5.0f;

    //[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    private List<BlockBehavior> ChildBlocks;

    private Attachable attachable;

    void Awake() {

		// #Important
		// used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
		if (photonView.isMine)
		{
		    ShipBehavior.LocalPlayerInstance = this.gameObject;
		}

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start() {
		// #Important
		// used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
		if (photonView.isMine || !PhotonNetwork.connected) {
			ShipBehavior.LocalPlayerInstance = this.gameObject;
		}

		attachable = GetComponent<Attachable>();

		StartCameraWork();
    }

    void StartCameraWork() {
		CameraWork cw = this.gameObject.GetComponent<CameraWork> ();

		if (cw == null) {
			Debug.LogError ("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
			return;
		}

		if (PhotonNetwork.connected == false || photonView.isMine) {
			cw.StartFollowing (this.gameObject);
		}
	}

    // Update is called once per frame
    void FixedUpdate () {

		if (PhotonNetwork.connected == true && photonView.isMine == false) {
            return;
        }

        ChildBlocks = GetComponentsInChildren<BlockBehavior>().ToList();
        InputMovement();
        InputFiring();
        InputDetach();
    }

    private void InputMovement() {
		var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);

		transform.position += move * speed * Time.deltaTime;
	}

	private void InputFiring() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			foreach (BlockBehavior b in ChildBlocks) {
				b.Fire();
			}
		}
	}

	private void InputDetach() {
		if (Input.GetKeyDown(KeyCode.F)) {
            attachable.DetachMostRecentBlock();
        }
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            Vector3 pos = transform.localPosition;
            stream.Serialize(ref pos);
        } else {
            Vector3 pos = Vector3.zero;
            stream.Serialize(ref pos);  // pos gets filled-in. must be used somewhere
        }
    }
}
