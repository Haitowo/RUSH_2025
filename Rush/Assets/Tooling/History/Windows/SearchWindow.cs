using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Editors.History.Utils;

namespace Editors.History
{
    public class SearchWindow : BaseDisplayButtonWindow<Object>
    {
        private Vector2 _ScrollPos = new Vector2();

        private bool _CloseIfSelected = false,
            _ShowEntirePath = false,
            _OpenIfSelected = true;

        private bool _IsShowingPreview = true;

        private string _AssetTypeString = "Default";

        private const string DEFAULT_PATH = "Assets";
        private string[] _FoldersToSearch = new string[1] { DEFAULT_PATH };

        private string _SearchedItem = string.Empty;

        public event System.Action<Object[]> onObjectSelected;

        string _StartString = string.Empty;

        private List<Object> _ObjectList = new List<Object>();
        private float _IconSize = 82f;

        private static Texture2D _LoopTexture;

        private static GUIStyle _LabelStyle;
        private GUIStyle _ButtonStyle;

        public static SearchWindow Open(Data pHistoryData, bool pCloseIfChosen = false, bool pShowEntirePath = false, bool pOpenIfSelected = true)
        {
            SearchWindow lWindow = GetWindow<SearchWindow>(pHistoryData.TypeName);
            lWindow.titleContent = new GUIContent($"Search/{pHistoryData.TypeName}", pHistoryData.logo);

            lWindow._AssetTypeString = T_SEARCH + pHistoryData.TypeName.ToLower();
            lWindow.SetWindow(pCloseIfChosen, pShowEntirePath, pOpenIfSelected);

            SetLabelStyle();
            _LoopTexture = (Texture2D)EditorGUIUtility.IconContent(SEARCH_ICON).image;

            lWindow.SetButtonStyle();
            lWindow.LoadObject();
            lWindow.Show();

            return lWindow;
        }

        private void SetButtonStyle()
        {
            _ButtonStyle = new GUIStyle(GUI.skin.button);
            _ButtonStyle.normal.background = Texture2D.whiteTexture;
            _ButtonStyle.normal.textColor = Color.white;
        }

        private void SetWindow(bool pCloseIfChosen, bool pShowEntirePath, bool pOpenIfSelected)
        {
            _OpenIfSelected = pOpenIfSelected;
            _CloseIfSelected = pCloseIfChosen;
            _ShowEntirePath = pShowEntirePath;
        }

        private static void SetLabelStyle()
        {
            _LabelStyle = new GUIStyle(EditorStyles.label);
            _LabelStyle.alignment = TextAnchor.MiddleCenter;
            _LabelStyle.fontSize = 10;
        }

        private void OnGUI()
        {
            SetSearchString();
            ShowUI();
            DisplayButton();

            if (_StartString != _SearchedItem)
            {
                _StartString = _SearchedItem;
                LoadObject();
                Repaint();
            }

            ListenEnter();
        }

        private void SetSearchString()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(_LoopTexture, GUILayout.Width(20f), GUILayout.Height(20f));
            GUI.SetNextControlName(m_GuiName);
            _SearchedItem = GUILayout.TextField(_SearchedItem);
            FocusControl();

            GUILayout.EndHorizontal();
        }

        private void ShowUI()
        {
            GUILayout.BeginHorizontal();
            _IconSize = EditorGUILayout.Slider(_IconSize, 32f, 128f);
            _ShowEntirePath = GUILayout.Toggle(_ShowEntirePath, "ShowEntirePath");
            _IsShowingPreview = GUILayout.Toggle(_IsShowingPreview, "Show Preview");
            GUILayout.EndHorizontal();
        }

        private void DisplayButton()
        {
            Object lObject;
            Texture2D lTexture;
            Rect lRect;
            bool lBool;

            GUILayout.Space(10);

            _ScrollPos = GUILayout.BeginScrollView(_ScrollPos);

            int lColumn = Mathf.FloorToInt(position.width / (_IconSize + 4));
            int lTotal = _ObjectList.Count;

            for (int i = 0; i < lTotal; i += lColumn)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < lColumn; j++)
                {
                    int lIndex = i + j;
                    if (lIndex >= lTotal)
                    {
                        GUILayout.Space(_IconSize + 2);
                        continue;
                    }
                    lObject = _ObjectList[lIndex];

                    lBool = i + j == 0 && !m_FolderSelectable.IsSelecting || m_FolderSelectable.SelectList.Contains(lObject);

                    lTexture = _IsShowingPreview ? TryGetAssetPreview(lObject) : AssetPreview.GetMiniThumbnail(lObject);

                    GUILayout.BeginVertical(GUILayout.Width(_IconSize));

                    lRect = GUILayoutUtility.GetRect(new GUIContent(TryGetAssetPreview(lObject)), GUI.skin.button, GUILayout.Width(_IconSize), GUILayout.Height(_IconSize));

                    DrawBoxWithIndepedantTexture(lRect, lObject, lTexture, lBool ? GUI.skin.settings.selectionColor : Color.white);
                    ManageEventOnRect(lObject, lRect);

                    string lLabel = _ShowEntirePath ? AssetDatabase.GetAssetPath(lObject) : lObject.name;
                    GUILayout.Label(lLabel, _LabelStyle, GUILayout.Width(_IconSize));

                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        protected void ListenEnter()
        {
            Event lEvent = Event.current;

            if (lEvent.keyCode == KeyCode.Return)
            {
                if (_ObjectList.Count >= 1) HandleCondition(_ObjectList[0]);
                lEvent.Use();
            }
        }

        protected override void OnControlClick(Object pObject) => m_FolderSelectable.TryAddByControl(pObject);

        protected override void OnShiftClick(Object pObject) => m_FolderSelectable.TryAddByShift(pObject, _ObjectList);
        protected override void OnClick(Object pObject)
        {
            HandleCondition(pObject);
        }

        protected override void OnLeftClick(Object pObject)
        {

        }

        protected override void OnMouseDrag(Object pObject)
        {
            
        }


        private void HandleCondition(Object pObject)
        {
            if (_OpenIfSelected) EditorGUIUtility.PingObject(pObject);
            onObjectSelected?.Invoke(m_FolderSelectable.IsSelecting ? m_FolderSelectable.SelectList.ToArray() : new Object[1] { pObject });

            HistoryBagWindow.StaticRepaint();
            if (_CloseIfSelected) Close();
        }

        private void DrawBoxWithIndepedantTexture(Rect pRect, Object pObject, Texture2D pTexture, Color pColor)
        {
            GUI.color = pColor;
            GUI.Box(pRect, GUIContent.none, _ButtonStyle);
            GUI.color = Color.white;

            GUI.DrawTexture(pRect, pTexture, ScaleMode.ScaleToFit, true);

            //Color[] lColor = pTexture.GetPixels(); //TODO COMPATIBLE WITH PREVIEW
            //foreach (var item in lColor)
            //{
            //    if (item.a == 0)
            //    {
            //        Debug.Log(pObject.name);
            //        break;
            //    }
            //}
        }

        private void LoadObject()
        {
            _ObjectList.Clear();
            m_FolderSelectable.ClearSelectedList();
            string lPath;
            string lName;
            string[] lPathArray = AssetDatabase.FindAssets(_AssetTypeString, _FoldersToSearch);

            foreach (string item in lPathArray)
            {
                lPath = AssetDatabase.GUIDToAssetPath(item);
                lName = Path.GetFileNameWithoutExtension(lPath);

                if (_SearchedItem == string.Empty || lName.Contains(_SearchedItem, System.StringComparison.OrdinalIgnoreCase))
                    _ObjectList.Add(AssetDatabase.LoadAssetAtPath<Object>(lPath));
            }

            Object lObject;
            foreach (string item in HistoryBagWindow.CurrentData.GUIDList)
            {
                lObject = GUIDToObject(item);
                if (_ObjectList.Contains(lObject)) _ObjectList.Remove(lObject);
            }
        }
    }
}