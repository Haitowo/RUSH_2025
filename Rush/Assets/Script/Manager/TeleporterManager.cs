using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.GameObjects;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 08/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.Manager
{
    
    public class TeleporterManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public static TeleporterManager Instance { get; private set; }

        private Dictionary<EColorSetter, List<Teleporter>> _DicoTeleporter = new Dictionary<EColorSetter, List<Teleporter>>();
        private Dictionary<Cube, Teleporter> _LastTeleporterUsed = new();

        private const int NEXT_INDEX = 1;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            #region Singleton Management
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                Debug.LogError(nameof(TeleporterManager) + "Instance already exists. Destroying the current instance.");
                return;
            }

            Instance = this;
            #endregion
        }

        public void Register(Teleporter pTeleporter)
        {
            if(!_DicoTeleporter.ContainsKey(pTeleporter.CurrentColor))
                _DicoTeleporter[pTeleporter.CurrentColor] = new List<Teleporter>();

            _DicoTeleporter[pTeleporter.CurrentColor].Add(pTeleporter);

            AutoAssignIndexes(pTeleporter.CurrentColor);
        }

        private void AutoAssignIndexes(EColorSetter pColor)
        {
            List<Teleporter> lListTeleporter = _DicoTeleporter[pColor];

            for (int i = 0; i < lListTeleporter.Count; i++)
            {
                lListTeleporter[i].Index = i;
            }
        }

        public Teleporter GetNext(Teleporter pOrigin)
        {
            List<Teleporter> lList = _DicoTeleporter[pOrigin.CurrentColor];

            int lNextIndex = (pOrigin.Index + NEXT_INDEX) % lList.Count;
            return lList[lNextIndex];
        }

        public bool CanTeleport(Cube pCube, Teleporter pTeleporter)
        {
            if (_LastTeleporterUsed.TryGetValue(pCube, out Teleporter pLast))
            {
                if (pLast == pTeleporter)
                    return false;
            }

            _LastTeleporterUsed[pCube] = pTeleporter;
            return true;
        }
    }
}
