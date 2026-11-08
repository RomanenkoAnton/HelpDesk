using HelpDesk.API.Data.DTO;

namespace HelpDesk.API.Repository.IRepository
{
    public interface IAuthRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginRequestDTO> Authenticate(LoginRequestDTO loginRequesDTO);
        Task<UserDTO> Register(RegisterationRequestDTO requestDTO);
    }
}
