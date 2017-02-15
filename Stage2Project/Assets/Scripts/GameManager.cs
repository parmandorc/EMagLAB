using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* The State (Paused | Playing) is not needed anymore, since, with the instance of this class being a NetworkIdentity,
 * it is now handled by the NetworkManager (the GameManager needs to be a NetworkIdentity because if not it is not possible
 * to spawn objects across the network). 
 * Consequently, when the manager is not connected (that is, the game is "paused" / in the main menu), 
 * the instance of the GameManager will not be enabled, and its Update function will not be called (hence no need for paused state).
 */ 
public class GameManager : NetworkBehaviour
{
    public enum State { Playing, GameOver }

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
    private float MaxGameDuration;

    [SyncVar]
    private State mState;

    [SyncVar]
    private float mGameTimeLeft;

    private GameObject mLocalPlayer;
    private List<GameObject> mObjects;
    private float mNextSpawn;

    public static State GameState { get; private set; }

    void Awake()
    {
        ScreenManager.OnNewGame += ScreenManager_OnNewGame;
        ScreenManager.OnExitGame += ScreenManager_OnExitGame;
    }

    void Update()
    {
        if (isServer)
        {
            if (mState == State.Playing)
            {
                mGameTimeLeft -= Time.deltaTime;
                if (mGameTimeLeft <= 0.0f)
                {
                    OnGameOver();
                }

                mNextSpawn -= Time.deltaTime;
                if (mNextSpawn <= 0.0f)
                {
                    SpawnObject();
                }
            }
        }

        if (GameState != mState)
            GameState = mState;
    }

    private void OnGameOver()
    {
        mGameTimeLeft = 0.0f;
        mState = State.GameOver;

        // Get the scores for all players
        MagnetizedByPlayer[] objs = FindObjectsOfType<MagnetizedByPlayer>();
        Dictionary<Player, int> scores = new Dictionary<Player, int>();
        foreach(MagnetizedByPlayer o in objs)
        {
            if (o.isScore && o.ActivePlayer != null)
            {
                if (!scores.ContainsKey(o.ActivePlayer))
                    scores[o.ActivePlayer] = 1;
                else
                    scores[o.ActivePlayer]++;
            }
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
        mNextSpawn = TimeBetweenSpawns;
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
        mNextSpawn = TimeBetweenSpawns;
        mGameTimeLeft = MaxGameDuration;
    }

    private void EndGame()
    {

    }

    private void ScreenManager_OnNewGame()
    {
        BeginNewGame();
    }

    private void ScreenManager_OnExitGame()
    {
        EndGame();
    }

    public void RegisterLocalPlayer(Player player)
    {
        mLocalPlayer = player.gameObject;
    }
}
