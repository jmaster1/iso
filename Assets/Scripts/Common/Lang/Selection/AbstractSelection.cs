using System;
using Common.Bind;
using Common.Lang.Observable;

namespace Common.Lang.Selection
{
    /// <summary>
    /// capable of maintaining selected objects from list
    /// </summary>
    public abstract class AbstractSelection<T> : BindableBean<ObsList<T>> where T : class
    {
        /// <summary>
        /// optional function that returns selected flag holder for element,
        /// if set, will be used to update selected flag 
        /// </summary>
        public Func<T, BoolHolder> HolderFunc;

        protected override void OnBind()
        {
            Model.AddListener(OnListChange);
        }
        
        protected override void OnUnbind()
        {
            Model.RemoveListener(OnListChange);
            Clear();
        }

        protected virtual void OnListChange(ObsListEvent type, ObsListEventData<T> data)
        {
            switch (type)
            {
                case ObsListEvent.RemoveBefore:
                    if (IsSelected(data.Element))
                    {
                        Unselect(data.Element);
                    }
                    break;
                case ObsListEvent.ClearBefore:
                    Clear();
                    break;
            }
        }

        /// <summary>
        /// select given item (must not be null)
        /// </summary>
        /// <returns>true if selection changed</returns>
        public bool Select(T element, bool select = true)
        {
            AssertBound();
            Assert(element != null && Model.Contains(element));
            if (IsSelected(element) == select) return false;
            SetSelected(element, select);
            SelectInternal(element, select);
            return true;
        }
        
        /// <summary>
        /// unselect given item (must not be null)
        /// </summary>
        public void Unselect(T element)
        {
            Select(element, false);
        }
        
        protected abstract void SelectInternal(T element, bool select);

        public bool IsSelected(T element)
        {
            if (HolderFunc == null) return IsSelectedInternal(element);
            var holder = HolderFunc(element);
            return holder.Get();
        }
        
        protected void SetSelected(T element, bool selected)
        {
            if(HolderFunc == null) return;
            var holder = HolderFunc(element);
            holder.Set(selected);
        }

        protected abstract bool IsSelectedInternal(T element);
        
        public void SelectNone()
        {
            for (var i = 0; i < Model.Count; i++)
            {
                var e = Model[i];
                if(IsSelected(e)) Select(e, false);
            }
        }
    }
}