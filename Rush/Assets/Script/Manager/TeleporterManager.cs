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

        private Dictionary<ETeleportColor, List<Teleporter>> _DicoTeleporter = new Dictionary<ETeleportColor, List<Teleporter>>();

        private int _NextIndex = 1;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            Instance = this;
        }

        public void Register(Teleporter pTeleporter)
        {
            if(!_DicoTeleporter.ContainsKey(pTeleporter.CurrentColor))
                _DicoTeleporter[pTeleporter.CurrentColor] = new List<Teleporter>();

            _DicoTeleporter[pTeleporter.CurrentColor].Add(pTeleporter);

            AutoAssignIndexes(pTeleporter.CurrentColor);
        }

        private void AutoAssignIndexes(ETeleportColor pColor)
        {
            List<Teleporter> lListTeleporter = _DicoTeleporter[pColor];

            for (int i = 0; i < lListTeleporter.Count; i++)
            {
                lListTeleporter[i].Index = i;
            }
        }

        public Teleporter GetNext(Teleporter origin)
        {
            List<Teleporter> lList = _DicoTeleporter[origin.CurrentColor];

            int lNextIndex = (origin.Index + _NextIndex) % lList.Count;
            return lList[lNextIndex];
        }
    }
}
