using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.Utilities;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.Audio
{
    
    public class SliderSound : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Slider _SoundSlider;
        [SerializeField] private EVolumeType _VolumeType;

        private float dB;
        private float lSliderValue;

        private SoundManager _SoundManager => SoundManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            GetVolumeValue();
            lSliderValue = _SoundManager.DBToSliderValue(dB);
            _SoundSlider.SetValueWithoutNotify(lSliderValue);
            _SoundSlider.onValueChanged.AddListener(ChangeSound);
        }

        private void GetVolumeValue()
        {
            switch (_VolumeType)
            {
                case EVolumeType.MASTER:
                    _SoundManager.mixer.GetFloat(Utils.MASTER_PARAM, out dB);
                    break;
                case EVolumeType.MUSIC:
                    _SoundManager.mixer.GetFloat(Utils.MUSIC_PARAM, out dB);
                    break;
                case EVolumeType.SFX:
                    _SoundManager.mixer.GetFloat(Utils.SOUND_PARAM, out dB);
                    break;
                default:
                    dB = 0;
                    break;
            }
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