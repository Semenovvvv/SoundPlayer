using Grpc.Core;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Services
{
    public class AuthGrpcService : AuthProto.AuthProtoBase
    {
        private readonly IAuthService _authService;
        public AuthGrpcService(IAuthService authService)
        {
            _authService = authService;
        }
        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            var success = await _authService.RegisterUser(new UserDto
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password
            });

            return new RegisterResponse { Success = success };
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            var token = await _authService.LoginUser(new LoginDto
            {
                Email = request.Email,
                Password = request.Password
            });

            return new LoginResponse { Token = token };
        }
    }
}
