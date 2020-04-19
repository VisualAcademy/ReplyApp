using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReplyApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReplyApp.Apis.Controllers
{
    [ApiController]
    [Route("api/Replys")]
    [Produces("application/json")]
    public class ReplysController : ControllerBase
    {
        private readonly IReplyRepository _repository;
        private readonly ILogger _logger;

        public ReplysController(IReplyRepository repository, ILoggerFactory loggerFactory)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(ReplysController));
            this._logger = loggerFactory.CreateLogger(nameof(ReplysController));
        }

        #region 출력
        // 출력
        // GET api/Replys
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var models = await _repository.GetAllAsync();
                if (!models.Any())
                {
                    return new NoContentResult(); // 참고용 코드
                }
                return Ok(models); // 200 OK
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }
        #endregion

        #region 상세
        // 상세
        // GET api/Replys/123
        [HttpGet("{id:int}", Name = "GetReplyById")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var model = await _repository.GetByIdAsync(id);
                if (model == null)
                {
                    //return new NoContentResult(); // 204 No Content
                    return NotFound();
                }
                return Ok(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }
        #endregion
    }
}
