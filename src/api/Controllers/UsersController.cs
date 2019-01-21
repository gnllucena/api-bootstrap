using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [Route("users")]
    [SwaggerTag("Create, edit, delete and retrieve users")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(
            Summary = "Retrieve a paginated list of users",
            Description = "Retrieves only users that were created by the authenticated user"
        )]
        [SwaggerResponse(200, "List of users filtered by the informed parameters", typeof(Pagination<User>))]
        public ActionResult<Pagination<User>> List()
        {
            var pagination = new Pagination<User>();

            pagination.Items = new List<User> 
            {
                new User() { Profile = Profile.Administrator, Country = Country.Kanto },
                new User() { Profile = Profile.Regular },
                new User() { Profile = Profile.Administrator },
            };

            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Retrieve a user by their ID",
            Description = "Retrieves user only if it were created by the authenticated user"
        )]
        [SwaggerResponse(200, "A user filtered by their ID", typeof(User))]
        public ActionResult<User> Get(
            [SwaggerParameter("User's ID")]int id)
        {
            var user = new User() { Profile = Profile.Administrator };

            return Ok(user);
        }
        
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new user",
            Description = "Creates a new user if all validations are succeded"
        )]
        [SwaggerResponse(201, "The user was successfully created", typeof(User))]
        public ActionResult<User> Post([FromBody] User user)
        {
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }
        
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Edits an existing user by their ID",
            Description = "Edits an existing user if all validations are succeded and were created by the authenticated user"
        )]
        [SwaggerResponse(200, "The user was successfully edited", typeof(User))]
        public ActionResult<User> Put(
            [SwaggerParameter("User's ID")]int id, 
            [FromBody] User user)
        {
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deletes a user by their ID",
            Description = "Deletes a user if that user is deletable and were created by the authenticated user"
        )]
        [SwaggerResponse(204, "The user was successfully deleted")]
        public ActionResult Delete(
            [SwaggerParameter("User's ID")]int id)
        {
            return NoContent();
        }

        [HttpPut("{id}/activate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Activate(int id) 
        {
            return NoContent();
        }

        [HttpPut("{id}/deactivate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Deactivate(int id) 
        {
            return NoContent();
        }
    }
}
