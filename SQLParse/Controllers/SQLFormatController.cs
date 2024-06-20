using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SQLParse.Models;
using SQLParse.Services;

namespace SQLParse.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SQLFormatController : Controller
    {
        private readonly ISQLFormatHandler _sqlFormatHandler;

        public SQLFormatController(ISQLFormatHandler sQLFormatHandler)
        {
            _sqlFormatHandler = sQLFormatHandler;
        }
        [HttpPost]
        public async Task<IActionResult> SQLFormat([FromBody] SQLFormatRequest options)
        {
            if (options == null || string.IsNullOrWhiteSpace(options.Script))
            {
                return BadRequest("Invalid input.");
            }

            var formattedSql = await _sqlFormatHandler.Format(options);
            return Ok(formattedSql);
        }
    }
}
