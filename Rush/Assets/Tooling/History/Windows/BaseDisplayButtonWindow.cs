using UnityEngine;

namespace Editors.History
{
    public abstract class BaseDisplayButtonWindow<T> : FocusableWindow
    {
        protected FolderSelectable<T> m_FolderSelectable = new FolderSelectable<T>();

        protected void ManageEventOnRect(Object pObject, Rect pRect)
        {
            Event lEvent = Event.current;

            if (pRect.Contains(lEvent.mousePosition))
            {
                switch (lEvent.type)
                {
                    case EventType.MouseUp:

                        if (lEvent.button == 0)
                        {
                            if (Event.current.control) OnControlClick(pObject);
                            else if (Event.current.shift) OnShiftClick(pObject);
                            else OnClick(pObject);
                        }
                        else if (lEvent.button == 1)
                        {
                            OnLeftClick(pObject);
                        }
                        break;

                    case EventType.MouseDrag:
                        OnMouseDrag(pObject);
                        break;
                    default:
                        break;
                }
            }
        }

        protected abstract void OnControlClick(Object pObject);
        protected abstract void OnShiftClick(Object pObject);
        protected abstract void OnClick(Object pObject);
        protected abstract void OnLeftClick(Object pObject);
        protected abstract void OnMouseDrag(Object pObject);
    }
}