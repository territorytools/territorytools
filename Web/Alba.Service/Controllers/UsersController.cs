using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TerritoryTools.Web.Alba.Service
{
    [ApiController]
    [Route("users")]
    //[Authorize(Policy = LocalApi.PolicyName, Roles = Roles.Admin)]
    //[Authorize(Roles = Roles.Admin)]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetAsync()
        {
            // var user = await userManager.FindByIdAsync(id.ToString());

            // if (user == null)
            // {
            //      return NotFound();
            // }

            var user = new UserDto(
                UserName: "marc",
                Name: "Marc",
                Email: "marc@durham.fam",
                AlbaRole: "Owner"
            );
            return user;
            //return user.AsDto();
        }

    }
}