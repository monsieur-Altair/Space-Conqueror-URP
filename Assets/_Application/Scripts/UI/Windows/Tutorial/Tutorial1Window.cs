namespace _Application.Scripts.UI.Windows.Tutorial
{
    public class Tutorial1Window : AnimatedWindow
    {
        protected override void StartFadeAnimation()
        {
            _duration = 7.5f;
            base.StartFadeAnimation();
        }
    }
}