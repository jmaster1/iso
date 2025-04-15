using Common.Lang.Observable;

namespace Common.Lang.Selection
{
    /// <summary>
    /// AbstractSelection extension that selects many elements
    /// </summary>
    public class MultiSelection<T> : AbstractSelection<T> where T : class
    {
        /// <summary>
        /// currently selected elements
        /// </summary>
        public ObsList<T> Selected = new ObsList<T>();

        public int Count
        {
            get { return Selected.Count; }
        }

        protected override void SelectInternal(T element, bool select)
        {
            if (select)
            {
                Selected.Add(element);
            }
            else
            {
                Selected.Remove(element);
            }
        }

        protected override bool IsSelectedInternal(T element)
        {
            return Selected.Contains(element);
        }

        public override void Clear()
        {
            Selected.Clear();
        }
    }
}