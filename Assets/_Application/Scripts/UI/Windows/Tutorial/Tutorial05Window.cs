namespace _Application.Scripts.UI.Windows.Tutorial
{
    public class Tutorial05Window : AnimatedWindow
    {
        protected override void StartFadeOutAnimation()
        {
            _duration = 7.5f;
            base.StartFadeOutAnimation();
        }
    }
}