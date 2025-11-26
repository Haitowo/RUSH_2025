using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.Audio
{
    
    public class SliderSound : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Slider _SoundSlider;
        [SerializeField] private EVolumeType _VolumeType;

        private float _Sound;

        private SoundManager _SoundManager => SoundManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            _SoundSlider.SetValueWithoutNotify(_Sound);
            _SoundSlider.onValueChanged.AddListener(ChangeSound);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {

        }

        private void ChangeSound(float pValue)
        {
            switch (_VolumeType)
            {
                case EVolumeType.MASTER:
                    _SoundManager.ChangeVolume(pValue, Utils.MASTER_PARAM);
                    break;
                case EVolumeType.MUSIC:
                    _SoundManager.ChangeVolume(pValue, Utils.MUSIC_PARAM);
                    break;
                case EVolumeType.SFX:
                    _SoundManager.ChangeVolume(pValue, Utils.SOUND_PARAM);
                    break;
            }
        }
    }
}