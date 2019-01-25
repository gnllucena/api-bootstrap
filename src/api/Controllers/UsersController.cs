using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Domains.Models;
using API.Domains.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [Route("users")]
    [SwaggerTag("Create, edit, delete and retrieve users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(
            IUserService userService) 
        {
            _userService = userService; 
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Retrieve a paginated list of users",
            Description = "Retrieves only users that were created by the authenticated user"
        )]
        [SwaggerResponse(200, "List of users filtered by the informed parameters", typeof(Pagination<User>))]
        public async Task<ActionResult> List(
            [SwaggerParameter("Skip that many items before beginning to return items")][BindRequired]  int offset,
            [SwaggerParameter("Limit the number of items that are returned")][BindRequired] int limit)
        {
            var pagination = await _userService.ListAsync(offset, limit);

            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Retrieve a user by their ID",
            Description = "Retrieves user only if it were created by the authenticated user"
        )]
        [SwaggerResponse(200, "A user filtered by their ID", typeof(User))]
        public async Task<ActionResult> Get(
            [SwaggerParameter("User's ID")]int id)
        {
            var user = await _userService.GetAsync(id);

            return Ok(user);
        }
        
        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new user",
            Description = "Creates a new user if all validations are succeded"
        )]
        [SwaggerResponse(201, "The user was successfully created", typeof(User))]
        public async Task<ActionResult> Post(
            [FromBody] User user)
        {
            var created = await _userService.CreateAsync(user);

            return CreatedAtAction(nameof(Get), new { id = user.Id }, created);
        }
        
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Edits an existing user by their ID",
            Description = "Edits an existing user if all validations are succeded and were created by the authenticated user"
        )]
        [SwaggerResponse(200, "The user was successfully edited", typeof(User))]
        public async Task<ActionResult> Put(
            [SwaggerParameter("User's ID")] int id, 
            [FromBody] User user)
        {
            var edited = await _userService.UpdateAsync(id, user);

            return Ok(edited);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deletes a user by their ID",
            Description = "Deletes a user if that user is deletable and were created by the authenticated user"
        )]
        [SwaggerResponse(204, "The user was successfully deleted")]
        public async Task<ActionResult> Delete(
            [SwaggerParameter("User's ID")]int id)
        {
            await _userService.DeleteAsync(id);

            return NoContent();
        }

        [HttpPut("{id}/activate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [SwaggerResponse(204, "The user was successfully activated")]
        public async Task<ActionResult> Activate(
            [SwaggerParameter("User's ID")] int id) 
        {
            await _userService.ActivateDeactivateAsync(id, true);

            return NoContent();
        }

        [HttpPut("{id}/deactivate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [SwaggerResponse(204, "The user was successfully deactivated")]
        public async Task<ActionResult> Deactivate(
            [SwaggerParameter("User's ID")] int id) 
        {
            await _userService.ActivateDeactivateAsync(id, false);

            return NoContent();
        }
    }
}
