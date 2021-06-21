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
    public class AdvertisementControllerTest
    {
        private Mock<IAdvertisementRepository> _mockRepo;
        private readonly AdvertisementController _controller;

        public AdvertisementControllerTest()
        {
            _mockRepo = new Mock<IAdvertisementRepository>();
            _controller = new AdvertisementController(this._mockRepo.Object);
        }

        public static Task<List<Advertisement>> GetTestAdvertisements(string brand, string company)
        {
            var advertisements = new List<Advertisement>();
            advertisements.Add(new Advertisement{Id = "1", AdvertisementContent = new AdvertisementContent {
               Car = new Car {
                   General = new GeneralInformationCar {
                       Brand = "Volvo"
                   }
               },
               Owner = new Owner {
                   Name = "Stern Auto"
               }
            }});
            advertisements.Add(new Advertisement{Id = "2", AdvertisementContent = new AdvertisementContent {
               Car = new Car {
                   General = new GeneralInformationCar {
                       Brand = "Volvo"
                   }
               },
               Owner = new Owner {
                   Name = "Stern Auto"
               }
            }});
            advertisements.Add(new Advertisement{Id = "3", AdvertisementContent = new AdvertisementContent {
               Car = new Car {
                   General = new GeneralInformationCar {
                       Brand = "Kia"
                   }
               },
               Owner = new Owner {
                   Name = "Stern Auto"
               }
            }});
            return Task.FromResult(advertisements
                .Where(x => brand == null || x.AdvertisementContent.Car.General.Brand == brand)
                .Where(x => company == null || x.AdvertisementContent.Owner.Name.Contains(company))
                .ToList());
        }

        #region Get all advertisements

        [Fact]
        public async Task GetAdvertisements_FindByBrandAndCompany_GetEmptyList()
        {
            //Arrange
            string brand = "EmptyBrand";
            string company = "EmptyCompany";
            _mockRepo.Setup( repo => repo.FindAllAsync(brand, company, 1, 10) )
                .Returns( GetTestAdvertisements(brand, company) );
            
            //Act
            IActionResult result = await _controller.FindAllAsync(brand, company);

            //Assert
            Assert.True(result is NoContentResult);
            Assert.Equal(204, (result as NoContentResult).StatusCode);
        }

        [Fact]
        public async Task GetAdvertisements_FindByBrandAndCompany()
        {
            // Arrange
            string brand = "Volvo";
            string company = "Stern Auto";
            _mockRepo.Setup( repo => repo.FindAllAsync(brand, company, 1, 10) )
                .Returns( GetTestAdvertisements(brand, company) );

            // Act
            IActionResult result = await _controller.FindAllAsync(brand, company);

            // Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
            Assert.NotNull( ((result as OkObjectResult).Value as List<Advertisement>) );

            foreach (var item in ((result as OkObjectResult).Value as List<Advertisement>))
            {
                Assert.Equal(brand, item.AdvertisementContent.Car.General.Brand);
                Assert.Contains(company, item.AdvertisementContent.Owner.Name);
            }
        }

        #endregion
        #region Get number of advertisements

        [Fact]
        public async Task GetNumberOfAdvertisements_FindByBrandAndCompany_ReturnNoContent()
        {
            // Arrange
            string brand = "Volvo";
            string company = "Stern Auto";
            _mockRepo.Setup( repo => repo.GetNumberOfAdvertisementsAsync(brand, company) )
                .Returns( Task.FromResult(-1) );

            // Act
            IActionResult result = await _controller.GetNumberOfAdvertisementsAsync(brand, company);

            // Assert
            Assert.True(result is BadRequestResult);
            Assert.Equal(400, (result as BadRequestResult).StatusCode);
        }

        [Fact]
        public async Task GetNumberOfAdvertisements_FindByBrandAndCompany()
        {
            // Arrange
            string brand = "Volvo";
            string company = "Stern Auto";
            _mockRepo.Setup( repo => repo.GetNumberOfAdvertisementsAsync(brand, company) )
                .Returns( Task.FromResult(8) );

            // Act
            IActionResult result = await _controller.GetNumberOfAdvertisementsAsync(brand, company);

            // Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
        }

        #endregion
        #region Get single advertisement by id

        [Fact]
        public async Task GetAdvertisementById_ReturnNotFound()
        {
            // Arrange
            _mockRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( Task.FromResult( new Advertisement{Id = "1", AdvertisementContent = null } ) );
            string nonExistentAdvertisementId = "This Is A Non Valid Id";

            // Act
            IActionResult result = await _controller.GetByIdAsync(nonExistentAdvertisementId);

            // Assert
            Assert.True(result is NotFoundResult);
            Assert.Equal(404, (result as NotFoundResult).StatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetAdvertisementById_ReturnBadRequest_EmptyId(string id)
        {
            // Arrange
            _mockRepo.Setup( repo => repo.GetByIdAsync("1") )
                .Returns( Task.FromResult( new Advertisement{Id = "1", AdvertisementContent = null } ) );

            // Act
            IActionResult result = await _controller.GetByIdAsync(id);

            // Assert
            Assert.True(result is BadRequestObjectResult);
            Assert.Equal(400, (result as BadRequestObjectResult).StatusCode);
        }

        [Fact]
        public async Task GetAdvertisementById_ReturnOkObjectWithExample()
        {
            //Arrange
            string id = "1";
            _mockRepo.Setup( repo => repo.GetByIdAsync(id) )
                .Returns( Task.FromResult( new Advertisement{Id = "1", AdvertisementContent = null } ) );
            
            //Act
            IActionResult result = await _controller.GetByIdAsync(id);
            
            //Assert
            Assert.True(result is OkObjectResult);
            Assert.Equal(200, (result as OkObjectResult).StatusCode);
            Assert.Equal(id, ((result as OkObjectResult).Value as Advertisement).Id);
        }

        #endregion
    }
}