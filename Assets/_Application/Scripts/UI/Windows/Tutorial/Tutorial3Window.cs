﻿namespace _Application.Scripts.UI.Windows.Tutorial
{
    public class Tutorial3Window : AnimatedWindow
    {
        protected override void StartFadeAnimation()
        {
            _duration = 10.0f;
            base.StartFadeAnimation();
        }
    }
}