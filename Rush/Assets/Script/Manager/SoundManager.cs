using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.Manager
{

    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _SoundGroup;
        [SerializeField] private AudioMixerGroup _MusicGroup;

        [SerializeField] private GameObject _MusicPlayerParent;
        [SerializeField] private GameObject _SoundPoolParent;

        [SerializeField] private AudioSource _SFX_Source;

        public static SoundManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // POOL
        private List<AudioSource> _InactiveSoundPlayer = new List<AudioSource>();
        private List<AudioSource> _ActiveSoundPlayer = new List<AudioSource>();

        private AudioSource _MusicPlayer;

        private bool _IsSoundAlreadyPlayed;

        private int _NumberOfSoundsToPlay = 8;

        private const float MIN_VALUE_SOUND = -25f;
        private const float NO_SOUND_VALUE = -80f;

        private void Init()
        {
            Instance = this;

            CreateMusicPlayer(_MusicPlayerParent);
            DontDestroyOnLoad(this);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY / AWAKE
        void Awake()
        {
            Init();

            for (int i = 0; i < _NumberOfSoundsToPlay; i++)
            {
                CreateSoundPlayer(_SoundPoolParent);
            }
        }

        private void CreateSoundPlayer(GameObject pObject)
        {
            AudioSource lInactiveSound = Instantiate(_SFX_Source);
            lInactiveSound.transform.SetParent(pObject.transform);
            _InactiveSoundPlayer.Add(lInactiveSound);
            lInactiveSound.outputAudioMixerGroup = _MusicGroup;
        }

        private void CreateMusicPlayer(GameObject pObject)
        {
            _MusicPlayer = pObject.AddComponent<AudioSource>();
            _MusicPlayer.transform.SetParent(_MusicPlayerParent.transform);
        }

        private void SoundFinish(AudioSource pCurrentSound)
        {
            _ActiveSoundPlayer.Remove(pCurrentSound);
            _InactiveSoundPlayer.Add(pCurrentSound);
        }

        public void PlayMusic(AudioClip pMusic)
        {
            _MusicPlayer.clip = pMusic;
            _MusicPlayer.loop = true;
            _MusicPlayer.Play();
            _MusicPlayer.outputAudioMixerGroup = _MusicGroup;
        }

        public void StopMusic() => _MusicPlayer.Stop();

        public void PlaySound(AudioClip pClip, Vector3 pSoundPosition)
        {
            if (_InactiveSoundPlayer.Count == 0) CreateSoundPlayer(_SoundPoolParent);
            else if (_IsSoundAlreadyPlayed) return;

            AudioSource lSoundPlayer = _InactiveSoundPlayer[0];
            _InactiveSoundPlayer.Remove(lSoundPlayer);
            _ActiveSoundPlayer.Add(lSoundPlayer);

            lSoundPlayer.clip = pClip;
            lSoundPlayer.transform.position = pSoundPosition;
            lSoundPlayer.Play();
            lSoundPlayer.loop = false;

            StartCoroutine(WaitForSoundToEnd(lSoundPlayer));
        }

        private IEnumerator WaitForSoundToEnd(AudioSource pSource)
        {
            yield return new WaitWhile(() => pSource.isPlaying);
            SoundFinish(pSource);
        }

        public void PlayRandomSound(AudioClip[] pSounds, Vector3 pPosition)
        {
            if (pSounds is null || pSounds.Length <= 0) return;

            int lRandomIndex = Random.Range(0, pSounds.Length - 1);
            PlaySound(pSounds[lRandomIndex], pPosition);
        }

        public void SoundVolume(float pValue)
        {
            if (pValue <= MIN_VALUE_SOUND)
                pValue = NO_SOUND_VALUE;
            foreach (AudioSource lSoundPlayer in _ActiveSoundPlayer)
            {
                lSoundPlayer.volume = pValue;
            }
        }

        public void MusicVolume(float pValue)
        {
            if (pValue <= MIN_VALUE_SOUND)
                pValue = NO_SOUND_VALUE;
            _MusicPlayer.volume = pValue;
        }
    }
}