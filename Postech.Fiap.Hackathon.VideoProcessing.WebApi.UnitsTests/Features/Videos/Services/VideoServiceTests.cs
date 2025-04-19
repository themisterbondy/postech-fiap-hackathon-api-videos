using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Common.ResultPattern;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Authentication.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Command;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Interfaces;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Models;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Repositories;
using Postech.Fiap.Hackathon.VideoProcessing.WebApi.Features.Videos.Services;

namespace Postech.Fiap.Hackathon.VideoProcessing.WebApi.UnitsTests.Features.Videos.Services;

public class VideoServiceTests
{
    private readonly ClaimsPrincipal _claimsPrincipal;
    private readonly IHttpContextAccessor _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
    private readonly IVideoQueueMessenger _queueMessenger = Substitute.For<IVideoQueueMessenger>();
    private readonly IStorageService _storageService = Substitute.For<IStorageService>();
    private readonly VideoService _sut;
    private readonly UserManager<User> _userManager;
    private readonly IVideoRepository _videoRepository = Substitute.For<IVideoRepository>();

    public VideoServiceTests()
    {
        var store = Substitute.For<IUserStore<User>>();
        _userManager = Substitute.For<UserManager<User>>(store, null, null, null, null, null, null, null, null);

        _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, "11111111-1111-1111-1111-111111111111")
        }, "mock"));

        var httpContext = new DefaultHttpContext
        {
            User = _claimsPrincipal
        };
        _httpContextAccessor.HttpContext.Returns(httpContext);

        _sut = new VideoService(_videoRepository, _storageService, _queueMessenger, _userManager, _httpContextAccessor);
    }

    [Fact]
    public async Task GetVideoById_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>())
            .Returns((User?)null);

        // Act
        var result = await _sut.GetVideoById(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("User not found");
    }

    [Fact]
    public async Task GetVideoById_ShouldReturnFailure_WhenVideoNotFound()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid().ToString() };
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(user);
        _videoRepository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>())
            .Returns((Video?)null);

        // Act
        var result = await _sut.GetVideoById(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Video not found");
    }

    [Fact]
    public async Task GetVideoById_ShouldReturnSuccess_WhenVideoIsFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var video = new Video { Id = Guid.NewGuid(), Status = VideoStatus.Processed, UserId = userId };

        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(new User { Id = userId.ToString() });
        _videoRepository.FindByIdAsync(Arg.Any<Guid>(), userId).Returns(video);

        // Act
        var result = await _sut.GetVideoById(video.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(VideoStatus.Processed);
    }

    [Fact]
    public async Task Upload_ShouldReturnFailure_WhenUserNotFound()
    {
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns((User?)null);

        var file = Substitute.For<IFormFile>();
        var command = new UploadVideoCommand.Command { File = file };

        var result = await _sut.Upload(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("User not found");
    }

    [Fact]
    public async Task Upload_ShouldReturnFailure_WhenUploadFails()
    {
        var user = new User { Id = Guid.NewGuid().ToString() };
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(user);

        var file = Substitute.For<IFormFile>();
        file.OpenReadStream().Returns(new MemoryStream());
        file.ContentType.Returns("video/mp4");

        var command = new UploadVideoCommand.Command { File = file };

        _storageService.UploadVideoAsync(Arg.Any<Guid>(), Arg.Any<Stream>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Result.Failure<string>(Error.Failure("code", "upload failed")));

        var result = await _sut.Upload(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Upload with error");
    }

    [Fact]
    public async Task Upload_ShouldReturnSuccess_WhenUploadSucceeds()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId.ToString() };
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(user);

        var file = Substitute.For<IFormFile>();
        file.OpenReadStream().Returns(new MemoryStream());
        file.ContentType.Returns("video/mp4");
        file.FileName.Returns("test.mp4");

        var command = new UploadVideoCommand.Command { File = file };

        _storageService.UploadVideoAsync(Arg.Any<Guid>(), Arg.Any<Stream>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Result.Success("path/to/video.mp4"));

        var result = await _sut.Upload(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(VideoStatus.Uploaded);

        await _videoRepository.Received(1).AddAsync(Arg.Any<Video>());
        await _queueMessenger.Received(1).SendAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task Download_ShouldReturnFailure_WhenVideoNotFound()
    {
        var user = new User { Id = Guid.NewGuid().ToString() };
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(user);
        _videoRepository.FindByIdAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns((Video?)null);

        var result = await _sut.Download(Guid.NewGuid(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Video not found");
    }

    [Fact]
    public async Task Download_ShouldReturnFailure_WhenFilePathIsEmpty()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId.ToString() };
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(user);

        var video = new Video { Id = Guid.NewGuid(), UserId = userId, ThumbnailsZipPath = "" };
        _videoRepository.FindByIdAsync(video.Id, userId).Returns(video);

        var result = await _sut.Download(video.Id, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("File path is null or empty");
    }

    [Fact]
    public async Task Download_ShouldReturnFailure_WhenDownloadFails()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId.ToString() };
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(user);

        var video = new Video { Id = Guid.NewGuid(), UserId = userId, ThumbnailsZipPath = "path/to/file.zip" };
        _videoRepository.FindByIdAsync(video.Id, userId).Returns(video);

        _storageService.DownloadVideoAsync(video.ThumbnailsZipPath, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Stream>(Error.Failure("x", "Download with error")));

        var result = await _sut.Download(video.Id, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Download with error");
    }

    [Fact]
    public async Task Download_ShouldReturnSuccess_WhenDownloadSucceeds()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId.ToString() };
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(user);

        var video = new Video
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ThumbnailsZipPath = "path/to/file.zip"
        };

        _videoRepository.FindByIdAsync(video.Id, userId).Returns(video);

        var stream = new MemoryStream();
        _storageService.DownloadVideoAsync(video.ThumbnailsZipPath, Arg.Any<CancellationToken>())
            .Returns(Result.Success<Stream>(stream));

        var result = await _sut.Download(video.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.ContentType.Should().Be("application/x-zip-compressed");
        result.Value.FileName.Should().Be($"{video.Id}.zip");
    }
}