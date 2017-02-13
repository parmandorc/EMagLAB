using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State { Paused, Playing }

    [SerializeField]
    private GameObject [] SpawnPrefabs;

    [SerializeField]
    private Player PlayerPrefab;

    [SerializeField]
    private Arena Arena;

    [SerializeField]
    private float TimeBetweenSpawns;

    private List<GameObject> mObjects;
    private State mState;
    private float mNextSpawn;

    void Awake()
    {
        ScreenManager.OnNewGame += ScreenManager_OnNewGame;
        ScreenManager.OnExitGame += ScreenManager_OnExitGame;
    }

    void Start()
    {
        //Arena.Calculate();
        mState = State.Paused;
    }

    void Update()
    {
        if( mState == State.Playing)
        {
            mNextSpawn -= Time.deltaTime;
            if( mNextSpawn <= 0.0f )
            {
                if (mObjects == null)
                {
                    mObjects = new List<GameObject>();
                }

                int indexToSpawn = Random.Range(0, SpawnPrefabs.Length);
                GameObject spawnObject = SpawnPrefabs[indexToSpawn];
                GameObject spawnedInstance = Instantiate(spawnObject);
                spawnedInstance.transform.parent = transform;
                mObjects.Add(spawnedInstance);
                mNextSpawn = TimeBetweenSpawns;
            }
        }
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

        mNextSpawn = TimeBetweenSpawns;
        mState = State.Playing;
    }

    private void EndGame()
    {
        mState = State.Paused;
    }

    private void ScreenManager_OnNewGame()
    {
        BeginNewGame();
    }

    private void ScreenManager_OnExitGame()
    {
        EndGame();
    }
}
