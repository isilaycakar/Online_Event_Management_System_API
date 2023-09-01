namespace OEMS_API.Models
{
    public class SignUpModel
    {
        public string NameSurname { get; set; }
        public string Username { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
