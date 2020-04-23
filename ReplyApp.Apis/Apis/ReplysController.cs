using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReplyApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReplyApp.Apis.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    //[Route("api/Replys")]
    //[Route("api/v{v:apiVersion}/Replys")]
    [Route("api/Replys")]
    [Produces("application/json")]
    [Authorize]
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
        [HttpGet("{id:int}", Name = "GetReplyById")] // Name 속성으로 RouteName 설정
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

        #region 페이징
        // 페이징
        // GET api/Replys/Page/1/10
        [HttpGet("Page/{pageNumber:int}/{pageSize:int}")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // 페이지 번호는 1, 2, 3 사용, 리포지토리에서는 0, 1, 2 사용
                int pageIndex = (pageNumber > 0) ? pageNumber - 1 : 0;

                var resultSet = await _repository.GetAllAsync(pageIndex, pageSize);
                if (resultSet.Records == null)
                {
                    return NotFound($"아무런 데이터가 없습니다.");
                }

                // 응답 헤더에 총 레코드 수를 담아서 출력
                Response.Headers.Add("X-TotalRecordCount", resultSet.TotalRecords.ToString());
                Response.Headers.Add("Access-Control-Expose-Headers", "X-TotalRecordCount");

                return Ok(resultSet.Records);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }
        #endregion

        #region 입력
        // 입력
        // POST api/Replys
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] Reply dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var temp = new Reply();
            temp.Name = dto.Name;
            temp.Title = dto.Title;
            temp.Content = dto.Content;
            temp.Created = DateTime.Now;

            try
            {
                var model = await _repository.AddAsync(temp);
                if (model == null)
                {
                    return BadRequest();
                }

                //[!] 다음 항목 중 원하는 방식 사용
                if (DateTime.Now.Second % 60 == 0)
                {
                    return Ok(model); // 200 OK
                }
                else if (DateTime.Now.Second % 3 == 0)
                {
                    return CreatedAtRoute("GetReplyById", new { id = model.Id }, model); // Status: 201 Created
                }
                else if (DateTime.Now.Second % 2 == 0)
                {
                    var uri = Url.Link("GetReplyById", new { id = model.Id });
                    return Created(uri, model); // 201 Created
                }
                else
                {
                    // GetById 액션 이름 사용해서 입력된 데이터 반환 
                    return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }
        #endregion

        #region 수정
        // 수정
        // PUT api/Replys/123
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAsync(int id, [FromBody] Reply dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                dto.Id = id;
                var status = await _repository.EditAsync(dto);
                if (!status)
                {
                    return BadRequest();
                }

                // 204 No Content
                return NoContent(); // 이미 전송된 정보에 모든 값 가지고 있기에...
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }
        #endregion

        #region 삭제
        // 삭제
        // DELETE api/Replys/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var status = await _repository.DeleteAsync(id);
                if (!status)
                {
                    return BadRequest();
                }

                return NoContent(); // 204 NoContent
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("삭제할 수 없습니다.");
            }
        }
        #endregion
    }

    [ApiVersion("2.0")]
    [ApiController]
    //[Route("api/v{v:apiVersion}/Replys")]
    [Route("api/Replys")]
    [Produces("application/json")]
    public class ReplysV2_0Controller : ControllerBase
    {
        private readonly IReplyRepository _repository;
        private readonly ILogger _logger;

        public ReplysV2_0Controller(IReplyRepository repository, ILoggerFactory loggerFactory)
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
                return Ok(models.OrderBy(m => m.Id)); // 200 OK
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
        [HttpGet("{id:int}", Name = "GetReplyById")] // Name 속성으로 RouteName 설정
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

        #region 페이징
        // 페이징
        // GET api/Replys/Page/1/10
        [HttpGet("Page/{pageNumber:int}/{pageSize:int}")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // 페이지 번호는 1, 2, 3 사용, 리포지토리에서는 0, 1, 2 사용
                int pageIndex = (pageNumber > 0) ? pageNumber - 1 : 0;

                var resultSet = await _repository.GetAllAsync(pageIndex, pageSize);
                if (resultSet.Records == null)
                {
                    return NotFound($"아무런 데이터가 없습니다.");
                }

                // 응답 헤더에 총 레코드 수를 담아서 출력
                Response.Headers.Add("X-TotalRecordCount", resultSet.TotalRecords.ToString());
                Response.Headers.Add("Access-Control-Expose-Headers", "X-TotalRecordCount");

                return Ok(resultSet.Records);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }
        #endregion

        #region 입력
        // 입력
        // POST api/Replys
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] Reply dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var temp = new Reply();
            temp.Name = dto.Name;
            temp.Title = dto.Title;
            temp.Content = dto.Content;
            temp.Created = DateTime.Now;

            try
            {
                var model = await _repository.AddAsync(temp);
                if (model == null)
                {
                    return BadRequest();
                }

                //[!] 다음 항목 중 원하는 방식 사용
                if (DateTime.Now.Second % 60 == 0)
                {
                    return Ok(model); // 200 OK
                }
                else if (DateTime.Now.Second % 3 == 0)
                {
                    return CreatedAtRoute("GetReplyById", new { id = model.Id }, model); // Status: 201 Created
                }
                else if (DateTime.Now.Second % 2 == 0)
                {
                    var uri = Url.Link("GetReplyById", new { id = model.Id });
                    return Created(uri, model); // 201 Created
                }
                else
                {
                    // GetById 액션 이름 사용해서 입력된 데이터 반환 
                    return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }
        #endregion

        #region 수정
        // 수정
        // PUT api/Replys/123
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAsync(int id, [FromBody] Reply dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                dto.Id = id;
                var status = await _repository.EditAsync(dto);
                if (!status)
                {
                    return BadRequest();
                }

                // 204 No Content
                return NoContent(); // 이미 전송된 정보에 모든 값 가지고 있기에...
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest();
            }
        }
        #endregion

        #region 삭제
        // 삭제
        // DELETE api/Replys/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var status = await _repository.DeleteAsync(id);
                if (!status)
                {
                    return BadRequest();
                }

                return NoContent(); // 204 NoContent
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest("삭제할 수 없습니다.");
            }
        }
        #endregion
    }
}
