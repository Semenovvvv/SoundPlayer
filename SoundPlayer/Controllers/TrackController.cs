using Google.Protobuf;
using Grpc.AspNetCore.Web;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Controllers
{
    public class TrackController : TrackProto.TrackProtoBase
    {
        private readonly ITrackService _trackService;
        private readonly ILogger<TrackController> _logger;

        public TrackController(ITrackService trackService, ILogger<TrackController> logger)
        {
            _trackService = trackService;
            _logger = logger;
        }

        //public override async Task<MessageResponse> UploadTrackInfo(TrackInfo request, ServerCallContext context)
        //{
        //    try
        //    {
        //        await _trackService.SaveTrackInfo(new TrackDto()
        //        {
        //            Name = request.Name,
        //            UserId = request.UserId,
        //            Duration = TimeSpan.FromSeconds(request.DurationInSeconds) 
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        return new MessageResponse
        //        {
        //            Message = $"Fail",
        //            Success = false
        //        };
        //    }

        //    return new MessageResponse
        //    {
        //        Message = $"Success",
        //        Success = true
        //    };
        //}

        //public override async Task<MessageResponse> UploadTrackChunks(IAsyncStreamReader<AudioChunk> requestStream, ServerCallContext context)
        //{
        //    try
        //    {
        //        var trackGuid = Guid.NewGuid();

        //        await foreach (var chunk in requestStream.ReadAllAsync())
        //        {
        //            await _trackService.SaveTrackChunkInDirectory(new TrackChunk()
        //            {
        //                Data = chunk.Data.ToByteArray(),
        //                IsFinishChunk = chunk.IsFinalChunk
        //            }, trackGuid);
        //        }

        //        return new MessageResponse
        //        {
        //            Message = $"File uploaded successfully.",
        //            Success = true
        //        };
        //    }
        //    catch
        //    {
        //        return new MessageResponse
        //        {
        //            Message = "Failed to upload the file.",
        //            Success = false
        //        };
        //    }
        //}

        //[Authorize(Roles = "User")]
        public override async Task<TrackInfo> GetTrackInfo(TrackId request, ServerCallContext context)
        {
            var response = await _trackService.GetTrackInfo(request.Id);
            var trackDto = response.Result;
            var trackInfo = new TrackInfo()
            {
                Name = trackDto.Name,
                UserId = trackDto.UserId
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

        [Authorize(Policy = "Admin")]
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

        //public override Task<UploadTrackResponse> UploadTrack(IAsyncStreamReader<UploadTrackRequest> requestStream, ServerCallContext context)
        //{
        //    try
        //    {
        //        var (trackInfo, tempFilePath) = await _fileService.SaveTrackFromStreamAsync(requestStream);

        //        if (trackInfo == null)
        //        {
        //            throw new RpcException(new Status(StatusCode.InvalidArgument, "Track info is missing."));
        //        }

        //        await _trackService.SaveTrackInfoAsync(trackInfo, tempFilePath);

        //        return new UploadTrackResponse
        //        {
        //            Success = true,
        //            Message = "Track uploaded successfully."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error uploading track.");
        //        return new UploadTrackResponse
        //        {
        //            Success = false,
        //            Message = ex.Message
        //        };
        //    }
        //}
    }
}
