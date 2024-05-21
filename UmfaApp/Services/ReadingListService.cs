using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Text.Json;
using UmfaApp.Data;
using UmfaApp.Data.Tables;
using UmfaApp.Enums;
using UmfaApp.Models;
using UmfaApp.Settings;

namespace UmfaApp.Services
{
    public interface IReadingListService
    {
        public Task<Guid?> CreateNewReadingListAsync(string name, int user, int partnerId, List<int> buildingIds, List<string> locations);
        public Task<ReadingListRequest> CreateNewReadingsRequestAsync(Guid readingListId, string periodName, List<ReadingListEntry> readingListEntries);

        public Task<List<ReadingList>> GetReadingListsAsync(int user, int partnerId);
        public Task<List<ReadingListRequest>> GetReadingListRequestsAsync(List<Guid> readingListIds);
        public Task<ReadingListRequest> GetReadingListRequestAsync(Guid id);
        public Task<bool> UpdateReadingListRequestAsync(ReadingListRequest request);
        public Task<List<Reading>> GetReadingsAsync(Guid requestId);

        public Task<bool> SaveReadingAsync(Reading reading);

        public Task<string> UploadReadingsAsync(int userId, string deviceId, ReadingListRequest request);

        public Task<string> UploadMediaAsync(int userId, string deviceId, ReadingListRequest request);

        public Task DeleteReadingListRequestsAsync(List<ReadingListRequest> requests, ReadingList list);
        public void DeletePhotoFile(string fileName);

    }
    public class ReadingListService : IReadingListService
    {
        private readonly DbAccessor _dbAccessor;
        private readonly ILogger<ReadingListService> _logger;
        private readonly IUmfaService _umfaService;
        private readonly IActionLogService _loggingService;
        public ReadingListService(ILogger<ReadingListService> logger, DbAccessor dbAccessor, IUmfaService umfaService, IActionLogService actionLogService) 
        {
            _logger = logger;
            _dbAccessor = dbAccessor;
            _umfaService = umfaService;
            _loggingService = actionLogService;
        }

        public async Task<Guid?> CreateNewReadingListAsync(string name, int user, int partnerId, List<int> buildingIds, List<string> locations)
        {
            try
            {
                var list = new ReadingList
                {
                    Name = name,
                    UserId = user,
                    BuildingIds = string.Join(",", buildingIds),
                    PartnerId = partnerId,
                    Locations = string.Join(",", locations),
                };
                
                var result = await _dbAccessor.InsertReadingListAsync(list);

                if(result != 1)
                {
                    _logger.LogError("Could not create new reading list");
                    return null;
                }

                return list.Id;
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<ReadingListRequest> CreateNewReadingsRequestAsync(Guid readingListId, string periodName, List<ReadingListEntry> readingListEntries)
        {
            try
            {
                var newRequestId = Guid.NewGuid();

                var newRequest = new ReadingListRequest { Id = newRequestId, Period = periodName, ReadingListId = readingListId };

                var result1 = await _dbAccessor.InsertReadingListRequestAsync(newRequest);
                if(result1 != 1)
                {
                    _logger.LogError("Could not insert new reading list request");
                    return null;
                }

                var result2 = await _dbAccessor.InsertReadingsAsync(readingListEntries.Select(rle => new Reading(newRequestId,rle)).ToList());

                return result2 > 0 ? newRequest : null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<List<ReadingListRequest>> GetReadingListRequestsAsync(List<Guid> readingListIds)
        {
            try
            {
                return await _dbAccessor.GetReadingListRequestsAsync(readingListIds);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<List<ReadingList>> GetReadingListsAsync(int user, int partnerId)
        {
            try
            {
                return await _dbAccessor.GetReadingListsAsync(user, partnerId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<List<Reading>> GetReadingsAsync(Guid requestId)
        {
            try
            {
                return await _dbAccessor.GetReadingsByReadingListRequestIdAsync(requestId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<bool> SaveReadingAsync(Reading reading)
        {
            try
            {
                var result = await _dbAccessor.UpdateReadingAsync(reading);

                if(result != 1)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public async Task<string> UploadMediaAsync(int userId, string deviceId, ReadingListRequest request)
        {
            try
            {
                var readings = await _dbAccessor.GetReadingsByReadingListRequestIdAsync(request.Id);

                foreach (var reading in readings)
                {

                    //reading.Photos = "C:\\Users\\conra\\OneDrive\\Pictures\\house3.jpg";
                    if (string.IsNullOrWhiteSpace(reading.Photos) && string.IsNullOrWhiteSpace(reading.VoiceNote))
                    {
                        if(request.MediaUploadStatus == UploadStatus.Pending) request.MediaUploadStatus = UploadStatus.Uploaded;
                        await _loggingService.AddLog(Enums.Action.UploadImage, (int)AppSettings.UserId!, JsonSerializer.Serialize(reading), request.Id);
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(reading.Photos))
                    {
                        var imagePaths = reading.Photos.Split('|');
                        var uploadedPaths = reading.UploadedPhotos?.Split('|').ToList() ?? new List<string>();
                        var failed = reading.FailedUploadedPhotos?.Split('|').ToList() ?? new List<string>();
                        foreach (var image in imagePaths.Where(i => !uploadedPaths.Contains(i) && !failed.Contains(i)))
                        {
                            try
                            {
                                var imageBytes = File.ReadAllBytes(Path.Combine(AppSettings.PhotoDirectory, image));

                                var compressed =  CompressImage(imageBytes);

                                var uploaded = await _umfaService.UploadReadingImageAsync(new Models.UmfaApiModels.RequestModels.UploadReadingImageRequest
                                {
                                    DeviceId = deviceId,
                                    UserID = userId,
                                    CommsDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                                    JSONReadingData = new Models.UmfaApiModels.RequestModels.ReadingImage
                                    {
                                        BuildingId = reading.BuildingId,
                                        BuildingServiceID = reading.BuildingServiceId,
                                        PeriodId = reading.PeriodId,
                                        ImageDTM = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                                        Image = compressed,
                                    }
                                });

                                if (uploaded)
                                {
                                    await _loggingService.AddLog(Enums.Action.UploadImage, (int)AppSettings.UserId!, JsonSerializer.Serialize(reading), request.Id);
                                    if (request.MediaUploadStatus == UploadStatus.Pending) request.MediaUploadStatus = UploadStatus.Uploaded;
                                    uploadedPaths.Add(image);
                                }
                                else
                                {
                                    await _loggingService.AddLog(Enums.Action.UploadImage, (int)AppSettings.UserId!, JsonSerializer.Serialize(reading), request.Id,"Could not upload image");
                                    if (request.MediaUploadStatus == UploadStatus.Uploaded) request.MediaUploadStatus = UploadStatus.Partial;
                                    failed.Add(image);
                                }
                            }
                            catch (Exception e)
                            {
                                await _loggingService.AddLog(Enums.Action.UploadImage, (int)AppSettings.UserId!, JsonSerializer.Serialize(reading), request.Id, e.Message);
                                if (request.MediaUploadStatus == UploadStatus.Uploaded) request.MediaUploadStatus = UploadStatus.Partial;
                                failed.Add(image);
                                _logger.LogError(e, e.Message);
                            }
                        }

                        reading.FailedUploadedPhotos = string.Join('|', failed);
                        reading.UploadedPhotos = string.Join('|', uploadedPaths);
                    }
                    
                    try
                    {
                        if(!string.IsNullOrWhiteSpace(reading.VoiceNote))
                        {
                            var audio = File.ReadAllBytes(reading.VoiceNote);

                            var uploaded = await _umfaService.UploadReadingAudioAsync(new Models.UmfaApiModels.RequestModels.UploadReadingAudioRequest
                            {
                                DeviceId = deviceId,
                                UserID = userId,
                                CommsDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                                JSONReadingData = new Models.UmfaApiModels.RequestModels.ReadingAudio
                                {
                                    BuildingId = reading.BuildingId,
                                    BuildingServiceID = reading.BuildingServiceId,
                                    PeriodId = reading.PeriodId,
                                    AudioDTM = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Audio = audio,
                                }
                            });

                            if (uploaded)
                            {
                                await _loggingService.AddLog(Enums.Action.UploadVoiceNote, (int)AppSettings.UserId!, JsonSerializer.Serialize(reading), request.Id);
                                if (request.MediaUploadStatus == UploadStatus.Pending) request.MediaUploadStatus = UploadStatus.Uploaded;
                            }
                            else
                            {
                                await _loggingService.AddLog(Enums.Action.UploadVoiceNote, (int)AppSettings.UserId!, JsonSerializer.Serialize(reading), request.Id, "Could not upload voicenote");
                                if (request.MediaUploadStatus == UploadStatus.Uploaded) request.MediaUploadStatus = UploadStatus.Partial;
                            }
                            
                            reading.VoiceNoteUploaded = uploaded;
                        }
                    }
                    catch (Exception e)
                    {
                        await _loggingService.AddLog(Enums.Action.UploadVoiceNote, (int)AppSettings.UserId!, JsonSerializer.Serialize(reading), request.Id, e.Message);
                        if (request.MediaUploadStatus == UploadStatus.Uploaded) request.MediaUploadStatus = UploadStatus.Partial;
                        _logger.LogError(e, e.Message);
                    }

                    if (request.MediaUploadStatus == UploadStatus.Pending) request.MediaUploadStatus = UploadStatus.Failed;

                    await _dbAccessor.UpdateReadingListRequestAsync(request);
                    await _dbAccessor.UpdateReadingAsync(reading);
                }

                return null;
            }
            catch (Exception e)
            {
                await _loggingService.AddLog(Enums.Action.UploadMedia, (int)AppSettings.UserId!, JsonSerializer.Serialize(request), request.Id, e.Message);
                _logger.LogError(e, e.Message);
                return $"Something went wrong: {e.Message}";
            }
        }

        private byte[] CompressImage(byte[] imageBytes, int quality = 75)
        {
            using (var input = new MemoryStream(imageBytes))
            using (var inputStream = new SKManagedStream(input))
            using (var original = SKBitmap.Decode(inputStream))
            {
                using (var output = new MemoryStream())
                {
                    // Set the encoding format to JPEG for better compression
                    var format = SKEncodedImageFormat.Jpeg;

                    // Create the image info using the original image dimensions
                    var info = new SKImageInfo(original.Width, original.Height);

                    using (var image = SKImage.FromBitmap(original))
                    {
                        // Encode the image to the output stream with JPEG format and specified quality
                        image.Encode(format, quality).SaveTo(output);

                        // Return the contents of the output stream after encoding
                        return output.ToArray();
                    }
                }
            }
        }

        public async Task<string> UploadReadingsAsync(int userId, string deviceId, ReadingListRequest request)
        {
            try
            {
                var readings = await _dbAccessor.GetReadingsByReadingListRequestIdAsync(request.Id);

                var response = await _umfaService.UploadReadingsAsync(new Models.UmfaApiModels.RequestModels.UploadReadingsRequest(userId, deviceId, readings));

                if(response.Error == -1)
                {
                    await _loggingService.AddLog(Enums.Action.UploadReadings, (int)AppSettings.UserId!, JsonSerializer.Serialize(request), request.Id, "Periods have been closed");
                    var buildingNames = readings.Where(r => response.BuildingIds.Contains(r.BuildingId)).Select(r => r.BuildingName);

                    return $"The following buildings' {string.Join(", ", buildingNames)} periods have been closed. Please merge this request with a new readings request & upload that one instead";
                }
                else if(response.Error == 0)
                {
                    request.ReadingsUploaded = true;

                    await _dbAccessor.UpdateReadingListRequestAsync(request);
                    await _loggingService.AddLog(Enums.Action.UploadReadings, (int)AppSettings.UserId!, JsonSerializer.Serialize(request), request.Id);
                    return null;
                }

                return $"Could not upload readings, request id: {response.RequestId}";
            }
            catch (Exception e)
            {
                await _loggingService.AddLog(Enums.Action.UploadReadings, (int)AppSettings.UserId!, JsonSerializer.Serialize(request), request.Id, e.Message);
                _logger.LogError(e, e.Message);
                return $"Something went wrong: {e.Message}";
            }
        }

        public async Task<ReadingListRequest> GetReadingListRequestAsync(Guid id)
        {
            try
            {
                return await _dbAccessor.GetReadingListRequestAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<bool> UpdateReadingListRequestAsync(ReadingListRequest request)
        {
            try
            {
                var result = await _dbAccessor.UpdateReadingListRequestAsync(request);

                if (result != 1)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }

        public async Task DeleteReadingListRequestsAsync(List<ReadingListRequest> requests, ReadingList list)
        {
            
            foreach(var request in requests)
            {
                var readings = await _dbAccessor.GetReadingsByReadingListRequestIdAsync(request.Id);

                var allPhotos = readings.SelectMany(r => r.PhotosList ?? Enumerable.Empty<string>()).ToList();

                foreach (var photo in allPhotos)
                {
                    DeletePhotoFile(photo);
                }

                await _dbAccessor.DeleteReadingListRequestCascadingAsync(request);

                await _loggingService.AddLog(Enums.Action.DeleteReadingListRequest, (int)AppSettings.UserId!, request.Id.ToString(), request.ReadingListId);
            }

            var requestsAfter = await _dbAccessor.GetReadingListRequestsAsync(list.Id);
            if(requestsAfter is null || !requestsAfter.Any())
            {
                await _dbAccessor.DeleteReadingListAsync(list);
            }
        }

        public void DeletePhotoFile(string fileName)
        {
            string photoPath = Path.Combine(AppSettings.PhotoDirectory, fileName);
            try
            {
                // Check if the file exists before trying to delete
                if (File.Exists(photoPath))
                {
                    // Delete the photo file
                    File.Delete(photoPath);

                    _loggingService.AddLog(Enums.Action.DeleteImage, (int)AppSettings.UserId!, photoPath, null);
                    return;
                }

                _loggingService.AddLog(Enums.Action.DeleteImage, (int)AppSettings.UserId!, photoPath, null, "Image does not exist");
                return;
            }
            catch(Exception e)
            {
                _loggingService.AddLog(Enums.Action.DeleteImage, (int)AppSettings.UserId!, photoPath, null, $"Could not delete image: ${e.Message}");
                return;
            }
        }
    }
}
