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
using Newtonsoft.Json;

using AdvertisementPrediction.Model;
using AdvertisementPrediction.Repositories;
using AdvertisementPrediction.Commands;
using AdvertisementPrediction.Controllers;

namespace AdvertisementPredictionAPI.UnitTests
{
    public class PredictionControllerTest
    {
        private Mock<IAdvertisementRepository> _mockAdvertisementRepo;
        private Mock<IPredictionRepository> _mockPredictionRepo;
        private Mock<IRestClient> _client;
        private readonly PredictionController _controller;
        private Mock<AdvertisementPredictionContext> _mockContext;
        private Mock<AddPredictionCommand> _addCommand;
        private Mock<UpdatePredictionCommand> _updateCommand;
        private Mock<DeletePredictionCommand> _deleteCommand;

        public PredictionControllerTest()
        {
            _mockAdvertisementRepo = new Mock<IAdvertisementRepository>();
            _mockPredictionRepo = new Mock<IPredictionRepository>();
            _client = new Mock<IRestClient>();
            _mockContext = new Mock<AdvertisementPredictionContext>();
            _addCommand = new Mock<AddPredictionCommand>(_mockContext.Object);
            _updateCommand = new Mock<UpdatePredictionCommand>(_mockContext.Object);
            _deleteCommand = new Mock<DeletePredictionCommand>(_mockContext.Object);
            _controller = new PredictionController(this._mockAdvertisementRepo.Object, this._mockPredictionRepo.Object, this._client.Object, this._addCommand.Object, this._updateCommand.Object, this._deleteCommand.Object);
        }

        public Task<List<Prediction>> getTestPredictions()
        {
            var predictions = new List<Prediction>();
            predictions.Add( new Prediction { AdvertisementId = "1",
                Model = "btw",
                GoodSentence = 0.00234754,
                WrongSentence = 0.987763,
                Date =  new DateTime (2020, 05, 13, 10, 12, 02) } );
            predictions.Add( new Prediction { AdvertisementId = "1",
                Model = "test",
                GoodSentence = 0.898009,
                WrongSentence = 0.0129874,
                Date =  new DateTime (2020, 05, 13, 10, 12, 02)  } );
            return Task.FromResult(predictions);
        }

        public static IEnumerable<Object[]> WrongPredictions
        => new Object[][] {
            new Object[] { new Prediction { PredictionId = "", AdvertisementId = "1",  Model = "test", GoodSentence = 0.898009, WrongSentence = 0.0129874, Date =  new DateTime (2020, 05, 13, 10, 12, 02)  } },
            new Object[] { new Prediction { PredictionId = "1", AdvertisementId = "",  Model = "test", GoodSentence = 0.898009, WrongSentence = 0.0129874, Date =  new DateTime (2020, 05, 13, 10, 12, 02)  } },
            new Object[] { new Prediction { PredictionId = "1", AdvertisementId = "1",  Model = null, GoodSentence = 0.898009, WrongSentence = 0.0129874, Date =  new DateTime (2020, 05, 13, 10, 12, 02)  } },
        };

        #region Get new prediction

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetNewPredictionSingleAdvertisement_ReturnBadRequest_EmptyId(string id)
        {
            //Arrange
            _mockAdvertisementRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( Task.FromResult( new Advertisement{Id = "1", AdvertisementContent = null } ) );
            
            //Act
            IActionResult result = await _controller.GetNewPredictionAsync(id);
            
            //Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task GetNewPredictionSingleAdvertisement_ReturnNotFound()
        {
            // Arrange
            _mockAdvertisementRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( Task.FromResult( new Advertisement{Id = "1", AdvertisementContent = null } ) );
            string nonExistentAdvertisementId = "This Is A Non Valid Id";

            // Act
            IActionResult result = await _controller.GetNewPredictionAsync(nonExistentAdvertisementId);

            // Assert
            Assert.True(result is NotFoundResult);
            Assert.Equal(404, (result as NotFoundResult).StatusCode);
        }

        [Fact]
        public async Task GetNewPredictionSingleAdvertisement_ServerResponseFailure()
        {
            //Arrange
            _mockAdvertisementRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( Task.FromResult( new Advertisement{Id = "1", AdvertisementContent = null } ) );
            string advertisementId = "1";

            _client.Setup(x => x.ExecuteAsync<string>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<string> { StatusCode = HttpStatusCode.BadRequest, ResponseStatus  = ResponseStatus.Completed });

            //Act
            ActionResult result = await _controller.GetNewPredictionAsync(advertisementId);
            
            //Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task GetNewPredictionSingleAdvertisement_ReturnOkObjectWithExample()
        {
            //Arrange
            _mockAdvertisementRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( Task.FromResult( new Advertisement{Id = "1", AdvertisementContent = null } ) );
            string advertisementId = "1";

            _client.Setup(x => x.ExecuteAsync<string>(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse<string> { StatusCode = HttpStatusCode.OK, ResponseStatus  = ResponseStatus.Completed, Content = " [ {'model': 'btw',  'WrongSentence': 0.0, 'GoodSentence': 0.0}, {'model': 'priceAllIn',  'WrongSentence': 0.87, 'GoodSentence': 0.13} ]" });

            //Act
            ActionResult result = await _controller.GetNewPredictionAsync(advertisementId);

            //Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
        }

        #endregion
        #region Get All Advertisement predictions
        
        [Fact]
        public async Task GetAllPredictions_ReturnNoContent()
        {
            _mockPredictionRepo.Setup( repo => repo.FindAllAsync(null, null, false, 1, 10) )
                .Returns( Task.FromResult( new List<Prediction>() ) );

            //Act
            ActionResult result = await _controller.GetAllAdvertisementWithPrediction(null, null);

            //Assert
            Assert.True(result is NoContentResult);
            Assert.Equal(204, (result as NoContentResult).StatusCode);
        }

        [Fact]
        public async Task GetAllPredictions_ReturnOkObjectWithExample()
        {
            _mockPredictionRepo.Setup( repo => repo.FindAllAsync(null, null, false, 1, 10) )
                .Returns( getTestPredictions() );

            //Act
            ActionResult result = await _controller.GetAllAdvertisementWithPrediction(null, null);      

            //Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
        }

        #endregion
        #region Get existing prediction

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetPredictionSingleAdvertisement_ReturnBadRequestEmptyString(string id)
        {
            //Arrange
            _mockPredictionRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( getTestPredictions() );

            //Act
            ActionResult result = await _controller.GetPredictionByIdAsync(id);           

            //Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task GetPredictionSingleAdvertisement_ReturnNotFound()
        {
            _mockPredictionRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( getTestPredictions() );
            string id = "2";

            //Act
            ActionResult result = await _controller.GetPredictionByIdAsync(id);           

            //Assert
            Assert.True(result is NotFoundObjectResult);
            Assert.Equal(404, (result as NotFoundObjectResult).StatusCode);
        }

        [Fact]
        public async Task GetPredictionSingleAdvertisement_ReturnOkObjectWithExample()
        {
            _mockPredictionRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( getTestPredictions() );
            string id = "1";

            //Act
            ActionResult result = await _controller.GetPredictionByIdAsync(id);           

            //Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
        }

        #endregion
        #region Count predictions
        [Fact]
        public void CountPredictions_ReturnBadRequest()
        {
            //Arrange
            _mockPredictionRepo.Setup( repo => repo.GetNumberOfPredictions(null, null) )
                .Returns( -1 );
            
            //Act
            IActionResult result = _controller.GetNumberOfPredictions();

            //Assert
            Assert.True(result is BadRequestResult);
            Assert.Equal(400, (result as BadRequestResult).StatusCode);
        }


        [Fact]
        public void CountPredictions_ReturnOk()
        {
            //Arrange
            _mockPredictionRepo.Setup( repo => repo.GetNumberOfPredictions(null, null) )
                .Returns( 4 );
            
            //Act
            IActionResult result = _controller.GetNumberOfPredictions();

            //Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
            Assert.True((int)(result as OkObjectResult).Value > 0);
        }
        #endregion

        #region Update an existing prediction

        [Theory]
        [MemberData(nameof(WrongPredictions))]
        public async Task UpdatePrediction_ReturnBadRequest_EmptyParameter( Prediction prediction )
        {
            //Arrange
            _updateCommand.Setup( command => command.Execute(prediction) );

            //Act
            IActionResult result = await _controller.ChangePredictionAsync(prediction);
            
            //Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task UpdatePrediction_ReturnOk()
        {
            //Arrange
            Prediction prediction = new Prediction { PredictionId = "1", AdvertisementId = "1",  Model = "test", GoodSentence = 0.898009, WrongSentence = 0.0129874, Date =  new DateTime (2020, 05, 13, 10, 12, 02)  };
            _addCommand.Setup( repo => repo.Execute(prediction) );
            
            //Act
            IActionResult result = await _controller.ChangePredictionAsync(prediction);
            
            //Assert
            Assert.True(result is OkResult);
            Assert.Equal(200, (result as OkResult).StatusCode);
        }

        #endregion
        #region Delete an existing prediction
    
        [Theory]
        [MemberData(nameof(WrongPredictions))]
        public async Task DeletePrediction_ReturnBadRequest_EmptyParameter( Prediction prediction)
        {
            //Arrange
            _deleteCommand.Setup( command => command.Execute(prediction) );

            //Act
            IActionResult result = await _controller.DeletePredictionAsync(prediction);
            
            //Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task DeletePrediction_ReturnOk()
        {
            //Arrange
            Prediction prediction = new Prediction { PredictionId = "1", AdvertisementId = "1",  Model = "test", GoodSentence = 0.898009, WrongSentence = 0.0129874, Date =  new DateTime (2020, 05, 13, 10, 12, 02)  };
            _addCommand.Setup( repo => repo.Execute(prediction) );
            
            //Act
           IActionResult result = await _controller.DeletePredictionAsync(prediction);
            
            //Assert
            Assert.True(result is OkResult);
            Assert.Equal(200, (result as OkResult).StatusCode);
        }
    
        #endregion
    }
}