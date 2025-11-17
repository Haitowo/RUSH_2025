using UnityEditor;
using UnityEngine;

namespace Editors.History
{
    public abstract class BaseEditor<T> : Editor where T : Object
    {
        protected T m_Target;
        protected void SetTarget() => m_Target = (T)target;
    }
}