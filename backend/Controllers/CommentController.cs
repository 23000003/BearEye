using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Dtos.Comment;
using backend.Interfaces;
using backend.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository commentRepo;
        private readonly IStockRepository stockRepo;
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            this.commentRepo = commentRepo;
            this.stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if(!ModelState.IsValid) // Modelstate is Inherited from ControllerBase
                return BadRequest(ModelState);

            var comments = await commentRepo.GetAllAsync();

            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var comment = await commentRepo.GetByIdAsync(id);

            if(comment == null){
                return NotFound("Comment Not Found");
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto comment)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            if(!await stockRepo.StockExists(stockId)){
                return BadRequest("Stock does not exists");
            }

            var commentModel = comment.ToCommentCreate(stockId);

            await commentRepo.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), 
                new { id = commentModel.Id }, commentModel.ToCommentDto()
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentDto updateDto)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var comment = await commentRepo.UpdateAsync(id, updateDto.ToCommentUpdate());

            if(comment == null){
                return NotFound("Comment Not Found");
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);
                
            var commentModel = await commentRepo.DeleteAsync(id);

            if(commentModel == null){
                return NotFound("Comment Not Found");
            }

            return Ok("Comment Successfully Deleted");
        }
    }
}