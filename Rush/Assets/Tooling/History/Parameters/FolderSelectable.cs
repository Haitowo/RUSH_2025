using System.Collections.Generic;
using System.Text;

namespace Editors.History
{
    public class FolderSelectable<T>
    {
        public List<T> SelectList = new List<T>();
        public bool IsSelecting { get; private set; } = false;

        public void TryAddByShift(T pT, IList<T> pOwningList)
        {
            if (IsSelecting)
            {
                int lObjectIndex = pOwningList.IndexOf(pT);
                int lMaxIndex = -1;
                int lCurrentIndex;

                for (int i = 0; i < SelectList.Count; i++)
                {
                    if (pOwningList.Contains(SelectList[i]))
                    {
                        lCurrentIndex = pOwningList.IndexOf(SelectList[i]);
                        if (lMaxIndex < lCurrentIndex) lMaxIndex = lCurrentIndex;
                    }
                }

                int lStart, lIteration;

                if (lMaxIndex > lObjectIndex)
                {
                    lStart = lObjectIndex;
                    lIteration = lMaxIndex;
                }
                else
                {
                    lStart = lMaxIndex;
                    lIteration = lObjectIndex;
                }

                for (int i = lStart; i <= lIteration; i++)
                {
                    TryAdd(pOwningList[i]);
                }
            }
            else TryAdd(pT);

            IsSelecting = true;
        }

        public void TryAddByControl(T pT)
        {
            if (SelectList.Contains(pT)) SelectList.Remove(pT);
            else SelectList.Add(pT);

            IsSelecting = SelectList.Count > 0;
        }

        private void TryAdd(T pT)
        {
            if (!SelectList.Contains(pT)) SelectList.Add(pT);
        }

        public void ClearSelectedList()
        {
            SelectList.Clear();
            IsSelecting = false;
        }

        /// <summary>
        /// Debug function to show the Selected Object from the pEnumerable, Selected object will be
        /// shown as such |myObject|
        /// </summary>
        public string SelectObjectComparaison(IEnumerable<T> pEnumerable)
        {
            StringBuilder lStringBuilder = new StringBuilder();
            foreach (T item in pEnumerable)
            {
                lStringBuilder.Append(SelectList.Contains(item) ? $"|{item}| " : $"{item} ");
            }
            return lStringBuilder.ToString();
        }
    }

}