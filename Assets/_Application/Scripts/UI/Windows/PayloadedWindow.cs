namespace _Application.Scripts.UI.Windows
{
    public abstract class PayloadedWindow<TPayload> : Window
    {
        protected TPayload _payload;

        public void Open(TPayload payload)
        {
            _payload = payload;
            Open();
        }

    }
}