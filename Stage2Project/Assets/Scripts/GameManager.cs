using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    public enum State { Playing, GameOver }

    // Events to update the UI scene
    public delegate void GameEvent();
    public static event GameEvent OnVictory;
    public static event GameEvent OnDefeat;

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

    [SyncVar]
    private State mState;

    [SyncVar]
    private float mGameTimeLeft;

    [SyncVar]
    private float mNextVortex;

    private GameObject mLocalPlayer;
    private List<GameObject> mObjects;
    private float mNextSpawn;
    private Vortex mVortex;

    public static State GameState { get; private set; }

    void Awake()
    {
        ScreenManager.OnNewGame += ScreenManager_OnNewGame;
        ScreenManager.OnExitGame += ScreenManager_OnExitGame;

        mVortex = FindObjectOfType<Vortex>();
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
        Player[] players = FindObjectsOfType<Player>();
        Dictionary<Player, int> scores = new Dictionary<Player, int>();
        foreach(Player p in players)
        {
            scores.Add(p, p.GetComponent<Score>().TotalScore);
        }

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

    [ClientRpc]
    private void RpcClaimVictor(GameObject victor)
    {
        if (victor == mLocalPlayer)
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

        mState = State.Playing;
        mNextSpawn = TimeBetweenSpawns + TimeBetweenSpawnsRandomVariance * Random.Range(-1.0f, 1.0f);
        mNextVortex = TimeBetweenVortexes;
        mGameTimeLeft = MaxGameDuration;
    }

    private void EndGame()
    {
        mVortex.enabled = false;
    }

    private void ScreenManager_OnNewGame()
    {
        BeginNewGame();
    }

    private void ScreenManager_OnExitGame()
    {
        EndGame();
    }

    public void AddPlayer(Player player)
    {
        // Assign the first unassigned Deposit to this player
        for (int i = 0; i < Deposits.Length; i++)
        {
            if (Deposits[i].Player == null)
            {
                Deposits[i].SetPlayer(player);
                break;
            }
        }
    }

    public void RemovePlayer(Player player)
    {
        // Unassign the Deposit that was assigned to this player, so it's available again
        for (int i = 0; i < Deposits.Length; i++)
        {
            if (Deposits[i].Player == player)
            {
                Deposits[i].SetPlayer(null);
            }
        }
    }
    
    // Registers the Player which holds the local player for this client.
    // When the Server claims a victor on game over, this is used to update the client's UI victory/defeat message accoridingly.
    public void RegisterLocalPlayer(Player player)
    {
        mLocalPlayer = player.gameObject;
    }
}
