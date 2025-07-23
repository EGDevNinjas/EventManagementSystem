using EventManagementSystem.API.Controllers.Client_Side;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using EventManagementSystem.DAL.Contexts;
using EventManagementSystem.DAL.Repositories;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EventManagementSystem.Tests
{
    public class EventBrowseControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ApplicationDbContext _context;
        private readonly IGenericRepository<Event> _repository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<EventBrowseController> _logger;
        private readonly EventBrowseController _controller;

        public EventBrowseControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(_options);
            _repository = new GenericRepository<Event>(_context);
            _cache = A.Fake<IMemoryCache>();
            _logger = A.Fake<ILogger<EventBrowseController>>();
            _controller = new EventBrowseController(_repository, _cache, _logger);
        }

        [Fact]
        public async Task GetAllEvents_ShouldReturnOk_WhenEventsAreFound()
        {
            // Arrange
            _context.Events.AddRange(
                new Event { Id = 1, Title = "Event 1", Description = "Desc 1", Location = "Loc 1", CoverImage = "img1.jpg" },
                new Event { Id = 2, Title = "Event 2", Description = "Desc 2", Location = "Loc 2", CoverImage = "img2.jpg" }
            );
            await _context.SaveChangesAsync();

            object cacheValue;
            A.CallTo(() => _cache.TryGetValue(A<string>._, out cacheValue)).Returns(false);

            // Act
            var result = await _controller.GetAllEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Equal(2, events.Count);
        }

        [Fact]
        public async Task GetAllEvents_ShouldReturnOk_WhenEventsAreCached()
        {
            // Arrange
            var mockRepository = A.Fake<IGenericRepository<Event>>();
            var mockCache = A.Fake<IMemoryCache>();
            var mockLogger = A.Fake<ILogger<EventBrowseController>>();

            var cachedEvents = new List<Event>
            {
                new Event { Id = 1, Title = "Cached Event 1", Description = "Desc 1", Location = "Loc 1", CoverImage = "img1.jpg" },
                new Event { Id = 2, Title = "Cached Event 2", Description = "Desc 2", Location = "Loc 2", CoverImage = "img2.jpg" }
            };

            object cacheValue = cachedEvents;
            A.CallTo(() => mockCache.TryGetValue(A<string>._, out cacheValue)).Returns(true);

            var controller = new EventBrowseController(mockRepository, mockCache, mockLogger);

            // Act
            var result = await controller.GetAllEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Equal(2, events.Count);
            Assert.Contains(events, e => e.Title == "Cached Event 1");
            Assert.Contains(events, e => e.Title == "Cached Event 2");
            A.CallTo(() => mockRepository.GetAll()).MustNotHaveHappened(); // Verify repository not called
        }
        [Fact]
        public async Task GetAllEvents_ShouldReturnOk_WhenNoEventsAreFound()
        {
            // Arrange
            object cacheValue;
            A.CallTo(() => _cache.TryGetValue(A<string>._, out cacheValue)).Returns(false);

            // Act
            var result = await _controller.GetAllEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Empty(events);
        }

        [Fact]
        public async Task GetAllEvents_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var mockRepository = A.Fake<IGenericRepository<Event>>();
            A.CallTo(() => mockRepository.GetAll()).Throws(new Exception("Database error"));

            var controller = new EventBrowseController(mockRepository, _cache, _logger);

            object cacheValue;
            A.CallTo(() => _cache.TryGetValue(A<string>._, out cacheValue)).Returns(false);

            // Act
            var result = await controller.GetAllEvents();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while retrieving the events: Database error", statusCodeResult.Value);
        }
        [Fact]
        public async Task GetAllEvents_ShouldCacheData_WhenCacheMissOccurs()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb_Cache_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Events.AddRange(
                new Event { Id = 1, Title = "Event 1", Description = "Desc 1", Location = "Loc 1", CoverImage = "img1.jpg" },
                new Event { Id = 2, Title = "Event 2", Description = "Desc 2", Location = "Loc 2", CoverImage = "img2.jpg" }
            );
            await context.SaveChangesAsync();

            var mockRepository = A.Fake<IGenericRepository<Event>>();
            A.CallTo(() => mockRepository.GetAll()).Returns(context.Events.AsQueryable());

            var mockCache = A.Fake<IMemoryCache>();
            var mockLogger = A.Fake<ILogger<EventBrowseController>>();

            // Simulate initial cache miss
            object initialCacheValue;
            A.CallTo(() => mockCache.TryGetValue("all_events", out initialCacheValue)).Returns(false);

            // Set up to verify cached data on a subsequent call
            var controller = new EventBrowseController(mockRepository, mockCache, mockLogger);
            await controller.GetAllEvents(); // First call to cache the data

            // Act: Check if data is cached by attempting to retrieve it
            object cachedValue;
            var isCached = mockCache.TryGetValue("all_events", out cachedValue);

            // Assert
            Assert.True(isCached); // Verify the data was cached
            var cachedEvents = Assert.IsType<List<Event>>(cachedValue);
            Assert.Equal(2, cachedEvents.Count);
            Assert.Contains(cachedEvents, e => e.Title == "Event 1");
            Assert.Contains(cachedEvents, e => e.Title == "Event 2");
            A.CallTo(() => mockLogger.LogInformation("✅ Events have been cached.")).MustHaveHappened();
        }
    }
}