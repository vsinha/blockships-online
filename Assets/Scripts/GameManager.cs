using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;
    public Transform[] spawns;

    private void Awake() {
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);

        playerPrefab = (GameObject)Resources.Load("Player", typeof(GameObject)) as GameObject;
    }

    void Start() {
		SpawnPlayer();
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom() {
        SceneManager.LoadScene("Launcher");
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer other) {
        Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.isMasterClient) {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected
            LoadArena();
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other) {
        Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects

        if (PhotonNetwork.isMasterClient) {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected

            LoadArena();
        }
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    void LoadArena() {
        if (!PhotonNetwork.isMasterClient) {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
        PhotonNetwork.LoadLevel("GameRoom");
    }

	void OnJoinedRoom () {
		Debug.Log ("We are Instantiating LocalPlayer from " + SceneManager.GetActiveScene ().name);

		//SpawnPlayer();
	}

	void SpawnPlayer() {

		if (ShipBehavior.LocalPlayerInstance == null) {
			Debug.Log ("We are Instantiating LocalPlayer from " + Application.loadedLevelName);
			// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
			var playerIndex = PhotonNetwork.playerList.Length - 1; // includes count of our own player

			if (playerIndex > spawns.Length) { // just in case
				playerIndex = playerIndex % spawns.Length;
			}

			GameObject player;
			if (PhotonNetwork.connected) {
				player = PhotonNetwork.Instantiate (this.playerPrefab.name, spawns [playerIndex].position, Quaternion.identity, 0);
			} else {
				player = Instantiate(this.playerPrefab, spawns [playerIndex].position, Quaternion.identity);
			}	

			//var appearance = player.GetComponent<PlayerAppearance>();
			//appearance.SetPlayerIndex(playerIndex);
		}else{
		    Debug.Log("Ignoring scene load for "+Application.loadedLevelName);
		    return;
		}
	}
}
