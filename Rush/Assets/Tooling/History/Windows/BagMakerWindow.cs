using static Editors.History.Utils;
using System.Security.Cryptography;
using Editors.History.Parameters;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Editors.History
{
    public class BagMakerWindow : FocusableWindow
    {
        private string _TabName = "Default";
        private string _CategoryName = string.Empty;
        private string[] _NameArray;
        private int _CurrentIndex = 0;

        private HistoryParameter _Parameter;
        private string _NewTabName = SHA256.Create().ToString();

        public static BagMakerWindow Open()
        {
            BagMakerWindow lWindow = GetWindow<BagMakerWindow>();
            lWindow.titleContent = new GUIContent("BagMaker", (Texture2D)EditorGUIUtility.IconContent(BAG_ICON).image);
            return lWindow;
        }

        private void CreateGUI()
        {
            _Parameter = GetHistoryParameter();
            SetCurrentFolderArborescence(_Parameter.savePath);
        }

        private void OnGUI()
        {
            ShowName();

            ManageArborescence();
            CreateCategories();
            CreateBag();
            ListenEnter();
        }

        private void CreateBag()
        {
            bool lIsValid = IsNameValid(_TabName) && IsTabPathValid(_TabName);

            GUI.backgroundColor = lIsValid ? Color.green : Color.red;

            if (GUILayout.Button("Create Tab"))
            {
                if (lIsValid)
                {
                    CreateTab();
                }
            }
            GUI.backgroundColor = Color.white;
        }

        private void ShowName()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tab Name");

            GUI.SetNextControlName(m_GuiName);

            _TabName = GUILayout.TextField(_TabName);
            FocusControl();
            GUILayout.EndHorizontal();
        }

        private void CreateCategories()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Create New Category");

            GUI.SetNextControlName(_NewTabName);
            _CategoryName = GUILayout.TextField(_CategoryName);
            bool lIsValid = IsNameValid(_CategoryName) && IsCategoryPathValid(_CategoryName);

            GUI.backgroundColor = lIsValid ? Color.green : Color.red;
            if (GUILayout.Button(new GUIContent((Texture2D)EditorGUIUtility.IconContent(PLUS_WHITE_ICON).image)) && lIsValid)
            {
                CreateCategory();
            }
            GUILayout.EndHorizontal();
        }

        private void ManageArborescence()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Categories");
            _CurrentIndex = EditorGUILayout.Popup(_CurrentIndex, _NameArray);
            GUILayout.EndHorizontal();
        }

        private void SetCurrentFolderArborescence(string pPath)
        {
            string[] lDirectoryArray = Directory.GetDirectories(pPath);
            _NameArray = new string[lDirectoryArray.Length];

            for (int i = 0; i < lDirectoryArray.Length; i++)
            {
                _NameArray[i] = Path.GetFileName(lDirectoryArray[i]);
            }
        }

        private bool IsNameValid(string pName)
        {
            foreach (char item in Path.GetInvalidFileNameChars())
            {
                if (pName.Contains(item)) return false;
            }
            return true;
        }

        private bool IsCategoryPathValid(string pName) => !Directory.Exists(_Parameter.savePath + "/" + pName) && pName != string.Empty;
        private bool IsTabPathValid(string pName) => !Directory.Exists(_Parameter.savePath + "/" + _NameArray[_CurrentIndex] + "/" + pName) && pName != string.Empty;

        private void CreateTab()
        {
            AssetDatabase.CreateFolder(_Parameter.savePath + "/" + _NameArray[_CurrentIndex], _TabName);
            AssetDatabase.Refresh();
            HistoryBagWindow.instance.SetDataBag(_Parameter.savePath + "/" + _NameArray[_CurrentIndex] + "/" + _TabName);
            Debug.Log(_Parameter.savePath + "/" + _NameArray[_CurrentIndex] + "/" + _TabName+" CreateTab ");

            Close();
        }

        private void CreateCategory()
        {
            AssetDatabase.CreateFolder(_Parameter.savePath, _CategoryName);
            AssetDatabase.Refresh();

            SetCurrentFolderArborescence(_Parameter.savePath);
            _CurrentIndex = System.Array.IndexOf(_NameArray, _CategoryName);
            _CategoryName = string.Empty;
        }

        private void ListenEnter()
        {
            Event lEvent = Event.current;

            if (lEvent.keyCode == KeyCode.Return)
            {
                string lName = GUI.GetNameOfFocusedControl();
                if (lName == m_GuiName) CreateTab();
                else if (lName == _NewTabName)
                {
                    CreateCategory();
                    GUI.FocusControl(m_GuiName);
                }
            }
        }
    }
}