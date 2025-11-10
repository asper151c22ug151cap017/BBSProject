using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application
{
    public class Messages
    {
        public static class Auth
        {
            public const string ValidateEmail = "EmailId id requerid";
            public const string InvalidPassword = " Invalid Password";
            public const string Invalid = "Invalid Email or Password";
            public const string Login = "Login Successfully";
            public const string Common = "An error During the login time can you check the server";
        }

            public static class User
            {
                public const string FetchSuccess = "Fetch successfully";
                public const string Registeruser = "Registered user successfully";
                public const string update = "update userinfo Succcessfully";
                public const string Existingemail = "Email Already Exist";
                public static string nullable = "Please Enter the fields";
                 public const string vaild = "Enter a vaild date ";
                public const string Failed = "An error occurred while Register user";
                public const string updatedatas = "Invalid user data.";
                 public const string updateprofile = "Error update user profile information by user";
                 public const string updateusers = "Invalid or missing user identity";
                public const string checkuserid = "Enter a valid userId";
                public const string NotFound = "User id not founded";
                public const string faildupdate = "An error occurred while updating the user.";
                public const string Deleted = "Delete User Successfully";
                public const string FailedDelete = "An error occurred while Delete the user";
                public const string exceptionadd = "Error while adding a new user";
               public const string exceptionupdate  = "Error while upddate a  user";
                public const string exceptiondelete = "Error while Delete user";
                 public const string failedtuserbyid = "An error occurred while Fetch user details";
                    public const string exceptiongetuserbyid = "Error while fetching user by id ";
                    public const string Controller = "Something went wrong";
                    public const string exceptiongetuser = "Error while fetching user by id ";
            public const string Createdby = "Invalid or missing user identity";
        }
        public static class BusMessage
        {
            public const string ValidateBusinfo = "BusNumber or Operator Number already exist ";
            public const string Added = "Added successfully ";
            public const string Updated = "Updated Successfully ";
            public const string Delete = "Dalete Successfully ";
            public const string Exceptionaddbus = "An Error while Add Buses";
            public const string ExceptionUpdatebus = "An Error while Update Buses";
            public const string Exceptiondeletebus = "An Error while Delete Buses";
            public const string ExceptionGetbus = "An Error while Fetch Buses";
            public const string NotFound = "Bus Not Founded";

        }
        public static class RoutesMessage
        {
            public const string ExceptionaddRoutes = "An Error while Add Routes";
            public const string ExceptionUpdateroutes = "An Error while Update Routes";
            public const string Exceptiondeleteroutes = "An Error while Delete Routes";
            public const string ExceptiongetRoutes = "An Error while Fetch All Routes";
            public const string ExceptionFilterroutes = "Error Filtrer and flecth routes";
            public const string Required = "Source, Destination, and valid TravelDate are required.";
            public const string ExceptiongetRoutecount = "An Error while Fetch Routes count";
            public const string validation = "Invalid Input";
            public const string BusIdvalidation = "Invalid bus id";
            public const string required = "Date is required";
            public const string NotFound = "Routeid Not Founded";

        }

    }
}
