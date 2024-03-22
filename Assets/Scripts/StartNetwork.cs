using Unity.Netcode;
using UnityEngine;

public class StartNetwork : MonoBehaviour
{
    
    //Start server/client/host and play audio
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        ClientMusicPlayer.Instance.PlayServerAudioClip();
    }
    
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        ClientMusicPlayer.Instance.PlayClientAudioClip();
    }
    
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        ClientMusicPlayer.Instance.PlayHostAudioClip();
    }
}
