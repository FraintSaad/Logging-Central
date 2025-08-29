using System;
using System.Threading.Tasks;
using Data.Context;
using Data.Models;
using LogsCentral.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class StatusPageViewModelTests
{
    private StatusPageViewModel CreateViewModel(string dbName, out LogsDbContext context)
    {
        var options = new DbContextOptionsBuilder<LogsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        context = new LogsDbContext(options);
        return new StatusPageViewModel(context);
    }

    [Fact]
    public async Task LoadAsync_ShouldCountLogsForDayWeekMonth()
    {
        var vm = CreateViewModel(nameof(LoadAsync_ShouldCountLogsForDayWeekMonth), out var context);

        context.SerilogEvents.Add(new LogEntity { Timestamp = DateTime.Now.AddHours(-5), Level = "Info", Message = "Day log" });
        context.SerilogEvents.Add(new LogEntity { Timestamp = DateTime.Now.AddDays(-3), Level = "Error", Message = "Week log" });
        context.SerilogEvents.Add(new LogEntity { Timestamp = DateTime.Now.AddDays(-20), Level = "Debug", Message = "Month log" });
        await context.SaveChangesAsync();

        await vm.LoadAsync(null);

        Assert.Equal(1, vm.LastDayCount);
        Assert.Equal(2, vm.LastWeekCount);
        Assert.Equal(3, vm.LastMonthCount);
    }

    [Fact]
    public async Task LoadAsync_ShouldRespectSelectedDays()
    {
        var vm = CreateViewModel(nameof(LoadAsync_ShouldRespectSelectedDays), out var context);

        context.SerilogEvents.Add(new LogEntity { Timestamp = DateTime.Now.AddDays(-2), Level = "Info", Message = "Old log" });
        context.SerilogEvents.Add(new LogEntity { Timestamp = DateTime.Now.AddHours(-1), Level = "Error", Message = "Recent log" });
        await context.SaveChangesAsync();

        await vm.LoadAsync(1);

        Assert.Equal(1, vm.SelectedDays);
        Assert.Equal(1, vm.SelectedPeriodCount);
    }

    [Fact]
    public async Task LoadAsync_ShouldGroupLogsByLevel()
    {
        var vm = CreateViewModel(nameof(LoadAsync_ShouldGroupLogsByLevel), out var context);

        context.SerilogEvents.Add(new LogEntity { Timestamp = DateTime.Now, Level = "Info", Message = "Log 1" });
        context.SerilogEvents.Add(new LogEntity { Timestamp = DateTime.Now, Level = "Info", Message = "Log 2" });
        context.SerilogEvents.Add(new LogEntity { Timestamp = DateTime.Now, Level = "Error", Message = "Log 3" });
        await context.SaveChangesAsync();

        await vm.LoadAsync(7);

        Assert.Equal(2, vm.LogsByLevel["Info"]);
        Assert.Equal(1, vm.LogsByLevel["Error"]);
    }

    [Fact]
    public async Task LoadAsync_ShouldSetDefaultDaysTo7_WhenNullPassed()
    {
        var vm = CreateViewModel(nameof(LoadAsync_ShouldSetDefaultDaysTo7_WhenNullPassed), out var context);

        context.SerilogEvents.Add(new LogEntity { Timestamp = DateTime.Now, Level = "Debug", Message = "Any log" });
        await context.SaveChangesAsync();

        await vm.LoadAsync(null);

        Assert.Equal(7, vm.SelectedDays);
    }
}
