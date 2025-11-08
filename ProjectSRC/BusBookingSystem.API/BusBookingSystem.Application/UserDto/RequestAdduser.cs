using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application.UserDto
{
    public class RequestAdduser
    {
        public string Name { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public int Age { get; set; }

        public int? GenderId { get; set; }
     

        public int? CreatedBy { get; set; }



        public int? RoleId { get; set; }

    }

}
