using System.Collections;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource _audioBackSource;
        private AudioSource _audioEffectsSource;
        
        [SerializeField]
        private AudioClip[] backgroundClips;
        
        [SerializeField]
        private AudioClip winClip;
        
        [SerializeField]
        private AudioClip loseClip;
        
        public static AudioManager Instance { get; private set; }
        
        private void Awake()
        {
             _audioBackSource = GetComponent<AudioSource>();
             _audioEffectsSource = transform.GetChild(0).GetComponent<AudioSource>();
             _audioBackSource.clip = backgroundClips[0];
             _audioBackSource.Play();
             Instance = this; 
             DontDestroyOnLoad(this);
        }

        public void PlayEndgame(bool isWin)
        {
            _audioBackSource.Pause();
            _audioEffectsSource.clip = isWin ? winClip : loseClip;
            StartCoroutine(PlayAudio());
        }

        private IEnumerator PlayAudio()
        {
            _audioEffectsSource.Play();
            yield return new WaitForSeconds(3.0f);
            _audioEffectsSource.Stop();
            _audioBackSource.Play();
        }
    }
}