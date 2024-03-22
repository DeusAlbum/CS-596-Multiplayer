using System.Collections;
using Unity.BossRoom.Infrastructure;
using Unity.Netcode;
using UnityEngine;

public class SilverCoinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private const int MaxPrefabCount = 50;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnCoinStart;
    }

    //Start coin spawning up to 9 for silver coins
    private void SpawnCoinStart()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnCoinStart;
        NetworkObjectPool.Singleton.InitializePool();
        for (int i = 0; i < 9; ++i)
        {
            SpawnCoin();
        }

        StartCoroutine(SpawnOverTime());
    }

    //How to actually spawn the coins and where to spawn them
    private void SpawnCoin()
    {
        NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(prefab,
            GetRandomPositionOnMap(), Quaternion.identity);
        obj.GetComponent<SilverCoin>().prefab = prefab;
        if (!obj.IsSpawned) obj.Spawn(destroyWithScene:true);
    }

    //Random position on map between certain x and y
    private Vector3 GetRandomPositionOnMap()
    {
        return new Vector3(Random.Range(-7f, 7f), Random.Range(-3f, 3f), 0f);
    }

    //Spawn over time and not immediately all of them
    IEnumerator SpawnOverTime()
    {
        while (NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            yield return new WaitForSeconds(2f);
            if (NetworkObjectPool.Singleton.GetCurrentPrefabCount(prefab) < MaxPrefabCount)
                SpawnCoin();
        }
    }
}

