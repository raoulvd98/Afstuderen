using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvertisementPrediction.Repositories;
using AdvertisementPrediction.Commands;
using AdvertisementPrediction.Model;
using RestSharp;
using RestSharp.Serializers;
using RestSharp.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvertisementPrediction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IPredictionRepository _predictionRepository;
        private readonly IRestClient _client;
        private IAbstractPredictionCommand _addCommand;
        private IAbstractPredictionCommand _updateCommand;
        private IAbstractPredictionCommand _deleteCommand;
        public PredictionController(IAdvertisementRepository advertisementRepository, IPredictionRepository predictionRepository, IRestClient client, 
            AddPredictionCommand AddCommand, UpdatePredictionCommand UpdateCommand, DeletePredictionCommand DeleteCommand)
        {
            this._advertisementRepository = advertisementRepository;
            this._predictionRepository = predictionRepository;
            this._client = client;
            this._addCommand = AddCommand;
            this._updateCommand = UpdateCommand;
            this._deleteCommand = DeleteCommand;
        }

        [HttpPost("{id}")] 
        public async Task<ActionResult> GetNewPredictionAsync(string id)
        {   
            if ( string.IsNullOrEmpty(id) )
            {
                return BadRequest("The id is Null or Empty");
            }
            
            Advertisement LoadedAdvertisement = await _advertisementRepository.GetByIdAsync(id);

            if ( LoadedAdvertisement == null || LoadedAdvertisement.GetType() != typeof( Advertisement ))
            {
                return NotFound();
            }

            IRestRequest request = new RestRequest("predict", Method.POST, DataFormat.Json);
            request.AddJsonBody(LoadedAdvertisement);

            IRestResponse response = await this._client.ExecuteAsync<string>(request);

            if ( ! response.IsSuccessful )
            {
                return BadRequest($"Server response error: {response.ErrorMessage}");
            }

            List<Prediction> listOfPredictions = JsonConvert.DeserializeObject<List<Prediction>>(response.Content);
            foreach (Prediction prediction in listOfPredictions)
            {
                prediction.PredictionId = Guid.NewGuid().ToString();
                prediction.AdvertisementId = id;
                prediction.Date = DateTime.Now;
                await this._addCommand.Execute(prediction);
            }

            return Ok(response.Content);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAdvertisementWithPrediction(string model = null, bool? positive = null, bool orderByModel = false, int page = 1)
        {
            List<Prediction> Predictions = await this._predictionRepository.FindAllAsync(model, positive, orderByModel, page);

            if ( Predictions == null || !Predictions.Any() )
            {
                return NoContent();
            }

            return Ok(Predictions);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult> GetPredictionByIdAsync (string id)
        {
            if ( string.IsNullOrEmpty(id) )
            {
                return BadRequest("The id is Null or Empty");
            }

            List<Prediction> Predictions = await this._predictionRepository.GetByIdAsync(id);

            if ( Predictions == null || !Predictions.Any() )
            {
                return NotFound("Er zijn nog geen voorspellingen gedaan voor deze advertentie!");
            }

            return Ok(Predictions);
        }

        [HttpGet("count")]
        public ActionResult GetNumberOfPredictions(string model = null, bool? positive = null)
        {
            int NumberOfExamples = _predictionRepository.GetNumberOfPredictions(model, positive);

            if (NumberOfExamples < 0) 
            {
                return BadRequest();
            }

            return Ok(NumberOfExamples);
        }

        [HttpPut]
        public async Task<IActionResult> ChangePredictionAsync ([FromBody] Prediction prediction)
        {
            if ( prediction == null || string.IsNullOrEmpty(prediction.AdvertisementId) ||  string.IsNullOrEmpty(prediction.PredictionId) ||  string.IsNullOrEmpty(prediction.Model)  )
            {
                return BadRequest("Not every inputvalue is correct");
            }

            await this._updateCommand.Execute(prediction);

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePredictionAsync ([FromBody] Prediction prediction)
        {
            if ( prediction == null || string.IsNullOrEmpty(prediction.AdvertisementId) ||  string.IsNullOrEmpty(prediction.PredictionId) ||  string.IsNullOrEmpty(prediction.Model)  )
            {
                return BadRequest("Not every inputvalue is correct");
            }

            await this._deleteCommand.Execute(prediction);

            return Ok();
        }
    }
}