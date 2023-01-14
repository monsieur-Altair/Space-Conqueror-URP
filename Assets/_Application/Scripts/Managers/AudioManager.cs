using System.Collections;
using _Application.Scripts.Infrastructure.Services;
using UnityEngine;

namespace _Application.Scripts.Managers
{
    public class AudioManager : MonoBehaviourService
    {
        private AudioSource _audioBackSource;
        private AudioSource _audioEffectsSource;
        
        [SerializeField]
        private AudioClip[] backgroundClips;
        
        [SerializeField]
        private AudioClip winClip;
        
        [SerializeField]
        private AudioClip loseClip;
        
        private Coroutine _playAudioCor;

        public void StartPlayBack()
        {
            _audioBackSource.Play();
        }

        public override void Init()
        {
            base.Init();
            
            _audioBackSource = GetComponent<AudioSource>();
            _audioEffectsSource = transform.GetChild(0).GetComponent<AudioSource>();
            _audioBackSource.clip = backgroundClips[0];
        }

        public void PlayEndgame(bool isWin)
        {
            _audioBackSource.Pause();
            _audioEffectsSource.clip = isWin ? winClip : loseClip;
            _playAudioCor = StartCoroutine(PlayAudio());
        }

        public void PlayBackgroundAgain()
        {
            StopCoroutine(_playAudioCor);
            
            _audioEffectsSource.Stop();
            _audioBackSource.Play();
        }

        private IEnumerator PlayAudio()
        {
            _audioEffectsSource.Play();
            while (_audioEffectsSource.isPlaying)
                yield return null;

            _audioEffectsSource.Stop();
            _audioBackSource.Play();
        }
    }
}