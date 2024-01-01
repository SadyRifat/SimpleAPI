namespace SimpleAPI.Dto.User
{
    public class Token
    {
        public string AccessToken { get; set; }
        public DateTime ExpireAt { get; set; }
        public string RefreshToken { get; set; }

        public Token(string accessToken, DateTime expireAt, string refreshToken)
        {
            AccessToken = accessToken;
            ExpireAt = expireAt;
            RefreshToken = refreshToken;
        }
    }
}
