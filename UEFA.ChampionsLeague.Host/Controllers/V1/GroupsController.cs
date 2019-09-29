using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UEFA.ChampionsLeague.Business.Services;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects.Groups;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects.Matches;

namespace UEFA.ChampionsLeague.Host.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/[controller]")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, (typeof(ResponseTemplateViewDto<bool>)))]
    [SwaggerResponse(HttpStatusCode.BadRequest, typeof(ResponseTemplateViewDto<List<ValidationErrorDto>>))]
    public class GroupsController : ControllerBase
    {
        private readonly GroupsService _service;

        public GroupsController(GroupsService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ResponseTemplateViewDto<bool>))]
        public async Task<IActionResult> CreateAsync([FromBody]List<MatchDto> dtos)
        {
            return Ok(await _service.CreateAsync(dtos));
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ResponseTemplateViewDto<bool>))]
        public async Task<IActionResult> UpdateAsync([FromBody]List<MatchDto> dtos)
        {
            return Ok(await _service.UpdateAsync(dtos));
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ResponseTemplateViewDto<List<GroupsNavigationDto>>))]
        public async Task<IActionResult> GetGroupedByGroupNameAsync([FromQuery] List<string> queryGroup)
        {
            return Ok(await _service.GetGroupedByGroupNameAsync(queryGroup));
        }

        [HttpGet("Search")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ResponseTemplateViewDto<List<MatchDto>>))]
        public async Task<IActionResult> SearchAsync(
            [FromQuery]string fromDate = "",
            [FromQuery]string toDate = "",
            [FromQuery]string queryTeam = "",
            [FromQuery]string queryGroup = ""
        )
        {
            return Ok(await _service.SearchAsync(fromDate, toDate, queryTeam, queryGroup));
        }
    }
}
