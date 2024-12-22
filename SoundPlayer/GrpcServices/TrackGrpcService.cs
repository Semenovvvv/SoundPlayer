using Google.Protobuf;
using Grpc.Core;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Services
{
    public class TrackGrpcService : TrackProto.TrackProtoBase
    {
        private readonly ITrackService _trackService;
        private readonly ILogger<TrackGrpcService> _logger;

        public TrackGrpcService(ITrackService trackService, ILogger<TrackGrpcService> logger)
        {
            _trackService = trackService;
            _logger = logger;
        }

        public override async Task<MessageResponse> UploadTrackInfo(TrackInfo request, ServerCallContext context)
        {
            try
            {
                await _trackService.SaveTrackInfo(new TrackDto()
                {
                    Name = request.Name,
                    UserEmail = request.UserEmail,
                    Duration = TimeSpan.FromSeconds(request.DurationInSeconds) 
                });
            }
            catch (Exception e)
            {
                return new MessageResponse
                {
                    Message = $"Fail",
                    Success = false
                };
            }

            return new MessageResponse
            {
                Message = $"Success",
                Success = true
            };
        }

        public override async Task<MessageResponse> UploadTrackChunks(IAsyncStreamReader<AudioChunk> requestStream, ServerCallContext context)
        {
            try
            {
                var trackGuid = Guid.NewGuid();

                await foreach (var chunk in requestStream.ReadAllAsync())
                {
                    await _trackService.SaveTrackChunkInDirectory(new TrackChunk()
                    {
                        Data = chunk.Data.ToByteArray(),
                        IsFinishChunk = chunk.IsFinalChunk
                    }, trackGuid);
                }

                return new MessageResponse
                {
                    Message = $"File uploaded successfully.",
                    Success = true
                };
            }
            catch
            {
                return new MessageResponse
                {
                    Message = "Failed to upload the file.",
                    Success = false
                };
            }
        }

        public override async Task<TrackInfo> GetTrackInfo(TrackId request, ServerCallContext context)
        {
            var response = await _trackService.GetTrackInfo(request.Id);
            var trackDto = response.Result;
            var trackInfo = new TrackInfo()
            {
                Name = trackDto.Name,
                UserEmail = trackDto.UserEmail
            };

            return trackInfo;
        }

        public override async Task GetTrackChunks(TrackId request, IServerStreamWriter<AudioChunk> responseStream, ServerCallContext context)
        {
            try
            {
                await _trackService.GetTrackChunks(
                    request.Id,
                    async chunk =>
                    {
                        var trackChunk = new TrackChunk
                        {
                            Data = chunk
                        };

                        await responseStream.WriteAsync(new AudioChunk()
                        {
                            Data = ByteString.CopyFrom(trackChunk.Data),
                            FileName = "",
                            IsFinalChunk = trackChunk.IsFinishChunk

                        });
                    });
            }
            catch (FileNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Ошибка передачи трека: {ex.Message}"));
            }
        }

        public override async Task<GetTracksResponse> GetTrackList(GetTracksRequest request, ServerCallContext context)
        {
            var result = await _trackService.GetTrackListByName(request.TrackName, request.PageNumber, request.PageSize);

            if (!result.IsSuccess)
            {
                throw new RpcException(new Status(StatusCode.Internal, result.Message));
            }

            return new GetTracksResponse
            {
                TotalCount = result.Result.TotalCount,
                PageNumber = result.Result.PageNumber,
                PageSize = result.Result.PageSize,
                Tracks = { result.Result.Items.Select(t => new TrackEntity
                {
                    Id = t. Id,
                    Name = t.Name,
                    UserEmail = t.UserEmail,
                    UserName = t.UserName,
                    Duration = (int)t.Duration.TotalSeconds,
                }) }
            };
        }
    }
}
