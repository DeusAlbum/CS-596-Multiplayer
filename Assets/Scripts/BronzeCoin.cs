using System;
using Unity.BossRoom.Infrastructure;
using UnityEngine;
using Unity.Netcode;

public class BronzeCoin : NetworkBehaviour
{
    public GameObject prefab;
    
    //Trigger collision of coin and player
    private void OnTriggerEnter2D(Collider2D col)
    {
        //Player collision return
        if (!col.CompareTag("Player")) return;

        //If server return
        if (!NetworkManager.Singleton.IsServer) return;

        //Add length if its first tail or other tail
        if (col.TryGetComponent(out PlayerLength playerLength))
        {
            playerLength.AddLength(1);
        }
        else if (col.TryGetComponent(out Tail tail))
        {
            tail.networkedOwner.GetComponent<PlayerLength>().AddLength(1);
        }

        //Despawn once collided
        NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, prefab);
        NetworkObject.Despawn();
    }
}

