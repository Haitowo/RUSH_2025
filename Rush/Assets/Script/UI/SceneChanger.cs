using Com.IsartDigital.Rush.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class SceneChanger : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private float _ChangeDuration = 3f;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            _ChangeDuration -= Time.deltaTime;
            if(_ChangeDuration <= 0f)
                SceneManager.LoadScene(Utils.SCENE_GAME);
        }
    }
}