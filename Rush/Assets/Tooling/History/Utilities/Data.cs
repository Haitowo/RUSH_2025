using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Editors.History.Utils;

namespace Editors.History
{
    public class Data : ScriptableObject
    {
        public string tabName = "Default";
        public Texture2D logo;
        [HideInInspector] public string typeCompleteName = string.Empty;

        public ShowType showType = ShowType.Preview;
        public bool showName = true;

        [HideInInspector] public string TypeName = string.Empty;

        [Range(3, 1024)] public int maxHistoryNumber = 30;
        [HideInInspector] public List<string> GUIDList = new List<string>();

        [HideInInspector] public string currentObject = string.Empty;

        public Color selectColor = Color.green;

        public event System.Action onValidate;

        public bool TryAdd(string pGUID)
        {
            if (GUIDList.Count + 1 < maxHistoryNumber && !GUIDList.Contains(pGUID) && pGUID != string.Empty && 
                GUIDToObject<Object>(pGUID).GetType().Name == TypeName)
            {
                GUIDList.Add(pGUID);
                EditorUtility.SetDirty(this);
                return true;
            }
            return false;
        }

        public bool TryAdd(Object pObject) => TryAdd(ObjectToGUID(pObject));

        public void TryAdds(IEnumerable<string> pGUIDEnumerable)
        {
            foreach (string item in pGUIDEnumerable)
            {
                TryAdd(item);
            }
        }

        public void TryAdds(IEnumerable<Object> pObjectEnumerable)
        {
            foreach (Object item in pObjectEnumerable)
            {
                TryAdd(item);
            }
        }

        public void TryRemove(string pGUID)
        {
            GUIDList.Remove(pGUID);
            EditorUtility.SetDirty(this);
        }

        public void TryRemove(Object pObject)
        {
            GUIDList.Remove(ObjectToGUID(pObject));
            EditorUtility.SetDirty(this);
        }

        public void TryRemoves(IEnumerable<string> pGUIDEnumerable)
        {
            foreach (string item in pGUIDEnumerable)
            {
                TryRemove(item);
            }
        }

        public void TryRemoves(IEnumerable<Object> pObjectEnumerable)
        {
            foreach (Object item in pObjectEnumerable)
            {
                TryAdd(item);
            }
        }

        private void OnValidate()
        {
            onValidate?.Invoke();
        }

        private void OnDestroy()
        {
            onValidate = null;
        }

        internal void RemoveAllPath() => GUIDList.Clear();

        public override string ToString()
        {
            StringBuilder lStringBuilder = new StringBuilder();
            foreach (string item in GUIDList)
            {
                lStringBuilder.Append("[");
                lStringBuilder.Append(item);
                lStringBuilder.Append("] ");
            }
            return lStringBuilder.ToString();
        }
    }

    public enum ShowType { Preview, Logo, None }
}