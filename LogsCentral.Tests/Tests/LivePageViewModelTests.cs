using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Models;
using LogsCentral.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class LivePageViewModelTests
{
    private LivePageViewModel CreateViewModel(string dbName)
    {
        var options = new DbContextOptionsBuilder<LogsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new LogsDbContext(options);

        return new LivePageViewModel(context);
    }

    [Fact]
    public async Task AddTestLogAsync_ShouldAddLogToDatabase()
    {
        var vm = CreateViewModel(nameof(AddTestLogAsync_ShouldAddLogToDatabase));

        await vm.AddTestLogAsync("Error", "Something bad happened");

        var logs = await vm.GetLatestLogsAsync();

        Assert.Single(logs);
        Assert.Equal("Error", logs[0].Level);
        Assert.Equal("Something bad happened", logs[0].Message);
    }

    [Fact]
    public async Task GetLatestLogsAsync_ShouldReturnLogsOrderedByTimestamp()
    {
        var vm = CreateViewModel(nameof(GetLatestLogsAsync_ShouldReturnLogsOrderedByTimestamp));

        await vm.AddTestLogAsync("Info", "First log");
        await Task.Delay(5);
        await vm.AddTestLogAsync("Info", "Second log");

        var logs = await vm.GetLatestLogsAsync();

        Assert.Equal(2, logs.Count);
        Assert.Equal("Second log", logs[0].Message);
        Assert.Equal("First log", logs[1].Message);
    }

    [Fact]
    public async Task GetLatestLogsAsync_ShouldFilterByLevel()
    {
        var vm = CreateViewModel(nameof(GetLatestLogsAsync_ShouldFilterByLevel));

        await vm.AddTestLogAsync("Info", "Info log");
        await vm.AddTestLogAsync("Error", "Error log");

        var logs = await vm.GetLatestLogsAsync("Error");

        Assert.Single(logs);
        Assert.Equal("Error", logs[0].Level);
        Assert.Equal("Error log", logs[0].Message);
    }

    [Fact]
    public async Task GetLatestLogsAsync_ShouldRespectTakeParameter()
    {
        var vm = CreateViewModel(nameof(GetLatestLogsAsync_ShouldRespectTakeParameter));

        for (int i = 0; i < 10; i++)
        {
            await vm.AddTestLogAsync("Debug", $"Log {i}");
        }

        var logs = await vm.GetLatestLogsAsync(take: 5);

        Assert.Equal(5, logs.Count);
    }
}
