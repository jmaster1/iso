using Common.Unity.Bind;

namespace Iso.Unity.World
{
    public class MovablesView : BindableMono<Movables.Movables>
    {

        public MovableListAdapter listAdapter;
        
        public override void OnBind()
        {
            base.OnBind();
            BindBindable(Model.List, listAdapter);
        }
    }
}
