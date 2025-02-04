using Grpc.Core;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Controllers
{
    public class UserController : UserProto.UserProtoBase
    {
        private IUserService _userService;
        
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        public override async Task<UserProfileResponse> GetUserProfile(UserProfileRequest request, ServerCallContext context)
        {
            var result = await _userService.GetUserById(request.UserId);

            return new UserProfileResponse()
            {
                UserId = result?.Id ?? 0,
                Email = result?.Email ?? "" ,
                Username = result?.UserName ?? ""
            };
        }

        public override async Task<UpdateUserProfileResponse> UpdateUserProfile(UpdateUserProfileRequest request, ServerCallContext context)
        {
            var dto = new UpdateUserDto()
            {
                Email = request.NewEmail,
                Username = request.NewUsername
            };
            
            var result = await _userService.UpdateUserProfile(request.UserId, dto);

            return new UpdateUserProfileResponse()
            {
                Success = result.IsSuccess
            };
        }
    }
}
