using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;

using AdvertisementPrediction.Model;
using AdvertisementPrediction.Repositories;
using AdvertisementPrediction.Commands;
using AdvertisementPrediction.Controllers;

namespace AdvertisementPredictionAPI.UnitTests
{
    public class ExampleControllerTest
    {
        private Mock<IExampleRepository> _mockRepo;
        private readonly ExampleController _controller;
        private Mock<AddExampleCommand> _addCommand;
        private Mock<UpdateExampleCommand> _updateCommand;
        private Mock<DeleteExampleCommand> _deleteCommand;

        public ExampleControllerTest()
        {
            _mockRepo = new Mock<IExampleRepository>();
            _addCommand = new Mock<AddExampleCommand>();
            _updateCommand = new Mock<UpdateExampleCommand>();
            _deleteCommand = new Mock<DeleteExampleCommand>();
            _controller = new ExampleController(this._mockRepo.Object, this._addCommand.Object, this._updateCommand.Object, this._deleteCommand.Object);
        }
        
        private Task<List<Example>> GetTestExamples(string cat)
        {
            var examples = new List<Example>();
            examples.Add(new Example("btw", "let op! prijs ex btw en ex bpm!", false, "1"));
            examples.Add(new Example("btw", "voor de meeneemprijs is de auto rijklaar, inclusief btw, bpm, apk en leges", true, "2"));
            examples.Add(new Example("other", "Dit is een ander voorbeeld", false, "3"));
            return Task.FromResult(examples.Where(x => x.category == cat).ToList());
        }

        private Task<int> CountExamples(string cat)
        {
             var examples = new List<Example>();
            examples.Add(new Example("btw", "let op! prijs ex btw en ex bpm!", false, "1"));
            examples.Add(new Example("btw", "voor de meeneemprijs is de auto rijklaar, inclusief btw, bpm, apk en leges", true, "2"));
            examples.Add(new Example("other", "Dit is een ander voorbeeld", false, "3"));
            return Task.FromResult(examples.Where(x => x.category == cat).Count());
        }
        public static IEnumerable<Object[]> WrongExamples
        => new Object[][] {
            new Object[] { new Example("", "let op! prijs ex btw en ex bpm!", false, "1") },
            new Object[] { new Example("btw", "", false, "1") },
            new Object[] { new Example(null, null, false, null) },
        };

        #region Get all examples

        [Fact]
        public async Task GetExamples_FindExamplesByCategory_GetEmptyList()
        {
            //Arrange
            string category = "EmptyCat";
            _mockRepo.Setup( repo => repo.FindAllPaginationAsync(category, 1, 10) )
                .Returns( GetTestExamples(category) );
            
            //Act
            IActionResult result = await _controller.FindAllAsync(category);

            //Assert
            Assert.True(result is NoContentResult);
            Assert.Equal(204, (result as NoContentResult).StatusCode);
        }

        [Fact]
        public async Task GetExamples_FindExamplesByCategory()
        {
            // Arrange
            string category = "btw";
            _mockRepo.Setup( repo => repo.FindAllPaginationAsync(category, 1, 10) )
                .Returns( GetTestExamples(category) );

            // Act
            IActionResult result = await _controller.FindAllAsync(category);

            // Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);

            // var exampleList = okObjectResult.Value as List<Example>;
            Assert.NotNull( ((result as OkObjectResult).Value as List<Example>) );
            // Assert.NotNull(exampleList);

            foreach (var item in ((result as OkObjectResult).Value as List<Example>))
            {
                Assert.Equal(category, item.category);
            }
        }
        
        #endregion
        #region Count examples

        [Fact]
        public async Task CountExample_Category_ReturnBadRequest()
        {
            //Arrange
            string category = "btw";
            _mockRepo.Setup( repo => repo.GetNumberOfExamplesAsync(category) )
                .Returns( Task.FromResult(-1) );
            
            //Act
            IActionResult result = await _controller.GetNumberOfExamplesAsync(category);

            //Assert
            Assert.True(result is BadRequestResult);
            Assert.Equal(400, (result as BadRequestResult).StatusCode);
        }

        [Fact]
        public async Task CountExample_NonExistingCategory_ReturnOk()
        {
            //Arrange
            string category = "EmptyCat";
            _mockRepo.Setup( repo => repo.GetNumberOfExamplesAsync(category) )
                .Returns( CountExamples(category) );
            
            //Act
            IActionResult result = await _controller.GetNumberOfExamplesAsync(category);

            //Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
            Assert.Equal(0, (result as OkObjectResult).Value);
        }

        [Fact]
        public async Task CountExample_Category_ReturnOk()
        {
            //Arrange
            string category = "btw";
            _mockRepo.Setup( repo => repo.GetNumberOfExamplesAsync(category) )
                .Returns( CountExamples(category) );
            
            //Act
            IActionResult result = await _controller.GetNumberOfExamplesAsync(category);

            //Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
            Assert.True((int)(result as OkObjectResult).Value > 0);
        }

        #endregion
        #region Get single example by id

        [Fact]
        public async Task GetExampleById_ReturnNotFound()
        {
            // Arrange
            _mockRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( Task.FromResult( new Example("btw", "let op! prijs ex btw en ex bpm!", false, "1") ) );
            string nonExistentExampleId = "This Is A Non Valid Id";

            // Act
            IActionResult result = await _controller.GetByIdAsync(nonExistentExampleId);

            // Assert
            Assert.True(result is NotFoundResult);
            Assert.Equal(404, (result as NotFoundResult).StatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetExampleById_ReturnBadRequest_EmptyId(string id)
        {
            // Arrange
            _mockRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( Task.FromResult( new Example("btw", "let op! prijs ex btw en ex bpm!", false, "1") ) );

            // Act
            IActionResult result = await _controller.GetByIdAsync(id);

            // Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task GetExampleById_ReturnOkObjectWithExample()
        {
            //Arrange
            string id = "1";
            _mockRepo.Setup( repo => repo.GetByIdAsync(id) )
                .Returns( Task.FromResult( new Example("btw", "let op! prijs ex btw en ex bpm!", false, "1") ) );
            
            //Act
            IActionResult result = await _controller.GetByIdAsync(id);
            
            //Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
            Assert.Equal(id, ((result as OkObjectResult).Value as Example).Id);
        }

        #endregion
        #region Create a new example

        [Theory]
        [MemberData(nameof(WrongExamples))]
        public async Task CreateExample_ReturnBadRequest_EmptyParameter( Example example )
        {
            //Arrange
            _addCommand.Setup( command => command.Execute(example) );

            //Act
            IActionResult result = await _controller.CreatExampleAsync(example);
            
            //Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task CreateExample_ReturnOk()
        {
            //Arrange
            Example example = new Example("btw", "let op! prijs ex btw en ex bpm!", false, "1");
            _addCommand.Setup( repo => repo.Execute(example) );
            
            //Act
            IActionResult result = await _controller.CreatExampleAsync(example);
            
            //Assert
            Assert.True(result is CreatedResult);
            Assert.Equal(201, (result as CreatedResult).StatusCode);
        }
    
        #endregion
        #region Update an existing example

        [Theory]
        [MemberData(nameof(WrongExamples))]
        public async Task UpdateExample_ReturnBadRequest_EmptyParameter( Example example )
        {
            //Arrange
            _updateCommand.Setup( command => command.Execute(example) );

            //Act
            IActionResult result = await _controller.UpdateExampleAsync(example);
            
            //Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task UpdateExample_ReturnOk()
        {
            //Arrange
            Example example = new Example("btw", "let op! prijs ex btw en ex bpm!", false, "1");
            _addCommand.Setup( repo => repo.Execute(example) );
            
            //Act
            IActionResult result = await _controller.UpdateExampleAsync(example);
            
            //Assert
            Assert.True(result is OkResult);
            Assert.Equal(200, (result as OkResult).StatusCode);
        }

        #endregion
        #region Delete an existing example
    
        [Theory]
        [MemberData(nameof(WrongExamples))]
        public async Task DeleteExample_ReturnBadRequest_EmptyParameter( Example example)
        {
            //Arrange
            _deleteCommand.Setup( command => command.Execute(example) );

            //Act
            IActionResult result = await _controller.DeleteExampleAsync(example);
            
            //Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task DeleteExample_ReturnOk()
        {
            //Arrange
            Example example = new Example("btw", "let op! prijs ex btw en ex bpm!", false, "1");
            _addCommand.Setup( repo => repo.Execute(example) );
            
            //Act
            IActionResult result = await _controller.DeleteExampleAsync(example);
            
            //Assert
            Assert.True(result is OkResult);
            Assert.Equal(200, (result as OkResult).StatusCode);
        }
    
        #endregion
    }
}