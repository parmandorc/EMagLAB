using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerCustom : NetworkManager
{
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (GameManager.GameState != GameManager.State.Lobby)
        {
            conn.Disconnect();
            return;
        }

        if (GameManager.NumberOfPlayers >= GameManager.MaxNumberOfPlayers)
        {
            conn.Disconnect();
            return;
        }

        base.OnServerConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (GameManager.GameState != GameManager.State.Lobby || GameManager.NumberOfPlayers >= GameManager.MaxNumberOfPlayers)
        {
            return;
        }

        base.OnServerAddPlayer(conn, playerControllerId);
    }
}
