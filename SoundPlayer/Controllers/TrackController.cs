using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using SoundPlayer.Domain.Constants;
using SoundPlayer.Domain.DTO;
using SoundPlayer.Domain.Interfaces;

namespace SoundPlayer.Controllers
{
    public class TrackController : TrackProto.TrackProtoBase
    {
        private readonly ITrackService _trackService;
        private readonly IFileService _fileService;
        private readonly ILogger<TrackController> _logger;

        public TrackController(ITrackService trackService, ILogger<TrackController> logger, IFileService fileService)
        {
            _fileService = fileService;
            _trackService = trackService;
            _logger = logger;
        }
        
        [Authorize(Policy = Policy.User)]
        public override async Task<UploadTrackResponse> UploadTrack(IAsyncStreamReader<TrackChunk> requestStream, ServerCallContext context)
        {
            var trackGuid = Guid.NewGuid();
            var createDate = DateTime.UtcNow;
            
            try
            {
                TrackMetadata? metadata = null;
                
                await foreach (var chunk in requestStream.ReadAllAsync())
                {
                    switch (chunk.DataCase)
                    {
                        case TrackChunk.DataOneofCase.Info:
                            metadata = chunk.Info;
                            continue;
                        case TrackChunk.DataOneofCase.Chunks:
                            await _fileService.SaveChunkAsync(trackGuid, chunk.Chunks.ToByteArray());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (metadata == null)
                {
                    throw new RpcException(new Status(
                        StatusCode.InvalidArgument, 
                        "Metadata must be sent as first chunk"));
                }

                var dto = new TrackDto()
                {
                    Name = metadata.Name,
                    UserId = metadata.UserId,
                    UserName = metadata.UserName,
                    Duration = TimeSpan.FromSeconds(metadata.DurationInSeconds)
                };

                var trackResponse =  await _trackService.SaveTrackInfo(dto, trackGuid, createDate);

                if (trackResponse.IsSuccess == false) throw new Exception(trackResponse.Message);
                    
                return new UploadTrackResponse
                {
                    TrackId = trackResponse.Result.Id,
                    Name = trackResponse.Result.Name,
                    Success = true,
                    Message = "Track uploaded successfully"
                };
            }
            catch (Exception ex)
            {
                await _fileService.DeleteTrackAsync(trackGuid, createDate);
                _logger.LogError(ex, "Error uploading track");
                return new UploadTrackResponse
                {
                    Success = false,
                    Message = $"Upload failed: {ex.Message}"
                };
            }
        }

        [Authorize(Policy = Policy.User)]
        public override async Task DownloadTrack(TrackId request, IServerStreamWriter<TrackChunk> responseStream, ServerCallContext context)
        {
            try
            {
                var trackMetadata = await _trackService.GetTrackInfo(request.Id);
                if (trackMetadata.IsSuccess == false || trackMetadata.Result == null)
                {
                    throw new RpcException(new Status(
                        StatusCode.NotFound,
                        $"Track with id {request.Id} not found. Ex = {trackMetadata.Message}"));
                }

                var track = trackMetadata.Result;

                await responseStream.WriteAsync(new TrackChunk
                {
                    Info = new TrackMetadata
                    {
                        Id = track.Id,
                        Name = track.Name,
                        UserId = track.UserId,
                        UserName = track.UserName,
                        DurationInSeconds = track.Duration.Seconds
                    }
                });

                const int chunkSize = 8;
                var filePath = await _fileService.GetTrackPath(Guid.Parse(track.UniqueName), track.UploadDate);

                await using var fileStream = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: chunkSize,
                    useAsync: true);

                var buffer = new byte[chunkSize];
                int bytesRead;

                while ((bytesRead = await fileStream.ReadAsync(buffer)) > 0)
                {
                    await responseStream.WriteAsync(new TrackChunk
                    {
                        Chunks = ByteString.CopyFrom(buffer, 0, bytesRead)
                    });

                    Array.Clear(buffer, 0, buffer.Length);
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "Audio file not found");
                throw new RpcException(new Status(
                    StatusCode.NotFound,
                    "Audio file not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during track download");
                throw new RpcException(new Status(
                    StatusCode.Internal, 
                    $"Download failed. {ex.Message}"));
            }
        }

        [Authorize(Policy = Policy.User)]
        public override async Task<TrackMetadata> GetTrackInfo(TrackId request, ServerCallContext context)
        {
            var response = await _trackService.GetTrackInfo(request.Id);

            if (response == null || response.Result == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Track not found"));
            }
            
            var trackDto = response.Result;
            var trackInfo = new TrackMetadata()
            {
                Name = trackDto.Name,
                UserId = trackDto.UserId
            };

            return trackInfo;
        }
        
        [Authorize(Policy = Policy.User)]
        public override async Task<GetTracksResponse> GetTrackList(GetTracksRequest request, ServerCallContext context)
        {
            var result = await _trackService.GetTrackListByName(request.TrackName, request.PageNumber, request.PageSize);

            if (!result.IsSuccess)
            {
                throw new RpcException(new Status(StatusCode.Internal, result.Message));
            }

            return new GetTracksResponse
            {
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                Tracks = { result.Items.Select(t => new TrackMetadata()
                {
                    Id = t.Id,
                    Name = t.Name,
                    UserId = t.UserId,
                    UserName = t.UserName,
                    DurationInSeconds = (int)t.Duration.TotalSeconds,
                }) }
            };
        }
    }
}
