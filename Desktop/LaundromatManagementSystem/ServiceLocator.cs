namespace LaundromatManagementSystem
{
    public static class ServiceLocator
    {
        public static T? GetService<T>() where T : class
        {
            return App.Services?.GetService<T>();
        }
    }
}