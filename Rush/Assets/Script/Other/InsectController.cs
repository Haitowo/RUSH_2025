using Com.IsartDigital.Rush.Lighting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.Environment
{
    
    public class InsectController : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private LightingManager _Lighting;
        [SerializeField] private ParticleSystem _ParticlesButterfly, _ParticlesFireFly;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            if (_Lighting == null) return;

            PlayParticlesInsect();
        }

        private void PlayParticlesInsect()
        {
#if UNITY_ANDROID || UNITY_IOS
            Destroy(gameObject);
            return,;
#else
            bool lIsNight = _Lighting.IsNight();

            HandleParticles(_ParticlesButterfly, pShouldPlay: !lIsNight);
            HandleParticles(_ParticlesFireFly, pShouldPlay: lIsNight);
#endif
        }


        private void HandleParticles(ParticleSystem pParticles, bool pShouldPlay)
        {
            if (pParticles == null) return;

            if (pShouldPlay && !pParticles.isPlaying)
                pParticles.Play();
            else if (!pShouldPlay && pParticles.isPlaying)
                pParticles.Stop();
        }
    }
}
