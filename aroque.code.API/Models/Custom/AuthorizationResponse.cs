namespace aroque.code.API.Models.Custom
{
    public class AuthorizationResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Response { get; set; }
        public string Message { get; set; }
    }
}
