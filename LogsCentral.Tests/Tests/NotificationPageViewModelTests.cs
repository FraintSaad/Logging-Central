using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities;
using LogsCentral.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class NotificationsPageViewModelTests
{
    private NotificationsPageViewModel CreateViewModel(string dbName)
    {
        var options = new DbContextOptionsBuilder<LogsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        var context = new LogsDbContext(options);
        return new NotificationsPageViewModel(context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddRule()
    {
        var vm = CreateViewModel(nameof(AddAsync_ShouldAddRule));

        var model = new NotificationRuleViewModel
        {
            Period = 5,
            Threshold = 10,
            Email = "test@mail.com"
        };

        await vm.AddAsync(model, "Error");

        var rules = await vm.GetAllAsync();

        Assert.Single(rules);
        Assert.Equal(5, rules[0].Period);
        Assert.Equal(10, rules[0].Threshold);
        Assert.Equal("Error", rules[0].LogLevels);
        Assert.Equal("test@mail.com", rules[0].Email);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRules()
    {
        var vm = CreateViewModel(nameof(GetAllAsync_ShouldReturnAllRules));

        await vm.AddAsync(new NotificationRuleViewModel { Period = 3, Threshold = 5, Email = "a@mail.com" }, "Info");
        await vm.AddAsync(new NotificationRuleViewModel { Period = 7, Threshold = 15, Email = "b@mail.com" }, "Warning");

        var rules = await vm.GetAllAsync();

        Assert.Equal(2, rules.Count);
        Assert.Contains(rules, r => r.Email == "a@mail.com");
        Assert.Contains(rules, r => r.Email == "b@mail.com");
    }

    [Fact]
    public async Task EditAsync_ShouldUpdateRule()
    {
        var vm = CreateViewModel(nameof(EditAsync_ShouldUpdateRule));

        var model = new NotificationRuleViewModel
        {
            Period = 3,
            Threshold = 5,
            Email = "old@mail.com"
        };

        await vm.AddAsync(model, "Info");

        var rule = (await vm.GetAllAsync()).First();

        rule.Period = 10;
        rule.Threshold = 20;
        rule.Email = "new@mail.com";

        await vm.EditAsync(rule, "Error");

        var updated = (await vm.GetAllAsync()).First();

        Assert.Equal(10, updated.Period);
        Assert.Equal(20, updated.Threshold);
        Assert.Equal("Error", updated.LogLevels);
        Assert.Equal("new@mail.com", updated.Email);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRule()
    {
        var vm = CreateViewModel(nameof(DeleteAsync_ShouldRemoveRule));

        await vm.AddAsync(new NotificationRuleViewModel { Period = 3, Threshold = 5, Email = "del@mail.com" }, "Warning");

        var rule = (await vm.GetAllAsync()).First();

        await vm.DeleteAsync(rule.Id);

        var rules = await vm.GetAllAsync();
        Assert.Empty(rules);
    }
}
