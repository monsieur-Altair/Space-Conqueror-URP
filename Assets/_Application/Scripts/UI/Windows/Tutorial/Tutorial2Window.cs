namespace _Application.Scripts.UI.Windows.Tutorial
{
    public class Tutorial2Window : AnimatedWindow
    {
        protected override void StartFadeAnimation()
        {
            _duration = 15.0f;
            base.StartFadeAnimation();
        }
    }
}