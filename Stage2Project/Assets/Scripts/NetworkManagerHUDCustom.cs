using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// This class handles the operations that need to be performed on the NetworkManager from the UI inputs in order to create/join games.
// It serves as a level of abstraction from the ScreenManager, which might not know of specifics about networking.
public class NetworkManagerHUDCustom : MonoBehaviour
{
    private NetworkManager mNetworkManager;
    private ScreenManager mScreenManager;
    private IEnumerator mCoroutine;

    void Awake()
    {
        mNetworkManager = FindObjectOfType<NetworkManager>();
        mScreenManager = GetComponent<ScreenManager>();
    }

    // Tries to start a host. Returns whether it succeeded or not.
    public bool StartHost()
    {
        NetworkClient client = mNetworkManager.StartHost();
        return client != null;
    }

    public void StartClient(string networkAddress)
    {
        mNetworkManager.networkAddress = networkAddress;
        mNetworkManager.StartClient();

        mCoroutine = WaitForJoin();
        StartCoroutine(mCoroutine);
    }

    // Joining a game is an "async" operation. A coroutine is used to determine when the client has connected
    private IEnumerator WaitForJoin()
    {
        while (!mNetworkManager.IsClientConnected())
        {
            yield return null;
        }

        mScreenManager.OnGameJoined();
    }

    public void StopGame()
    {
        mNetworkManager.StopHost();
    }

    public void OnCancelJoin()
    {
        mNetworkManager.StopClient();
    }
}
