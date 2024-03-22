using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ClientMusicPlayer : Singleton<ClientMusicPlayer>
{
    //Audio clips for different sounds
    [SerializeField] private AudioClip chingAudioClip;
    [SerializeField] private AudioClip serverAudioClip;
    [SerializeField] private AudioClip clientAudioClip;
    [SerializeField] private AudioClip hostAudioClip;
    [SerializeField] private AudioClip explosionAudioClip;
    [SerializeField] private AudioClip gameOverAudioClip;
    
    private AudioSource _audioSource;

    public override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    //Play different kinds of sounds below
    public void PlayServerAudioClip()
    {
        _audioSource.clip = serverAudioClip;
        _audioSource.Play();
    }
    
    public void PlayClientAudioClip()
    {
        _audioSource.clip = clientAudioClip;
        _audioSource.Play();
    }
    public void PlayHostAudioClip()
    {
        _audioSource.clip = hostAudioClip;
        _audioSource.Play();
    }
    public void PlayChingAudioClip()
    {
        _audioSource.clip = chingAudioClip;
        _audioSource.Play();
    }
    
    public void PlayExplosionAudioClip()
    {
        _audioSource.clip = explosionAudioClip;
        _audioSource.Play();
    }
    
    public void PlayGameOverAudioClip()
    {
        _audioSource.clip = gameOverAudioClip;
        _audioSource.Play();
    }
}
