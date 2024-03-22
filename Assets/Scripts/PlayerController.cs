using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 3f;
    
    public static event System.Action GameOverEvent;
    
    private Camera _mainCamera;
    private Vector3 _mouseInput = Vector3.zero;
    private PlayerLength _playerLength;
    private bool _canCollide = true;

    private readonly ulong[] _targetClientsArray = new ulong[1];
    
    //Initialize camera and playerlength
    private void Initialize() {
        _mainCamera = Camera.main;
        _playerLength = GetComponent<PlayerLength>();
    }

    //On network spawn just initialize
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }
    
    //Update once per frame
    private void Update()
    {
        //If not owner or application isn't focused return
        if (!IsOwner || !Application.isFocused) return;
        
        //Movement
        _mouseInput.x = Input.mousePosition.x;
        _mouseInput.y = Input.mousePosition.y;
        _mouseInput.z = _mainCamera.nearClipPlane;
        Vector3 mouseWorldCoordinates = _mainCamera.ScreenToWorldPoint(_mouseInput);
        mouseWorldCoordinates.z = 0f;
        transform.position = Vector3.MoveTowards(transform.position, mouseWorldCoordinates, Time.deltaTime * speed);
        
        //Rotate
        if (mouseWorldCoordinates != transform.position)
        {
            Vector3 targetDirection = mouseWorldCoordinates - transform.position;
            transform.up = targetDirection;
        }
    }

    //ServerRpc to determine who is the winner in a player collision
    [ServerRpc(RequireOwnership = false)]
    private void DetermineCollisionWinnerServerRpc(PlayerData player1, PlayerData player2)
    {
        if (player1.Length > player2.Length)
        {
            WinInformationServerRpc(player1.Id, player2.Id);
        }
        else
        {
            WinInformationServerRpc(player2.Id, player1.Id);
        }
    }

    //ServerRpc to send the Winner information
    [ServerRpc]
    private void WinInformationServerRpc(ulong winner, ulong loser)
    {
        _targetClientsArray[0] = winner;
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = _targetClientsArray
            }
        };
        AtePlayerClientRpc(clientRpcParams);

        _targetClientsArray[0] = loser;
        clientRpcParams.Send.TargetClientIds = _targetClientsArray;
        GameOverClientRpc(clientRpcParams);
    }

    //ClientRpc to say you beat a player if owner
    [ClientRpc]
    private void AtePlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Debug.Log("You bested a player! :)");
        
    }

    //Clientrpc to say you lost and invoke the GameOver event if true and shut down
    [ClientRpc]
    private void GameOverClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Debug.Log("You lost :(");
        GameOverEvent?.Invoke();
        NetworkManager.Singleton.Shutdown();
    }

    //IEnumerator to time if able to collide at first
    private IEnumerator CollisionCheckCoroutine()
    {
        _canCollide = false;
        yield return new WaitForSeconds(0.5f);
        _canCollide = true;
    }
    
    //On collision check if head or tail collision and call determine winner with data
    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Player Collision");
        if (!col.gameObject.CompareTag("Player")) return;
        if (!IsOwner) return;
        if (!_canCollide) return;
        StartCoroutine(CollisionCheckCoroutine());
        
        //Head on collision
        if (col.gameObject.TryGetComponent(out PlayerLength playerLength))
        {
            //Get player data
            Debug.Log("Head Collision");
            var player1 = new PlayerData()
            {
                Id = OwnerClientId,
                Length = _playerLength.length.Value
            };
            var player2 = new PlayerData()
            {
                Id = playerLength.OwnerClientId,
                Length = playerLength.length.Value
            };
            
            //Send player data for DetermineCollision... and play audio
            ClientMusicPlayer.Instance.PlayExplosionAudioClip();
            DetermineCollisionWinnerServerRpc(player1, player2);
            ClientMusicPlayer.Instance.PlayExplosionAudioClip();
            Thread.Sleep(1000);
            ClientMusicPlayer.Instance.PlayGameOverAudioClip();
        }
        
        //Tail collision
        else if (col.gameObject.TryGetComponent(out Tail tail))
        {
            //Play audio and send WinnerInformation
            Debug.Log("Tail Collision");
            ClientMusicPlayer.Instance.PlayExplosionAudioClip();
            WinInformationServerRpc(tail.networkedOwner.GetComponent<PlayerController>().OwnerClientId,
                OwnerClientId);
            ClientMusicPlayer.Instance.PlayExplosionAudioClip();
            Thread.Sleep(1000);
            ClientMusicPlayer.Instance.PlayGameOverAudioClip();
        }
    }

    //Player data serialized
    struct PlayerData : INetworkSerializable
    {
        public ulong Id;
        public ushort Length;


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Length);
        }
    }
}
