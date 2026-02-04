namespace ChioriApp.Models
{
    public class AdminUserItem
    {
        public int ID { get; set; }
        public string Логин { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Телефон { get; set; } = string.Empty;
        public string Имя { get; set; } = string.Empty;
        public string Фамилия { get; set; } = string.Empty;
        public string Отчество { get; set; } = string.Empty;
        public string Роль { get; set; } = string.Empty;
        public string Статус { get; set; } = string.Empty;
        public DateTime Дата_регистрации { get; set; }
    }
}