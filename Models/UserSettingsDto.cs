namespace SmartFinance.API.Models
{
    public class UserSettingsDto
    {
        public required string Username { get; set; }
        public required string PreferredCurrency { get; set; }
    }
}
