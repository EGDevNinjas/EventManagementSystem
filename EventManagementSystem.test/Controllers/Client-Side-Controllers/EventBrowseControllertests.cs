using EventManagementSystem.API.Controllers.Client_Side;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using EventManagementSystem.DAL.Contexts;
using EventManagementSystem.DAL.Repositories;
using FakeItEasy;
using FakeItEasy.Core;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task GetEventById_ShouldReturnOk_WhenEventIsFound()
        {
            // Arrange
            var eventItem = new Event { Title = "Test Event", Description = "Desc", Location = "Loc", CoverImage = "img.jpg" };
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetEventById(eventItem.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEvent = Assert.IsType<Event>(okResult.Value);
            Assert.Equal(eventItem.Id, returnedEvent.Id);
            Assert.Equal("Test Event", returnedEvent.Title);
        }

        [Fact]
        public async Task GetEventById_ShouldReturnNotFound_WhenEventIsNotFound()
        {
            // Act
            var result = await _controller.GetEventById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Event not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetEventById_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var mockRepository = A.Fake<IGenericRepository<Event>>();
            A.CallTo(() => mockRepository.GetByIdAsync(A<int>._)).ThrowsAsync(new Exception("Test exception"));
            var controller = new EventBrowseController(mockRepository, _cache, _logger);

            // Act
            var result = await controller.GetEventById(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while retrieving the event: Test exception", statusCodeResult.Value);
        }

        // Tests for SearchEvents
        [Fact]
        public async Task SearchEvents_ShouldReturnOk_WhenEventsAreFound()
        {
            // Arrange
            _context.Events.AddRange(
                new Event { Title = "Tech Conference", Description = "A tech event", Location = "Loc1", CoverImage = "img1.jpg" },
                new Event { Title = "Music Festival", Description = "A music event", Location = "Loc2", CoverImage = "img2.jpg" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.SearchEvents("Tech");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Single(events);
            Assert.Equal("Tech Conference", events[0].Title);
        }

        [Fact]
        public async Task SearchEvents_ShouldReturnNotFound_WhenNoEventsAreFound()
        {
            // Arrange
            _context.Events.Add(new Event { Title = "Music Festival", Description = "A music event", Location = "Loc2", CoverImage = "img2.jpg" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.SearchEvents("Tech");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No events found matching the search term", notFoundResult.Value);
        }

        // Fix for CS1061 and CS1026
        [Fact]
        public async Task SearchEvents_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var mockRepository = A.Fake<IGenericRepository<Event>>();
            A.CallTo(() => mockRepository.FindByCondition(A<Expression<Func<Event, bool>>>._))
                .Throws(new Exception("Test exception")); var controller = new EventBrowseController(mockRepository, _cache, _logger);

            // Act
            var result = await controller.SearchEvents("Tech");

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while searching for events: Test exception", statusCodeResult.Value);
        }

        // Tests for FilterEventsByCategory
        [Fact]
        public async Task FilterEventsByCategory_ShouldReturnOk_WhenEventsAreFound()
        {
            // Arrange
            //  Message: 
            //Microsoft.EntityFrameworkCore.DbUpdateException : Required properties '{'CoverImage', 'Description', 'Location'}' are missing for the instance of entity type 'Event'.Consider using 'DbContextOptionsBuilder.EnableSensitiveDataLogging' to see the entity key value.

            _context.Events.AddRange(
                new Event { Title = "Event1", CategoryId = 1, CoverImage = "img1.jpg", Description = "desc1", Location = "loc1" },
                new Event { Title = "Event2", CategoryId = 2, CoverImage = "img2.jpg", Description = "desc2", Location = "loc2" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.FilterEventsByCategory(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Single(events);
            Assert.Equal("Event1", events[0].Title);
        }

        [Fact]
        public async Task FilterEventsByCategory_ShouldReturnOkWithEmptyList_WhenNoEventsAreFound()
        {
      
            // Arrange
            _context.Events.Add(new Event { Title = "Event2", CategoryId = 2, CoverImage = "img2.jpg", Description = "desc2", Location = "loc2" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.FilterEventsByCategory(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Empty(events);
        }

        [Fact]
        public async Task FilterEventsByCategory_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var mockRepository = A.Fake<IGenericRepository<Event>>();
            A.CallTo(() => mockRepository.FindByCondition(A<Expression<Func<Event, bool>>>._)).Throws(new Exception("Test exception"));
            var controller = new EventBrowseController(mockRepository, _cache, _logger);

            // Act
            var result = await controller.FilterEventsByCategory(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while filtering events: Test exception", statusCodeResult.Value);
        }

        // Tests for GetPagedEvents
        [Fact]
        public async Task GetPagedEvents_ShouldReturnOk_WithPagedEvents()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
               
                    _context.Events.Add(new Event
                    {
                        Title = $"Event {i}",
                        CategoryId = 1,
                        CoverImage = $"img{i}.jpg",
                        Description = $"desc{i}",
                        Location = $"loc{i}"
                    });
                //new Event { Title = "Music Event", CategoryId = 4, CoverImage = "img4.jpg", Description = "desc4", Location = "loc4" }
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetPagedEvents(page: 2, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Equal(5, events.Count);
            Assert.Equal("Event 6", events.First().Title);
            Assert.Equal("Event 10", events.Last().Title);
        }

        [Fact]
        public async Task GetPagedEvents_ShouldReturnOk_WithFilteredPagedEvents()
        {
            // Arrange
            _context.Events.AddRange(
                new Event { Title = "Tech Event1", CategoryId = 1, CoverImage = "img1.jpg", Description = "desc1", Location = "loc1" },
                new Event { Title = "Tech Event2", CategoryId = 3, CoverImage = "img3.jpg", Description = "desc3", Location = "loc3" },
                new Event { Title = "Music Event", CategoryId = 4, CoverImage = "img4.jpg", Description = "desc4", Location = "loc4" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetPagedEvents(page: 1, pageSize: 2, keyword: "Tech");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Equal(2, events.Count);
            Assert.All(events, e => Assert.Contains("Tech", e.Title));
        }
        #region

        //[Fact]
        //    public async Task GetAllEvents_ShouldCacheData_WhenCacheMissOccurs()
        //    {
        //        // Arrange
        //        var events = new List<Event>
        //{
        //    new Event { Id = 1, Title = "Event 1", Description = "Desc 1", Location = "Loc 1", CoverImage = "img1.jpg" },
        //    new Event { Id = 2, Title = "Event 2", Description = "Desc 2", Location = "Loc 2", CoverImage = "img2.jpg" }
        //};

        //        var mockRepository = A.Fake<IGenericRepository<Event>>();
        //        A.CallTo(() => mockRepository.GetAll()).Returns(events.AsQueryable());

        //        // Use a real MemoryCache instance
        //        var cache = new MemoryCache(new MemoryCacheOptions());

        //        var mockLogger = A.Fake<ILogger<EventBrowseController>>();
        //        var controller = new EventBrowseController(mockRepository, cache, mockLogger);

        //        // Act
        //        var result = await controller.GetAllEvents();

        //        // Assert
        //        // Verify repository was called once
        //        A.CallTo(() => mockRepository.GetAll()).MustHaveHappenedOnceExactly();

        //        // Check if data is cached
        //        object cachedValue;
        //        var isCached = cache.TryGetValue("all_events", out cachedValue);
        //        Assert.True(isCached, "Data should be cached after a cache miss.");
        //        var cachedEvents = Assert.IsType<List<Event>>(cachedValue);
        //        Assert.Equal(2, cachedEvents.Count);

        //        // Verify the controller returned the correct data
        //        var okResult = Assert.IsType<OkObjectResult>(result);
        //        var returnedEvents = Assert.IsType<List<Event>>(okResult.Value);
        //        Assert.Equal(2, returnedEvents.Count);
        //    }
        #endregion

    }
}