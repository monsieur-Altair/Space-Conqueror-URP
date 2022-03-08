namespace _Application.Scripts.Infrastructure.Services
{
    public class AllServices
    {
        private static AllServices _instance;
        public static AllServices Instance => _instance ??= new AllServices();

        public TService GetSingle<TService>() where TService:IService => 
            Implementation<TService>.ServiceInstance;

        public void RegisterSingle<TService>(TService implementation) where TService:IService => 
            Implementation<TService>.ServiceInstance = implementation;

        private static class Implementation<TService> where TService:IService
        {
            public static TService ServiceInstance;
        }
    }
}