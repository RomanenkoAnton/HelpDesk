using AutoMapper;
using HelpDesk.API.Data;
using HelpDesk.API.Data.DTO;
using HelpDesk.API.Models;
using HelpDesk.API.Repository.IRepository;

namespace HelpDesk.API.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public AuthRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.LocalUsers.FirstOrDefault(x =>x.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }
        public Task<LoginRequestDTO> Authenticate(LoginRequestDTO loginRequesDTO)
        {
            
        }



        public async Task<UserDTO> Register(RegisterationRequestDTO requestDTO)
        {
            LocalUser userObj = new()
            {
                UserName = requestDTO.UserName,
                Password = requestDTO.Password,
                Name = requestDTO.Name,
                Role = "admin"
            };

            _db.LocalUsers.Add(userObj);
            _db.SaveChanges();
            userObj.Password = "";
            return _mapper.Map<UserDTO>(userObj);
        }
    }
}
