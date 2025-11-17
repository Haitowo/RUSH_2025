using UnityEditor;
using UnityEngine;
using System.Security.Cryptography;

namespace Editors.History
{
    public abstract class FocusableWindow : EditorWindow
    {
        protected string m_GuiName = SHA256.Create().ToString();
        private bool _WaitFocus = true;

        protected virtual void FocusControl()
        {
            if (_WaitFocus)
            {
                GUI.FocusControl(m_GuiName);
                _WaitFocus = false;
            }
        }
    }
}