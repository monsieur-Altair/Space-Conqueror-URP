namespace _Application.Scripts.UI.Windows.Tutorial
{
    public class Tutorial05Window : AnimatedWindow
    {
        protected override void StartFadeAnimation()
        {
            _duration = 7.5f;
            base.StartFadeAnimation();
        }
    }
}