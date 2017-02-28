using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public const uint MaxNumberOfPlayers = 4;

    public enum State { Lobby, Playing, GameOver }

    // Events to update the UI scene
    public delegate void GameEvent();
    public static event GameEvent OnVictory;
    public static event GameEvent OnDefeat;
    public static event GameEvent OnStart;

    [SerializeField]
    private GameObject [] SpawnPrefabs;

    [SerializeField]
    private Player PlayerPrefab;

    [SerializeField]
    private Arena Arena;

    [SerializeField]
    private float TimeBetweenSpawns;

    [SerializeField]
    private float TimeBetweenSpawnsRandomVariance;

    [SerializeField]
    private float TimeBetweenVortexes;

    [SerializeField]
    private float VortexDuration;

    [SerializeField]
    private float MaxGameDuration;

    [SerializeField]
    private Deposit[] Deposits;

    [SerializeField]
    private Color[] PlayerColors;

    [SyncVar]
    private State mState;

    [SyncVar]
    private float mGameTimeLeft;

    [SyncVar]
    private float mNextVortex;

    private Player[] mPlayers;
    private GameObject mLocalPlayer;
    private List<GameObject> mObjects;
    private float mNextSpawn;
    private Vortex mVortex;

    public static State GameState { get; private set; }
    public static int NumberOfPlayers { get; private set; }

    void Awake()
    {
        ScreenManager.OnNewGame += ScreenManager_OnNewGame;
        ScreenManager.OnStartGame += ScreenManager_OnStartGame;
        ScreenManager.OnExitGame += ScreenManager_OnExitGame;

        mVortex = FindObjectOfType<Vortex>();
        mPlayers = new Player[MaxNumberOfPlayers];
        Debug.Assert(Deposits.Length >= MaxNumberOfPlayers, "Not enough deposits for the maximum amount of players.");
        Debug.Assert(PlayerColors.Length >= MaxNumberOfPlayers, "Not enough player colors for the maximum amount of players.");
    }

    void Update()
    {
        UpdateVortex();

        if (isServer)
        { 
            if (mState == State.Playing)
            { 
                mGameTimeLeft -= Time.deltaTime;
                if (mGameTimeLeft <= 0.0f)
                {
                    OnGameOver();
                }

                if (!mVortex.enabled)
                {
                    mNextSpawn -= Time.deltaTime;
                    if (mNextSpawn <= 0.0f)
                    {
                        SpawnObject();
                    }
                }
            }
        }

        if (GameState != mState)
            GameState = mState;
    }

    private void UpdateVortex()
    {
        if (isServer)
        {
            if (mState == State.Playing)
            {
                mNextVortex -= Time.deltaTime;
                if (mNextVortex <= -VortexDuration) // Vortex duration elapsed
                {
                    mNextVortex = TimeBetweenVortexes;
                }
            }
        }

        // Set the state of the vortex (this is done on clients too so the vortex state is updated properly).
        if (mNextVortex > 0.0f && mVortex.enabled)
        {
            mVortex.enabled = false;
        }
        else if (mNextVortex <= 0.0f && !mVortex.enabled)
        {
            mVortex.enabled = true;
        }
    }

    private void OnGameOver()
    {
        mGameTimeLeft = 0.0f;
        mState = State.GameOver;

        // Get the scores for all players
        Dictionary<Player, int> scores = new Dictionary<Player, int>();
        foreach(Player p in mPlayers)
        {
            if (p != null)
            {
                scores.Add(p, p.GetComponent<Score>().TotalScore);
            }
        }

        if (scores.Count > 0)
        {
            // Get the player with the highest score
            KeyValuePair<Player, int> victor = new KeyValuePair<Player, int>(null, -1);
            foreach (KeyValuePair<Player, int> kv in scores)
            {
                if (kv.Value > victor.Value)
                {
                    victor = kv;
                }
            }

            RpcClaimVictor(victor.Key.gameObject);
        }
        else
        {
            // All players are dead
            RpcClaimVictor(null);
        }
    }

    [ClientRpc]
    private void RpcClaimVictor(GameObject victor)
    {
        if (victor != null && victor == mLocalPlayer)
        {
            if (OnVictory != null)
            {
                OnVictory();
            }
        }
        else
        {
            if (OnDefeat != null)
            {
                OnDefeat();
            }
        }
    }

    private void SpawnObject()
    {
        if (mObjects == null)
        {
            mObjects = new List<GameObject>();
        }

        int indexToSpawn = Random.Range(0, SpawnPrefabs.Length);
        GameObject spawnObject = SpawnPrefabs[indexToSpawn];
        GameObject spawnedInstance = Instantiate(spawnObject);
        spawnedInstance.transform.parent = transform;
        NetworkServer.Spawn(spawnedInstance);
        mObjects.Add(spawnedInstance);
        mNextSpawn = TimeBetweenSpawns + TimeBetweenSpawnsRandomVariance * Random.Range(-1.0f, 1.0f);
    }

    
    private void BeginNewGame()
    {
        if (mObjects != null)
        {
            for (int count = 0; count < mObjects.Count; ++count)
            {
                Destroy(mObjects[count]);
            }
            mObjects.Clear();
        }

        mState = State.Lobby;
        mNextSpawn = TimeBetweenSpawns + TimeBetweenSpawnsRandomVariance * Random.Range(-1.0f, 1.0f);
        mNextVortex = TimeBetweenVortexes;
        mGameTimeLeft = MaxGameDuration;
    }

    private void EndGame()
    {
        mVortex.enabled = false;

        // Unregister players and unassign deposits
        for (int i = 0; i < MaxNumberOfPlayers; i++)
        {
            mPlayers[i] = null;
            if (Deposits[i].Player != null)
            {
                Deposits[i].SetPlayer(null);
            }
        }
        NumberOfPlayers = 0;

        mState = State.Lobby;
        GameState = State.Lobby;
    }

    private void ScreenManager_OnNewGame()
    {
        BeginNewGame();
    }

    [Server]
    private void ScreenManager_OnStartGame()
    {
        mState = State.Playing;
        RpcOnStartGame();
    }

    [ClientRpc]
    private void RpcOnStartGame()
    {
        if (OnStart != null)
        {
            OnStart();
        }
    }

    private void ScreenManager_OnExitGame()
    {
        EndGame();
    }

    [Server]
    public int AddPlayer(Player player)
    {

        // Assign the first unassigned Deposit to this player
        for (int i = 0; i < MaxNumberOfPlayers; i++)
        {
            if (mPlayers[i] == null)
            {
                mPlayers[i] = player;
                player.SetPlayerColor(PlayerColors[i]);
                Deposits[i].SetPlayer(player);
                ++NumberOfPlayers;
                return i;
            }
        }

        return -1;
    }

    [Client]
    public void SetPlayer(Player player, int playerIndex)
    {
        if (playerIndex >= MaxNumberOfPlayers)
        {
            Debug.LogWarning("Invalid player index: " + playerIndex);
            return;
        }

        mPlayers[playerIndex] = player;
        player.SetPlayerColor(PlayerColors[playerIndex]);
        Deposits[playerIndex].SetPlayer(player);
    }

    public Color GetPlayerColor(int playerIndex)
    {
        if (playerIndex >= MaxNumberOfPlayers)
        {
            Debug.LogWarning("Invalid player index: " + playerIndex);
            return Color.clear;
        }

        return PlayerColors[playerIndex];
    }

    public void RemovePlayer(int playerIndex)
    {
        if (playerIndex >= MaxNumberOfPlayers)
        {
            Debug.LogWarning("Invalid player index: " + playerIndex);
            return;
        }

        // Game Over for removed player
        if (mPlayers[playerIndex].gameObject == mLocalPlayer)
        {
            if (OnDefeat != null)
            {
                OnDefeat();
            }
        }

        // Unassign the Deposit that was assigned to this player, so it's available again
        mPlayers[playerIndex] = null;
        Deposits[playerIndex].SetPlayer(null);

        // Check for Game Over
        if (isServer)
        {
            if (--NumberOfPlayers <= 1)
            {
                OnGameOver();
            }
        }
    }
    
    // Registers the Player which holds the local player for this client.
    // When the Server claims a victor on game over, this is used to update the client's UI victory/defeat message accoridingly.
    public void RegisterLocalPlayer(Player player)
    {
        mLocalPlayer = player.gameObject;
    }

    // This is needed in the event the connection with the host is lost.
    // Should this happen, the scene must be reset for the main menu.
    void OnDisable()
    {
        EndGame();
    }
}
