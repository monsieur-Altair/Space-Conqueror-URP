namespace _Application.Scripts.Infrastructure.Services
{
    public class AllServices
    {
        public static TService Get<TService>() where TService:IService => 
            Implementation<TService>.ServiceInstance;

        public static TService Register<TService>(TService implementation) where TService:IService => 
            Implementation<TService>.ServiceInstance = implementation;

        private static class Implementation<TService> where TService:IService
        {
            public static TService ServiceInstance;
        }
    }
}