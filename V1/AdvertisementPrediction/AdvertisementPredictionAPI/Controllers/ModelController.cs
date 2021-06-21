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
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvertisementPrediction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly IExampleRepository _repository;
        private readonly IRestClient _client;
        public ModelController(IExampleRepository repository, IRestClient client)
        {
            this._repository = repository;
            this._client = client;
        }

        [HttpGet]
        public async Task<ActionResult> GetLearningModelsAsync()
        {
            var request = new RestRequest("trainmodel/all", Method.GET);
            IRestResponse response = await this._client.ExecuteAsync<string>(request);
            
            if ( !response.IsSuccessful )
            {
                return BadRequest($"Server response error: {response.ErrorMessage}");
            }

            return Ok(response.Content);
        }

        [HttpGet("{category}")] 
        public async Task<ActionResult> TrainModelAsync(string category)
        {   
            if ( string.IsNullOrEmpty(category) )
            {
                return BadRequest("The category is Null or Empty");
            }
            
            List<Example> LoadedExamples = await _repository.FindAllAsync(category);

            if ( LoadedExamples.Count < 6 )
            {
                return BadRequest($"You need at least 6 exmples to train a model, got: {LoadedExamples.Count}");
            }

            RestRequest request = new RestRequest("trainmodel/" + category, Method.POST, DataFormat.Json);
            request.AddJsonBody(LoadedExamples);

            IRestResponse response = await this._client.ExecuteAsync<string>(request);

            if ( !response.IsSuccessful )
            {
                return BadRequest($"Server response error: {response.ErrorMessage}");
            }

            return Ok(response.Content);
        }
    }
}