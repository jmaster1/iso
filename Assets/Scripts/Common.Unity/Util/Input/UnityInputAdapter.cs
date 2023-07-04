using Common.Api.Input;
using Common.Bind;

namespace Common.Unity.Util.Input
{
    /// <summary>
    /// capable of blocking unity input according to InputApi lock,
    /// this requires FilteredStandaloneInputModule component in scene
    /// </summary>
    public class UnityInputAdapter : BindableBean<FilteredStandaloneInputModule>
    {
        private InputApi inputApi = GetBean<InputApi>();

        protected override void OnBind()
        {
            Model.RaycastResultFilter += OnInputFilter;
        }

        protected override void OnUnbind()
        {
            Model.RaycastResultFilter -= OnInputFilter;
        }

        private void OnInputFilter(object sender, RaycastResultEventArgs args)
        {
            if (inputApi.Lock.Get())
            {
                args.res.Clear();
            }
        }
    }
}