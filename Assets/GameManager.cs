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
    }


    void Start() {
        InstantiatePlayerFromPrefab();
    }

    private void InstantiatePlayerFromPrefab() {
        if (playerPrefab == null) {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            return;
        }

        if (!PhotonNetwork.connected) {
            Debug.Log("No photon network");

            if (PlayerManager.LocalPlayerInstance == null) {
                Instantiate(this.playerPrefab, spawns[0].position, Quaternion.identity);
            }
            return;
        }

        if (photonView.isMine) {
            Debug.Log("We are Instantiating LocalPlayer from " + SceneManager.GetActiveScene().name);

            var playerIndex = PhotonNetwork.playerList.Length - 1; // includes count of our own player

            if (playerIndex > spawns.Length) { // just in case
                playerIndex = playerIndex % spawns.Length;
            }

            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            var player = PhotonNetwork.Instantiate(this.playerPrefab.name, spawns[playerIndex].position, Quaternion.identity, 0);
            var appearance = player.GetComponent<PlayerAppearance>();
            appearance.SetPlayerIndex(playerIndex);
        }
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
}
