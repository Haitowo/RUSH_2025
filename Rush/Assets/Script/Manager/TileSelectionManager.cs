using Com.IsartDigital.Rush.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.Manager
{
    public class TileSelectionManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        public static TileSelectionManager Instance { get; private set; }

        private List<TileEntryRuntime> _Entries;
        private int _CurrentIndex = 0;

        public TileEntryRuntime CurrentEntry => _Entries != null && _Entries.Count > 0 ? _Entries[_CurrentIndex] : null;

        //TODO : Change UI when selection or amount changes
        public event Action OnSelectionChanged;
        public event Action OnAmountChanged;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            Instance = this;

            #region Singleton Management
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                Debug.LogError(nameof(TileSelectionManager) + "Instance already exists. Destroying the current instance.");
                return;
            }

            Instance = this;
            //DontDestroyOnLoad(this);
            #endregion
        }

        public void Init(List<TileEntryRuntime> pEntries)
        {
            _Entries = pEntries;
            _CurrentIndex = 0;

            SkipIfEmpty();
        }

        public GameObject GetCurrentPrefab()
        {
            if (CurrentEntry == null) return null;
            return CurrentEntry.prefab;
        }

        public void UseOne()
        {
            if (CurrentEntry == null) return;

            CurrentEntry.UseOne();
            SkipIfEmpty();
        }

        private void SkipIfEmpty()
        {
            if(!HasAnyTilesAvailable())
                return;

            int lSafety = 0;

            while (CurrentEntry != null && CurrentEntry.remaining <= 0)
            {
                _CurrentIndex++;
                if (_CurrentIndex >= _Entries.Count)
                    _CurrentIndex = 0;

                lSafety++;
                if (lSafety > _Entries.Count) break;
            }
        }

        public void Next()
        {
            _CurrentIndex = (_CurrentIndex + 1) % _Entries.Count;
            SkipIfEmpty();
        }

        public void Previous()
        {
            _CurrentIndex--;
            if (_CurrentIndex < 0) _CurrentIndex = _Entries.Count - 1;
            SkipIfEmpty();
        }

        private bool HasAnyTilesAvailable()
        {
            foreach (TileEntryRuntime entry in _Entries)
            {
                if (entry.remaining > 0)
                    return true;
            }
            return false;
        }

        private void NotifySelectionChanged() => OnSelectionChanged?.Invoke();

        private void NotifyAmountChanged() => OnAmountChanged?.Invoke();
    }
}
