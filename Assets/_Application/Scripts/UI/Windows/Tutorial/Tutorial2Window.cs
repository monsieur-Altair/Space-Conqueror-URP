namespace _Application.Scripts.UI.Windows.Tutorial
{
    public class Tutorial2Window : AnimatedWindow
    {
        protected override void StartFadeOutAnimation()
        {
            _duration = 15.0f;
            base.StartFadeOutAnimation();
        }
    }
}