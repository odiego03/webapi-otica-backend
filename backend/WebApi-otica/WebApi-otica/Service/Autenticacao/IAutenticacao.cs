namespace WebApi_otica.Service.Autenticacao
{
    public interface IAutenticacao
    {
        Task<bool> RegisterUser(string email, string password);
        Task<bool> Authenticate(string email, string password); // Retorna o token JWT
        Task Logout();
    }
}
