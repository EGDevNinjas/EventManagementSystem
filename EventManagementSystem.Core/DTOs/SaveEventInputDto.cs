using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.DTOs
{
    public class SaveEventInputDto
    {
        [Required]
        public int EventId { get; set; }
    }
}
