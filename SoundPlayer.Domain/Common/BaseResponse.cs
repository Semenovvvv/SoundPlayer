namespace SoundPlayer.Domain.Common
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }

        public BaseResponse() { }

        public BaseResponse(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
        
        public BaseResponse(bool isSuccess, string? message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }

    public class BaseResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Result { get; set; }
    }
}
