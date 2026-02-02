using Microsoft.EntityFrameworkCore;

namespace ChioriApp
{
    public static class AppConfig
    {
        public static string GetConnectionString()
        {
            return "Host=localhost;Database=chiori_company;Username=XIII;Password=123";
        }
    }
}