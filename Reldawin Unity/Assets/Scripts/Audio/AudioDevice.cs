using UnityEngine;

namespace LowCloud.Reldawin
{
    public class Sound
    {
        public enum Common
        {
            Pickup
        }
    }

    public class AudioDevice : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] common;

        public static AudioDevice Instance;

        public void Awake()
        {
            Instance = this;
            DontDestroyOnLoad( this );
        }

        public void Play( AudioClip clip )
        {
            if ( clip != null )
                audioSource.PlayOneShot( clip );
        }

        public void Play(Sound.Common index)
        {
            audioSource.PlayOneShot( common[(int)index] );
        }
    }
}