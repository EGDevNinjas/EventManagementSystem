using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementSystem.API.Controllers.Boking_Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        // Book / View bookings

        IGenericRepository<Booking> _repository;
        public BookingController(IGenericRepository<Booking> repository)
        {
            _repository = repository;
        }

        //[HttpPost]



    }
}
