using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// This class handles the operations that need to be performed on the NetworkManager from the UI inputs in order to create/join games.
// It serves as a level of abstraction from the ScreenManager, which might not know of specifics about networking.
public class NetworkManagerHUDCustom : MonoBehaviour
{
    [SerializeField]
    private uint NumberOfExtraChecksWhenJoining = 3;

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
        while (!mNetworkManager.IsClientConnected() && mNetworkManager.isNetworkActive)
        {
            yield return null;
        }

        bool failed = false;
        for (int i = 0; i < NumberOfExtraChecksWhenJoining && !failed; i++)
        {
            // Need to check extra times if successfully joined the game.
            // When connection is refused by host, there are cases where the connection is created very briefly before refusing it.
            // Double checking for a few frames consistently manages these cases.
            if (mNetworkManager.client == null || mNetworkManager.client.connection == null || mNetworkManager.client.connection.connectionId == -1)
            {
                mScreenManager.OnJoinError();
                failed = true;
            }
            else
            {
                yield return null;
            }
        }

        if (!failed)
        {
            mScreenManager.OnGameJoined();
        }
    }

    public void StopGame()
    {
        mNetworkManager.StopHost();
    }

    public void OnCancelJoin()
    {
        mNetworkManager.StopClient();

        StopCoroutine(mCoroutine);
    }
}
