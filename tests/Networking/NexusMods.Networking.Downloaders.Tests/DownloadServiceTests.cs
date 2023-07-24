using FluentAssertions;
using NexusMods.DataModel.RateLimiting;
using NexusMods.Networking.Downloaders.Interfaces;
using Size = NexusMods.Paths.Size;

namespace NexusMods.Networking.Downloaders.Tests;

public class DownloadServiceTests
{
    // For the uninitiated with xUnit: This is initialized before every test.
    private readonly DownloadService _downloadService;
    private readonly DummyDownloadTask _dummyTask;

    public DownloadServiceTests(DownloadService downloadService)
    {
        // Create a new instance of the DownloadService
        _downloadService = downloadService;
        _dummyTask = new DummyDownloadTask(_downloadService);
    }

    [Fact]
    public void AddTask_ShouldFireStartedObservable()
    {
        // Arrange
        var startedObservableFired = false;
        _downloadService.StartedTasks.Subscribe(_ => { startedObservableFired = true; });

        // Act
        _downloadService.AddTask(_dummyTask);

        // Assert
        startedObservableFired.Should().BeTrue();
    }

    [Fact]
    public void OnComplete_ShouldFireCompletedObservable()
    {
        // Arrange
        var completedObservableFired = false;
        _downloadService.CompletedTasks.Subscribe(_ => { completedObservableFired = true; });

        // Act
        _downloadService.OnComplete(_dummyTask);

        // Assert
        completedObservableFired.Should().BeTrue();
    }

    [Fact]
    public void OnCancelled_ShouldFireCancelledObservable()
    {
        // Arrange
        var cancelledObservableFired = false;
        _downloadService.CancelledTasks.Subscribe(_ => { cancelledObservableFired = true; });

        // Act
        _downloadService.OnCancelled(_dummyTask);

        // Assert
        cancelledObservableFired.Should().BeTrue();
    }

    [Fact]
    public void OnPaused_ShouldFirePausedObservable()
    {
        // Arrange
        var pausedObservableFired = false;
        _downloadService.PausedTasks.Subscribe(_ => { pausedObservableFired = true; });

        // Act
        _downloadService.OnPaused(_dummyTask);

        // Assert
        pausedObservableFired.Should().BeTrue();
    }

    [Fact]
    public void OnResumed_ShouldFireResumedObservable()
    {
        // Arrange
        var resumedObservableFired = false;
        _downloadService.ResumedTasks.Subscribe(_ => { resumedObservableFired = true; });

        // Act
        _downloadService.OnResumed(_dummyTask);

        // Assert
        resumedObservableFired.Should().BeTrue();
    }

    private class DummyDownloadTask : IDownloadTask
    {
        public DummyDownloadTask(DownloadService service) { Owner = service; }
        public IEnumerable<IJob<Size>> DownloadJobs => Array.Empty<IJob<Size>>();
        public DownloadService Owner { get; }
        public DownloadTaskStatus Status { get; }
        public string FriendlyName { get; } = "";

        public Task StartAsync() => Task.CompletedTask;
        public void Cancel() => Owner.OnCancelled(this);
        public void Pause() => Owner.OnPaused(this);
        public void Resume() => Owner.OnResumed(this);
    }
}