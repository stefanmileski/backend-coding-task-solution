using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Domain;
using Claims.Infrastructure.Interfaces;
using Claims.Services.Cover;
using Claims.Services.Cover.Interfaces;
using NSubstitute;
using Xunit;

namespace Claims.Tests;

public class CoversServiceTests
{
    private readonly IClaimsContext _context = Substitute.For<IClaimsContext>();
    private readonly ICoversService _sut;

    private static readonly DateTime Today = DateTime.UtcNow.Date;

    public CoversServiceTests()
    {
        _sut = new CoversService(_context);
    }

    // ── CreateCoverAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateCover_ValidRequest_ReturnsCover()
    {
        var request = new CreateCoverRequest(Today, Today.AddMonths(3), CoverType.Yacht);
        _context.AddCoverAsync(Arg.Any<Cover>()).Returns(c => c.Arg<Cover>());

        CoverResponse? result = await _sut.CreateCoverAsync(request);

        Assert.NotNull(result);
        Assert.Equal(CoverType.Yacht, result.Type);
        Assert.True(result.Premium > 0);
    }

    [Fact]
    public async Task CreateCover_StartDateInPast_ReturnsNull()
    {
        var request = new CreateCoverRequest(Today.AddDays(-1), Today.AddMonths(3), CoverType.Yacht);

        CoverResponse? result = await _sut.CreateCoverAsync(request);

        Assert.Null(result);
        await _context.DidNotReceive().AddCoverAsync(Arg.Any<Cover>());
    }

    [Fact]
    public async Task CreateCover_PeriodExceedsOneYear_ReturnsNull()
    {
        var request = new CreateCoverRequest(Today, Today.AddYears(1).AddDays(1), CoverType.Yacht);

        CoverResponse? result = await _sut.CreateCoverAsync(request);

        Assert.Null(result);
        await _context.DidNotReceive().AddCoverAsync(Arg.Any<Cover>());
    }

    [Fact]
    public async Task CreateCover_PeriodExactlyOneYear_ReturnsCover()
    {
        var request = new CreateCoverRequest(Today, Today.AddYears(1), CoverType.Yacht);
        _context.AddCoverAsync(Arg.Any<Cover>()).Returns(c => c.Arg<Cover>());

        CoverResponse? result = await _sut.CreateCoverAsync(request);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateCover_EndDateBeforeStartDate_ReturnsNull()
    {
        var request = new CreateCoverRequest(Today, Today.AddDays(-1), CoverType.Yacht);

        CoverResponse? result = await _sut.CreateCoverAsync(request);

        Assert.Null(result);
        await _context.DidNotReceive().AddCoverAsync(Arg.Any<Cover>());
    }

    // ── GetCoverAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetCover_ExistingId_ReturnsCover()
    {
        var cover = new Cover("cover-1", Today, Today.AddMonths(3), CoverType.Yacht, 1000m);
        _context.GetCoverAsync("cover-1").Returns(cover);

        CoverResponse? result = await _sut.GetCoverAsync("cover-1");

        Assert.NotNull(result);
        Assert.Equal("cover-1", result.Id);
    }

    [Fact]
    public async Task GetCover_NonExistentId_ReturnsNull()
    {
        _context.GetCoverAsync("nonexistent").Returns((Cover?)null);

        CoverResponse? result = await _sut.GetCoverAsync("nonexistent");

        Assert.Null(result);
    }

    // ── GetCoversAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetCovers_ReturnsAllCovers()
    {
        var covers = new List<Cover>
        {
            new("cover-1", Today, Today.AddMonths(3), CoverType.Yacht,         1000m),
            new("cover-2", Today, Today.AddMonths(6), CoverType.PassengerShip, 2000m),
        };
        _context.GetCoversAsync().Returns(covers);

        IEnumerable<CoverResponse> result = await _sut.GetCoversAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetCovers_NoCovers_ReturnsEmptyList()
    {
        _context.GetCoversAsync().Returns([]);

        IEnumerable<CoverResponse> result = await _sut.GetCoversAsync();

        Assert.Empty(result);
    }

    // ── DeleteCoverAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteCover_ExistingId_ReturnsTrue()
    {
        _context.DeleteCoverAsync("cover-1").Returns(true);

        bool result = await _sut.DeleteCoverAsync("cover-1");

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCover_NonExistentId_ReturnsFalse()
    {
        _context.DeleteCoverAsync("nonexistent").Returns(false);

        bool result = await _sut.DeleteCoverAsync("nonexistent");

        Assert.False(result);
    }

    // ── ComputePremium ──────────────────────────────────────────────────────

    [Theory]
    [InlineData(CoverType.Yacht, 1, 1375.00)]   // 1250 * 1.1
    [InlineData(CoverType.PassengerShip, 1, 1500.00)]   // 1250 * 1.2
    [InlineData(CoverType.Tanker, 1, 1875.00)]   // 1250 * 1.5
    [InlineData(CoverType.BulkCarrier, 1, 1625.00)]   // 1250 * 1.3
    public void ComputePremium_SingleDay_ReturnsCorrectRate(CoverType type, int days, decimal expected)
    {
        decimal result = _sut.ComputePremium(Today, Today.AddDays(days - 1), type);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ComputePremium_Yacht_30Days_NoDiscount()
    {
        decimal premiumPerDay = 1250m * 1.1m;
        decimal expected = premiumPerDay * 30;

        decimal result = _sut.ComputePremium(Today, Today.AddDays(29), CoverType.Yacht);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ComputePremium_Yacht_31Days_AppliesFirstDiscount()
    {
        decimal premiumPerDay = 1250m * 1.1m;
        decimal expected = premiumPerDay * 30           // first 30 days
                         + premiumPerDay * 0.95m * 1;  // day 31 at 5% discount

        decimal result = _sut.ComputePremium(Today, Today.AddDays(30), CoverType.Yacht);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ComputePremium_Yacht_181Days_AppliesSecondDiscount()
    {
        decimal premiumPerDay = 1250m * 1.1m;
        decimal expected = premiumPerDay * 30           // days 0–29
                         + premiumPerDay * 0.95m * 150 // days 30–179
                         + premiumPerDay * 0.92m * 1;  // day 180 at 8% discount

        decimal result = _sut.ComputePremium(Today, Today.AddDays(180), CoverType.Yacht);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ComputePremium_NonYacht_181Days_AppliesSecondDiscount()
    {
        decimal premiumPerDay = 1250m * 1.3m;
        decimal expected = premiumPerDay * 30           // days 0–29
                         + premiumPerDay * 0.98m * 150 // days 30–179
                         + premiumPerDay * 0.97m * 1;  // day 180 at 3% discount

        decimal result = _sut.ComputePremium(Today, Today.AddDays(180), CoverType.BulkCarrier);

        Assert.Equal(expected, result);
    }
}