using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.DTOs
{
    public class RatingSummaryDto
    {
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
    }
}
