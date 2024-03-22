using System.Collections;
using Unity.BossRoom.Infrastructure;
using Unity.Netcode;
using UnityEngine;

public class GoldCoinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private const int MaxPrefabCount = 50;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnCoinStart;
    }

    //Start coin spawning up to 3 for gold coins
    private void SpawnCoinStart()
    {
        NetworkManager.Singleton.OnServerStarted -= SpawnCoinStart;
        NetworkObjectPool.Singleton.InitializePool();
        for (int i = 0; i < 3; ++i)
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
        obj.GetComponent<GoldCoin>().prefab = prefab;
        if (!obj.IsSpawned) obj.Spawn(destroyWithScene:true);
    }

    //Random position on map between certain x and y bounds
    private Vector3 GetRandomPositionOnMap()
    {
        return new Vector3(Random.Range(-7f, 7f), Random.Range(-3f, 3f), 0f);
    }

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

