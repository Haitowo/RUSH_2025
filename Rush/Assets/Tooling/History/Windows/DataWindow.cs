using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Editors.History.Utils;

namespace Editors.History
{
    public class DataWindow : FocusableWindow
    {
        private Data _Data;
        private SerializedObject _SerializedData;
        List<SerializedProperty> _DataPropertyList = new List<SerializedProperty>();

        System.Type _Type;

        public Object[] temporalData;
        private bool _IsListening = true;
        private bool _IsUpdatingIcon = true;
        private string _CurrentPath;

        public static DataWindow Open() => SetWindowAndOpen();
        public static DataWindow Open(Object[] pObjectArray)
        {
            DataWindow lWindow = SetWindowAndOpen();
            lWindow.SetType(pObjectArray);
            return lWindow;
        }

        private static DataWindow SetWindowAndOpen()
        {
            DataWindow lWindow = GetWindow<DataWindow>();
            lWindow.titleContent = new GUIContent("Tab Creator", (Texture2D)EditorGUIUtility.IconContent(FOLDER_ICON).image);
            return lWindow;
        }

        private void CreateGUI()
        {
            FocusControl();
            _Data = new Data();
            SetSeriazableVariable();
            Selection.selectionChanged += GetIconAndType;
        }

        private void OnGUI()
        {
            _CurrentPath = GetDataFullPath();
            ShowParameter();
            ListenToType();
            Create();
            _SerializedData.ApplyModifiedProperties(); //TODO OPTIMIZE
        }

        private void SetSeriazableVariable()
        {
            _SerializedData = new SerializedObject(_Data);
            SerializedProperty pSerialized = _SerializedData.GetIterator();
            _DataPropertyList.Clear();

            while (pSerialized.NextVisible(true))
            {
                if (pSerialized.name != "<SelectColor>k__BackingField" && pSerialized.name != nameof(_Data.GUIDList))
                {
                    _DataPropertyList.Add(pSerialized.Copy());
                }
            }
        }

        private void ShowParameter()
        {
            GUI.SetNextControlName(m_GuiName);
            foreach (SerializedProperty item in _DataPropertyList)
            {
                EditorGUILayout.PropertyField(item);
                GUILayout.Space(3.5f);
            }
        }

        private void ListenToType()
        {
            GUILayout.Space(10f);

            GUILayout.Label(_Data.TypeName == string.Empty ? $"Please drop an object with the wished type into the window"
                : $"This Bag will store object of the {_Data.TypeName} type");
        }

        private void Create() //TODO CHECK IF FILE PATH VALID
        {
            bool pPlaceable = IsDataPlaceable();
            GUI.backgroundColor = pPlaceable ? Color.green : Color.red;

            if (GUILayout.Button("Create") && pPlaceable)
            {
                CreateData();
            }
        }

        private void CreateData()
        {
            OnValidate();

            AssetDatabase.CreateAsset(_Data, _CurrentPath);
            AssetDatabase.Refresh();
            HistoryBagWindow.instance.UpdateData();

            EditorApplication.delayCall += () =>
            {
                _Data = AssetDatabase.LoadAssetAtPath<Data>(_CurrentPath);
                HistoryBagWindow.instance.UpdateData();
                HistoryBagWindow.CurrentData = _Data;
                HistoryBagWindow.StaticRepaint();
                Close();
            };
        }

        private void GetIconAndType()
        {
            if (_IsListening && _SerializedData != null)
            {
                Object lSelected = Selection.activeObject;
                _SerializedData.FindProperty(nameof(_Data.typeCompleteName)).stringValue = lSelected.GetType().ToString();
                OnValidate();
            }
            if (_IsUpdatingIcon) _Data.logo = GetSelectionTexture();
        }

        private string GetDataFullPath() => HistoryBagWindow.Parameter.GetCurrentBagPath() + "/" + _Data.tabName + ASSET_EXTENSION;

        public void SetType(Object[] pObjectArray)
        {
            Object lObject = pObjectArray[0];
            _Type = lObject.GetType();
            _Data.TypeName = _Type.Name;

            _Data.logo = (Texture2D)AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(lObject));

            foreach (var item in pObjectArray)
            {
                _Data.GUIDList.Add(ObjectToGUID(item));
            }

            SetSeriazableVariable();
        }

        private void OnValidate()
        {
            _SerializedData.ApplyModifiedProperties();
        }
        protected void ListenEnter()
        {
            Event lEvent = Event.current;

            if (GUI.GetNameOfFocusedControl() == m_GuiName)
            {
                CreateData();
            }
        }

        public bool IsDataPlaceable()
            => !File.Exists(_CurrentPath) &&
            Directory.GetParent(_CurrentPath).Exists &&
            _Data.TypeName != string.Empty;
    }
}