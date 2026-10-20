using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLibrary.WcfModels;
using System.IO;
using System.ServiceModel;
using UserServiceReference;
using System.Threading;

namespace WEB_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly WcfUserServiceClient client;

        public UserController()
        {
            client = new WcfUserServiceClient();
        }
        
        [HttpPost("registration")]
        public ActionResult CreateUser([FromBody] UserDtoRegistration userDto)
        {
            try
            {
                return Ok(client.AddUserAsync(userDto));
            }
            catch (FaultException e)
            {
                if (e.Message == "Email already in use!")
                    return StatusCode(StatusCodes.Status409Conflict, e.Message);
                else if (e.Message == "Username already in use!")
                    return StatusCode(StatusCodes.Status409Conflict, e.Message);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

        }

        [HttpPost("login")]
        public IActionResult LoginUser([FromBody] UserDtoLogin userdto)
        {
            try
            {
                return Ok(client.LoginUserAsync(userdto));
            }
            catch (Exception e)
            {
                if (e.Message == "There is no user with that email!")
                    return StatusCode(StatusCodes.Status404NotFound, "Invalid email provided!");
                else if (e.Message == "There is no user with that password!")
                    return StatusCode(StatusCodes.Status404NotFound, "Invalid password provided!");
                else if (e.Message == "The user hasnt been approved yet!")
                    return StatusCode(StatusCodes.Status409Conflict, "The user hasnt been approved yet!");
                else if (e.Message == "The user registration has been denied!")
                    return StatusCode(StatusCodes.Status409Conflict, "The user registration has been denied!");
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error!");
            }
        }

        [HttpGet("get-user-by-email/{email}")]
        [Authorize(Roles = "admin, buyer, seller")]
        public ActionResult GetUserByEmail(string email)
        {
            try
            {
                return Ok(client.GetUserByEmailAsync(email));

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error!");
            }

        }

        [HttpPut("update/{id:int}")]
        [Authorize(Roles = "admin, buyer, seller")]
        public ActionResult UpdateUser(int id, [FromBody] UserDtoRegistration userDtoRegistration)
        {
            try
            {
                return Ok(client.UpdateUserAsync(id, userDtoRegistration));
            }
            catch (Exception e)
            {
                if (e.Message == "Email already in use!")
                    return StatusCode(StatusCodes.Status409Conflict, "Email already in use!");
                else if (e.Message == "Username already in use!")
                    return StatusCode(StatusCodes.Status409Conflict, "Username already in use!");
                else if (e.Message == "There is no user ith that id!")
                    return StatusCode(StatusCodes.Status409Conflict, "There is no user ith that id!");
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error!");
            }

        }

        [HttpGet("{id:int}")]
        public ActionResult GetUserById(int id)
        {
            try
            {
                return Ok(client.GetUserByIdAsync(id));
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet("get-all")]
        [Authorize(Roles = "admin")]
        public ActionResult GetUsers()
        {
            try
            {
                return Ok(client.GetUsersAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpPost("update-status")]
        [Authorize(Roles = "admin")]
        public ActionResult UpdateStatus([FromBody] UserDtoStatus userDtoStatus)
        {
            try
            {
                return Ok(client.UpdateUserStatusAsync(userDtoStatus));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }
        [HttpPost("upload-picture/{email}")]
        public ActionResult UploadPicture(IFormFile image, string email)
        {
            try
            {
                return Ok(client.UploadImageMSAsync(null, email));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
