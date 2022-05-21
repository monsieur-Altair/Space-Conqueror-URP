namespace _Application.Scripts.UI.Windows.Tutorial
{
    public class Tutorial5Window : AnimatedWindow
    {
        protected override void StartFadeAnimation()
        {
            _duration = 10.0f;
            base.StartFadeAnimation();
        }
    }
}