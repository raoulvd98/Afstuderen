using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvertisementPrediction.Model;
using AdvertisementPrediction.Repositories;
using AdvertisementPrediction.Commands;
using Newtonsoft.Json;

namespace AdvertisementPrediction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase
    {
        private readonly IExampleRepository _repository;
        private IAbstractExampleCommand _addCommand;
        private IAbstractExampleCommand _updateCommand;
        private IAbstractExampleCommand _deleteCommand;
        public ExampleController(IExampleRepository repository, AddExampleCommand AddCommand, UpdateExampleCommand UpdateCommand, DeleteExampleCommand DeleteCommand)
        {
            this._repository = repository;
            this._addCommand = AddCommand;
            this._updateCommand = UpdateCommand;
            this._deleteCommand = DeleteCommand;
        }

        // GET: api/Example
        // GET: api/Example?category=btw
        [HttpGet]
        public async Task<ActionResult> FindAllAsync(string category, int page = 1)
        {
            List<Example> ExamplesList = await _repository.FindAllPaginationAsync(category, page);

            if (ExamplesList == null || ExamplesList.Count == 0)
            {
                return NoContent();
            }

            return Ok(ExamplesList);
        }

        [HttpGet("count")]
        public async Task<ActionResult> GetNumberOfExamplesAsync(string category)
        {
            int NumberOfExamples = await _repository.GetNumberOfExamplesAsync(category);

            if (NumberOfExamples < 0) 
            {
                return BadRequest();
            }

            return Ok(NumberOfExamples);
        }

        // GET: api/Example/5
        [HttpGet("{id}")] 
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            if( String.IsNullOrEmpty(id) )
            {
                return BadRequest("This is not a valid Id");
            }

            Example example = await _repository.GetByIdAsync(id);

            if (example == null)
            {
                return NotFound();
            }

            return Ok(example);
        }

        // POST: api/Example/
        [HttpPost]
        public async Task<IActionResult> CreatExampleAsync ([FromBody] Example example)
        {
            if ( example == null || string.IsNullOrEmpty(example.example) ||  string.IsNullOrEmpty(example.category) )
            {
                return BadRequest("Not every inputvalue is correct");
            }

            await this._addCommand.Execute(example);
            
            return Created(example.Id, example);
        }

        // PUT: api/Example/
        [HttpPut]
        public async Task<IActionResult> UpdateExampleAsync ([FromBody] Example example)
        {
            if ( example == null || string.IsNullOrEmpty(example.example) ||  string.IsNullOrEmpty(example.category) )
            {
                return BadRequest("Not every inputvalue is correct");
            }

            await this._updateCommand.Execute(example);

            return Ok();
        }

        // DELETE: api/Example
        [HttpDelete]
        public async Task<IActionResult> DeleteExampleAsync ([FromBody] Example example)
        {
            if ( example == null || string.IsNullOrEmpty(example.example) ||  string.IsNullOrEmpty(example.category) )
            {
                return BadRequest("Not every inputvalue is correct");
            }

            await this._deleteCommand.Execute(example);

            return Ok();
        }
    }
}