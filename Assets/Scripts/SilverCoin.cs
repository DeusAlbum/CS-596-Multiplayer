using System;
using Unity.BossRoom.Infrastructure;
using UnityEngine;
using Unity.Netcode;

public class SilverCoin : NetworkBehaviour
{
    public GameObject prefab;
    
    //Trigger collision of player and coin
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        if (!NetworkManager.Singleton.IsServer) return;

        //Add length if its first tail or other tail, 3 for silver coin
        if (col.TryGetComponent(out PlayerLength playerLength))
        {
            playerLength.AddLength(3);
        }
        else if (col.TryGetComponent(out Tail tail))
        {
            tail.networkedOwner.GetComponent<PlayerLength>().AddLength(3);
        }

        //Despawn once collided
        NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, prefab);
        NetworkObject.Despawn();
    }
}
