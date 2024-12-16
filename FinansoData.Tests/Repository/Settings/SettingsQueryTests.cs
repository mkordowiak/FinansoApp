using FinansoData.Data;
using FinansoData.Repository;
using FinansoData.Repository.Settings;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FinansoData.Tests.Repository.Settings
{
    public class SettingsQueryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly Mock<ICacheWrapper> _cacheWrapperMock;
        private readonly Models.Settings _settings;


        public SettingsQueryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _cacheWrapperMock = new Mock<ICacheWrapper>();


            _settings = new Models.Settings { Id = 1, Key = "TestSetting1", Value = "25", Type = "int", Description = "" };

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Settings.Add(_settings);
                context.SaveChanges();
            }
        }

        #region GetSettingsAsync

        [Fact]
        public async Task SettingsQueryRepository_GetSettingsAsync_ShouldReturnValue()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            _cacheWrapperMock.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()));

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                SettingsQueryRepository settingsQueryRepository = new SettingsQueryRepository(context, _cacheWrapperMock.Object);

                // Act
                int result = await settingsQueryRepository.GetSettingsAsync<int>(_settings.Key);

                // Assert
                result.Should().Be(int.Parse(_settings.Value));

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task SettingsQueryRepository_GetSettingsAsync_ShouldReturnCacheValue()
        {
            // Arrange
            Models.Settings outValue = _settings;
            outValue.Value = "1";

            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out outValue))
                .Returns(true);
            _cacheWrapperMock.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()));

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                SettingsQueryRepository settingsQueryRepository = new SettingsQueryRepository(context, _cacheWrapperMock.Object);

                // Act
                int result = await settingsQueryRepository.GetSettingsAsync<int>(_settings.Key);

                // Assert
                result.Should().Be(1);


                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task SettingsQueryRepository_GetSettingsAsync_ShouldSaveCache()
        {
            // Arrange
            _cacheWrapperMock.Setup(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns(false);
            _cacheWrapperMock.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()));

            using (ApplicationDbContext context = new ApplicationDbContext(_dbContextOptions))
            {
                SettingsQueryRepository settingsQueryRepository = new SettingsQueryRepository(context, _cacheWrapperMock.Object);

                // Act
                int result = await settingsQueryRepository.GetSettingsAsync<int>(_settings.Key);

                // Assert
                _cacheWrapperMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()), Times.Once);

                // Delete in-memory database
                context.Database.EnsureDeleted();
            }
        }

        #endregion
    }
}
