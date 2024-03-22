using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class PlayerLength : NetworkBehaviour
{
    [SerializeField] private GameObject tailPrefab;
    public NetworkVariable<ushort> length = new(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public static event System.Action<ushort> ChangedLengthEvent;
        
    private List<GameObject> _tails;
    private Transform _lastTail;
    private Collider2D _collider2D;

    //On network spawn make tail, add collider, etc.
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _tails = new List<GameObject>();
        _lastTail = transform;
        _collider2D = GetComponent<Collider2D>();
        if (!IsServer) length.OnValueChanged += LengthChangedEvent;
    }

    //On network despawn despawn all coins
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        DestroyCoins();
    }

    //Destroy coins that are attached
    private void DestroyCoins()
    {
        while (_tails.Count != 0)
        {
            GameObject tail = _tails[0];
            _tails.RemoveAt(0);
            Destroy(tail);
        }
    }
    
    //Add length to coin trail based off of value of coin
    public void AddLength(ushort value)
    {
        length.Value += value;
        LengthChanged();
    }

    //Instantiate tail increase and if ChangedLengthEvent happened invoke
    private void LengthChanged()
    {
        InstantiateTail();

        if (!IsOwner) return;
        ChangedLengthEvent?.Invoke(length.Value);
        //play audio
        ClientMusicPlayer.Instance.PlayChingAudioClip();
    }

    //callback
    private void LengthChangedEvent(ushort previousValue, ushort newValue)
    {
        Debug.Log("LengthChanged Callback");
        LengthChanged();
    }
    
    //Instantiate tail ass a game object and make physics for how tail follows
    private void InstantiateTail()
    {
        GameObject tailGameObject = Instantiate(tailPrefab, transform.position, Quaternion.identity);
        tailGameObject.GetComponent<SpriteRenderer>().sortingOrder = -length.Value;
        if (tailGameObject.TryGetComponent(out Tail tail))
        {
            tail.networkedOwner = transform;
            tail.followTransform = _lastTail;
            _lastTail = tailGameObject.transform;
            Physics2D.IgnoreCollision(tailGameObject.GetComponent<Collider2D>(), _collider2D);
        }
        _tails.Add(tailGameObject);
    }
}

