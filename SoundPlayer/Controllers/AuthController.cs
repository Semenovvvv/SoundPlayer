using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Controllers
{
    public class AuthController : AuthProto.AuthProtoBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            var response = await _authService.RegisterUser(new UserDto
            {
                Login = request.Username,
                Email = request.Email,
                Password = request.Password
            });

            return new RegisterResponse { Success = response.IsSuccess };
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            var response = await _authService.LoginUser(new UserDto
            {
                Email = request.Email,
                Password = request.Password
            });

            var user = response.Result.Item1;
            var token = response.Result.Item2;

            if (user == null || token == null)
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            
            return new LoginResponse
            {
                IsSuccess = response.IsSuccess,
                Token = token,
                User = new User()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Login = user.Login,
                    CreatedAt = Timestamp.FromDateTime(user.CreatedAt)
                }
            };
        }
    }
}
