namespace SimpleAPI.Dto.User
{
    public class AccessRequest
    {
        public string UserEmail {  get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public string GrantType { get; set; }
    }
}
