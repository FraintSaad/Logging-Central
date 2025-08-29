using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Models;
using LogsCentral.Models;
using LogsCentral.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class LogsViewModelTests
{
    private LogsViewModel CreateViewModel(string dbName)
    {
        var options = new DbContextOptionsBuilder<LogsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new LogsDbContext(options);

        if (!context.SerilogEvents.Any())
        {
            context.SerilogEvents.AddRange(
                new LogEntity { Level = "Debug", Message = "Debug message", Timestamp = DateTime.UtcNow.AddMinutes(-10) },
                new LogEntity { Level = "Information", Message = "Info message", Timestamp = DateTime.UtcNow.AddMinutes(-5) },
                new LogEntity { Level = "Warning", Message = "Warning message", Timestamp = DateTime.UtcNow.AddMinutes(-2) },
                new LogEntity { Level = "Error", Message = "Error message", Timestamp = DateTime.UtcNow }
            );
            context.SaveChanges();
        }

        return new LogsViewModel(context);
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnOnlyErrorLogs_WhenFilterErrorIsTrue()
    {
        var vm = CreateViewModel(nameof(LoadAsync_ShouldReturnOnlyErrorLogs_WhenFilterErrorIsTrue));

        var result = await vm.LoadAsync(false, false, false, true, null, true, 1);

        Assert.Single(result.Logs);
        Assert.Equal("Error", result.Logs[0].Level);
    }

    [Fact]
    public async Task LoadAsync_ShouldReturnAllLogs_WhenNoFilterSelected()
    {
        var vm = CreateViewModel(nameof(LoadAsync_ShouldReturnAllLogs_WhenNoFilterSelected));

        var result = await vm.LoadAsync(false, false, false, false, null, true, 1);

        Assert.Equal(4, result.Logs.Count);
    }

    [Fact]
    public async Task LoadAsync_ShouldSortByMessageAscending()
    {
        var vm = CreateViewModel(nameof(LoadAsync_ShouldSortByMessageAscending));

        var result = await vm.LoadAsync(true, true, true, true, "message", true, 1);

        var messages = result.Logs.Select(l => l.Message).ToList();
        var sorted = messages.OrderBy(m => m).ToList();

        Assert.Equal(sorted, messages);
    }

    [Fact]
    public async Task LoadAsync_ShouldSortByMessageDescending()
    {
        var vm = CreateViewModel(nameof(LoadAsync_ShouldSortByMessageDescending));

        var result = await vm.LoadAsync(true, true, true, true, "message", false, 1);

        var messages = result.Logs.Select(l => l.Message).ToList();
        var sorted = messages.OrderByDescending(m => m).ToList();

        Assert.Equal(sorted, messages);
    }

    [Fact]
    public async Task LoadAsync_ShouldPaginateLogsCorrectly()
    {
        var vm = CreateViewModel(nameof(LoadAsync_ShouldPaginateLogsCorrectly));

        var result = await vm.LoadAsync(true, true, true, true, null, true, 2, 2);

        Assert.Equal(2, result.Logs.Count);
        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(2, result.TotalPages);
    }
}
