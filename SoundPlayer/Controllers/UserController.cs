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
        
        public override async Task<UserResponse> GetUserById(UserRequest request, ServerCallContext context)
        {
            var result = await _userService.GetUserById(request.UserId);

            return new UserResponse()
            {
                UserId = result?.Id ?? 0,
                Email = result?.Email ?? "" ,
                Username = result?.UserName ?? ""
            };
        }

        public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            var dto = new UpdateUserDto()
            {
                Email = request.NewEmail,
                Username = request.NewUsername
            };
            
            var result = await _userService.UpdateUserProfile(request.UserId, dto);

            return new UpdateUserResponse()
            {
                Success = result.IsSuccess
            };
        }

        public override async Task<GetUserResponse> GetUserList(GetUserRequest request, ServerCallContext context)
        {
            var userNamePattern = request.UserName;
            var pageSize = request.PageSize;
            var pageNumber = request.PageNumber;

            var response = await _userService.GetUsersByName(userNamePattern, pageNumber, pageSize);

            if (!response.IsSuccess)
            {
                throw new RpcException(new Status(StatusCode.Unknown, response.Message));
            }


            var result = new GetUserResponse()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = response.TotalCount,
                Users =
                {
                    response.Items.Select(user => new UserMetadata()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        UserEmail = user.Email
                    })
                }
            };
            
            return result;
        }
    }
}
