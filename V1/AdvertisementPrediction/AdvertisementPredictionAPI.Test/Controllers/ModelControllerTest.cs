using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using RestSharp;
using Newtonsoft.Json.Linq;

using AdvertisementPrediction.Model;
using AdvertisementPrediction.Repositories;
using AdvertisementPrediction.Commands;
using AdvertisementPrediction.Controllers;

namespace AdvertisementPredictionAPI.UnitTests
{
    public class ModelControllerTest
    {
        private Mock<IExampleRepository> _mockRepo;
        private Mock<IRestClient> _client;
        private readonly ModelController _controller;

        public ModelControllerTest()
        {
            _mockRepo = new Mock<IExampleRepository>();
            _client = new Mock<IRestClient>();
            _controller = new ModelController(this._mockRepo.Object, this._client.Object);
        }

        private Task<List<Example>> GetTestExamples(string cat)
        {
            var examples = new List<Example>();
            examples.Add(new Example("btw", "let op! prijs ex btw en ex bpm!", false, "1"));
            examples.Add(new Example("btw", "voor de meeneemprijs is de auto rijklaar, inclusief btw, bpm, apk en leges", true, "2"));
            examples.Add(new Example("other", "Dit is een ander voorbeeld", false, "3"));
            examples.Add(new Example("btw", "let op! prijs ex btw en ex bpm!", false, "4"));
            examples.Add(new Example("btw", "voor de meeneemprijs is de auto rijklaar, inclusief btw, bpm, apk en leges", true, "5"));
            examples.Add(new Example("btw", "Dit is een ander voorbeeld", false, "6"));
            examples.Add(new Example("btw", "voor de meeneemprijs is de auto rijklaar, inclusief btw, bpm, apk en leges", true, "7"));
            examples.Add(new Example("btw", "Dit is een ander voorbeeld", false, "8"));
            return Task.FromResult(examples.Where(x => x.category == cat).ToList());
        }

        [Fact]
        public async Task getLearningModels_ReturnBadRequest()
        {
            //Arrange
            _client.Setup(x => x.ExecuteAsync<string>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<string> { StatusCode = HttpStatusCode.BadRequest, ResponseStatus  = ResponseStatus.Completed });

            //Act
            IActionResult result = await _controller.GetLearningModelsAsync();

            //Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task getLearningModels_ReturnOkObject()
        {
            //Arrange
            _client.Setup(x => x.ExecuteAsync<string>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<string> { StatusCode = HttpStatusCode.OK, ResponseStatus  = ResponseStatus.Completed });

            //Act
            IActionResult result = await _controller.GetLearningModelsAsync();

            // Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task TrainLearningModel_GetEmptyCategory(string category)
        {
            //Arrange
            _mockRepo.Setup( repo => repo.FindAllAsync(category) )
                .Returns( GetTestExamples(category) );

            //Act
            IActionResult result = await _controller.TrainModelAsync(category);

            // Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task TrainLearningModel_ReturnBadRequest()
        {
            //Arrange
            string category = "btw";
            _mockRepo.Setup( repo => repo.FindAllAsync(category) )
                .Returns(  GetTestExamples(category) );

            _client.Setup(x => x.ExecuteAsync<string>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<string> { StatusCode = HttpStatusCode.BadRequest, ResponseStatus  = ResponseStatus.Completed });

            //Act
            IActionResult result = await _controller.TrainModelAsync(category);

            // Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task TrainLearningModel_ReturnBadRequest_NotEnoughExamples()
        {
            //Arrange
            string category = "other";
            _mockRepo.Setup( repo => repo.FindAllAsync(category) )
                .Returns(  GetTestExamples(category) );

            _client.Setup(x => x.ExecuteAsync<string>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<string> { StatusCode = HttpStatusCode.OK, ResponseStatus  = ResponseStatus.Completed });

            //Act
            IActionResult result = await _controller.TrainModelAsync(category);

            // Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task TrainLearningModel_ReturnOkObject()
        {
            //Arrange
            string category = "btw";
            _mockRepo.Setup( repo => repo.FindAllAsync(category) )
                .Returns( GetTestExamples(category) );

            _client.Setup(x => x.ExecuteAsync<string>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<string> { StatusCode = HttpStatusCode.OK, ResponseStatus  = ResponseStatus.Completed });

            //Act
            IActionResult result = await _controller.TrainModelAsync(category);

            // Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
        }
    }
}