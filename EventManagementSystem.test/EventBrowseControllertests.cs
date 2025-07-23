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
using System.Threading.Tasks;
using Xunit;

namespace EventManagementSystem.test
{
    public class EventBrowseControllerTests
    {
        [Fact]
        public async Task GetEvents_ShouldReturnOk_whenfoundevents()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Events.AddRange(
                new Event
                {
                    Id = 1,
                    Title = "Event 1",
                    Description = "Description 1",  // Required property
                    Location = "Location 1",        // Required property
                    CoverImage = "cover1.jpg"      // Required property
                },
                new Event
                {
                    Id = 2,
                    Title = "Event 2",
                    Description = "Description 2",  // Required property
                    Location = "Location 2",        // Required property
                    CoverImage = "cover2.jpg"      // Required property
                }
            );
            await context.SaveChangesAsync();

            var repository = new GenericRepository<Event>(context);
            var mockCache = A.Fake<IMemoryCache>();
            var mockLogger = A.Fake<ILogger<EventBrowseController>>();

            object cacheValue;
            A.CallTo(() => mockCache.TryGetValue(A<string>._, out cacheValue)).Returns(false);

            var controller = new EventBrowseController(repository, mockCache, mockLogger);

            // Act
            var result = await controller.GetAllEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var events = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Equal(2, events.Count);
            Assert.Contains(events, e => e.Title == "Event 1");
            Assert.Contains(events, e => e.Title == "Event 2");
        }
    }
}