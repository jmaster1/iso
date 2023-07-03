using Common.Lang.Observable;
using Newtonsoft.Json;

namespace Common.Lang.Selection
{
    /// <summary>
    /// AbstractSelection extension that selects one element at a time
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SingleSelection<T> : AbstractSelection<T> where T : class
    {
        /// <summary>
        /// currently selected element
        /// </summary>
        [JsonProperty]
        public readonly Holder<T> Selected = new Holder<T>();
        
        public T SelectedValue => Selected.Get();

        /// <summary>
        /// if true, will automatically select something when nothing selected (if possible)
        /// </summary>
        public bool Autoselect;

        protected override void OnBind()
        {
            base.OnBind();
            TryAutoselect();
        }

        protected override void OnListChange(ObsListEvent type, ObsListEventData<T> data)
        {
            base.OnListChange(type, data);
            switch (type)
            {
                case ObsListEvent.AddAfter:
                    TryAutoselect();
                    break;
            }
        }

        /// <summary>
        /// try to select any element
        /// </summary>
        /// <returns>true if selected, false otherwise</returns>
        public bool TryAutoselect()
        {
            if (Autoselect && (Selected.IsNull() || Model.IsRemovingElement(Selected.Get())) && Model.Count > 0)
            {
                for (var i = 0; i < Model.Count; i++)
                {
                    if(Model.IsRemovingIndex(i)) continue;
                    var val = Model[i];
                    Select(val);
                    return true;
                }
            }

            return false;
        }

        protected override void SelectInternal(T element, bool select)
        {
            //
            // cleanup selected flag for all elements, but selected
            if (HolderFunc != null)
            {
                foreach (var e in Model)
                {
                    if(e == element && select) continue;
                    SetSelected(e, false);
                }
            }

            if (!select)
            {
                if (!TryAutoselect())
                {
                    Selected.SetDefault();
                }
            }
            else
            {
                Selected.Set(element);
            }
        }

        protected override bool IsSelectedInternal(T element)
        {
            return Selected.Is(element);
        }

        public override void Clear()
        {
            Selected.Clear();
        }

        /// <summary>
        /// get currently selected element
        /// </summary>
        public T Get()
        {
            return Selected.Get();
        }
    }
}
