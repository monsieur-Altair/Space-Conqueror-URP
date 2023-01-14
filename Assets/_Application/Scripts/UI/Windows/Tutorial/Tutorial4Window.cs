namespace _Application.Scripts.UI.Windows.Tutorial
{
    public class Tutorial4Window : AnimatedWindow
    {
        protected override void StartFadeOutAnimation()
        {
            _duration = 7.5f;
            base.StartFadeOutAnimation();
        }
    }
}