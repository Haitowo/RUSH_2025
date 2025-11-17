using UnityEditor;
using UnityEngine;

namespace Editors.History
{
    public abstract class Base<T> : EditorWindow where T : EditorWindow
    {
        public static T Open()
        {
            T lWindow = GetWindow<T>();
            lWindow.titleContent = new GUIContent(nameof(T));
            return lWindow;
        }

        public static T Open(Texture2D pTexture)
        {
            T lWindow = GetWindow<T>();
            lWindow.titleContent = new GUIContent(nameof(T), pTexture);
            return lWindow;
        }

        public static T Open(string pName, Texture2D pTexture)
        {
            T lWindow = GetWindow<T>();
            lWindow.titleContent = new GUIContent(pName, pTexture);
            return lWindow;
        }
    }
}
