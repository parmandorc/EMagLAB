﻿using System.Collections;
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
    [SerializeField]
    private GameObject [] SpawnPrefabs;

    [SerializeField]
    private Player PlayerPrefab;

    [SerializeField]
    private Arena Arena;

    [SerializeField]
    private float TimeBetweenSpawns;

    private List<GameObject> mObjects;
    private float mNextSpawn;

    void Awake()
    {
        ScreenManager.OnNewGame += ScreenManager_OnNewGame;
        ScreenManager.OnExitGame += ScreenManager_OnExitGame;
    }

    void Update()
    {
        if (isServer)
        {
            mNextSpawn -= Time.deltaTime;
            if (mNextSpawn <= 0.0f)
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
}
