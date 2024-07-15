using Business;
using DataAccess.Concrete;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private LogService logService;

        public LogController(LogService logService)
        {
            this.logService = logService;
        }

        [HttpPost]
        public async Task<IActionResult> Save(LogModel model)
        {
            var isSuccess = await logService.SaveAsync(model);

           
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await logService.GetAllLogAsync();

            return Ok(result);
        }
    }
}
