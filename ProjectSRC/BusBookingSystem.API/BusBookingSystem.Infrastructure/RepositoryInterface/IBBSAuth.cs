using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusBookingSystem.Domain.Models;

namespace BusBookingSystem.Infrastructure.RepositoryInterface
{
    public interface IBBSAuth
    {
       Task <Tbluser> Login(string email, string password);
    }
}
