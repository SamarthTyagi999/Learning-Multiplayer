using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    private void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    void SetScreen(GameObject screen)
    {
        //deactivate all screens
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        //enable the requested screen
        screen.SetActive(true);
    }

    //called when "Create Room" button is pressed
    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    //called when "Join room" button is pressed
    public void OnJoinRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.JoinRoom(roomNameInput.text);
    }

    //called when the player name input field has been updated
    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    //called when we join a room
    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);

        //since there is a new player in the lobby tell everyone to update the lobby
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //we don't RPC it like join lobby
        //because OnJoinedRoom is only called for the player joining the room
        //while OnPalyerLeftRoom gets called for all the players in the room
        UpdateLobbyUI();
    }

    //update the lobby ui to show player list and host buttons
    //PunRpc is used for content which will be available for all players across the server
    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";

        //display all the players currently in lobby
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        //on;y host can start the game
        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    public void OnStartGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All , "Game");
    }

}
