using Moq;
using Solicitors.Core.Data;
using Solicitors.Core.Models;

namespace Solicitors.Core.Tests;

public class SolicitorServiceTests
{
    private readonly Mock<IReadOnlySolicitorRepository> _mockRepo;
    private readonly SolicitorService _service;

    public SolicitorServiceTests()
    {
        _mockRepo = new Mock<IReadOnlySolicitorRepository>();
        _service = new SolicitorService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetSolicitorInfoByIdAsync_Should_ReturnNull_When_NoMatchingSolicitor()
    {
        //arrange
        var cts = new CancellationTokenSource();
        var id = Guid.NewGuid();
        _mockRepo
            .Setup(x => x.GetSolicitorByIdAsync(It.IsAny<Guid>(), cts.Token))
            .ReturnsAsync(default(Solicitor?));
        
        //act
        var result = await _service.GetSolicitorInfoByIdAsync(id, cts.Token);
        
        //assert
        Assert.Null(result);
        _mockRepo.Verify(x => x.GetSolicitorByIdAsync(id, cts.Token), Times.Once);
    }

    [Fact]
    public async Task GetSolicitorInfoByIdAsync_Should_ReturnMainSolicitorInfo_When_MatchingSolicitor()
    {
        //arrange
        var cts = new CancellationTokenSource();
        var id = Guid.NewGuid();

        var expected = new Solicitor
        {
            SolicitorId = id,
            Name = "Solicitor",
            ShortDescription = "Short Desc",
            Phone = "000 0000 0000",
            Email = "example@example.com",
            Website = "example.com",
            RelativeUrl = "/-some-solicitor.html"
        };
        
        _mockRepo
            .Setup(x => x.GetSolicitorByIdAsync(It.IsAny<Guid>(), cts.Token))
            .ReturnsAsync(expected);
        
        //act
        var result = await _service.GetSolicitorInfoByIdAsync(id, cts.Token);
        
        //assert
        Assert.NotNull(result);
        Assert.Equal(result.Id, expected.SolicitorId);
        Assert.Equal(result.Name, expected.Name);
        Assert.Equal(result.ShortDescription, expected.ShortDescription);
        Assert.Equal(result.Phone, expected.Phone);
        Assert.Equal(result.Email, expected.Email);
        Assert.Equal(result.Website, expected.Website);
    }

    [Fact]
    public async Task GetSolicitorInfoByIdAsync_Should_MapCities_When_CitiesArePresent()
    {
        //arrange
        var cts = new CancellationTokenSource();
        var id = Guid.NewGuid();

        var expected = new Solicitor
        {
            Name = "Solicitor",
            RelativeUrl = "/-some-solicitor.html"
        };
        
        expected.Cities.Add(new City { Name = "London" });
        
        _mockRepo
            .Setup(x => x.GetSolicitorByIdAsync(It.IsAny<Guid>(), cts.Token))
            .ReturnsAsync(expected);
        // expected.Ratings.Add(new SolicitorRating { Provider = "Google", Value = 4, Maximum = 5, Solicitor = expected });
        
        
        //act
        var result = await _service.GetSolicitorInfoByIdAsync(id, cts.Token);
        
        //assert
        Assert.NotNull(result);
        Assert.Collection(result.Cities, city => Assert.Equal("London", city));
    }

    [Fact]
    public async Task GetSolicitorInfoByIdAsync_Should_MapRatings_When_RatingsArePresent()
    {
        //arrange
        var cts = new CancellationTokenSource();
        var id = Guid.NewGuid();

        var expected = new Solicitor
        {
            Name = "Solicitor",
            RelativeUrl = "/-some-solicitor.html"
        };
        expected.Ratings.Add(new SolicitorRating { Provider = "Google", Value = 4, Maximum = 5, Solicitor = expected });
        
        _mockRepo
            .Setup(x => x.GetSolicitorByIdAsync(It.IsAny<Guid>(), cts.Token))
            .ReturnsAsync(expected);
        
        //act
        var result = await _service.GetSolicitorInfoByIdAsync(id, cts.Token);
        
        //assert
        Assert.NotNull(result);
        Assert.Collection(result.Ratings, rating =>
        {
            Assert.Equal("Google", rating.Provider);
            Assert.Equal(4, rating.Value);
            Assert.Equal(5, rating.Maximum);
        });
    }
}