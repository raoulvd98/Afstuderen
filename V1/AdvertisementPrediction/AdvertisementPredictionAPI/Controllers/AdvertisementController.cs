using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvertisementPrediction.Model;
using AdvertisementPrediction.Repositories;

namespace AdvertisementPrediction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementRepository _repository;

        public AdvertisementController(IAdvertisementRepository repository)
        {
            this._repository = repository;
        }

        // GET: api/Advertisement
        // GET: api/Advertisement?brand=Volvo
        // GET: api/Advertisement?company=Mossel
        // GET: api/Advertisement?brand=Toyota&company=Louwman
        [HttpGet]
        public async Task<IActionResult> FindAllAsync(string brand, string company, int page = 1)
        {
            List<Advertisement> AdvertisementList = await _repository.FindAllAsync(brand, company, page);

            if (AdvertisementList == null || AdvertisementList.Count == 0)
            {
                return NoContent();
            }

            return Ok(AdvertisementList);
        }

        // GET: api/Advertisement/Count
        [HttpGet("Count")]
        public async Task<IActionResult> GetNumberOfAdvertisementsAsync(string brand, string company)
        {
            int NumberOfAdvertisements = await _repository.GetNumberOfAdvertisementsAsync(brand, company);

            if (NumberOfAdvertisements < 0)
            {
                return BadRequest();
            }

            return Ok(NumberOfAdvertisements);
        }

        // GET: api/Advertisement/5abdf4dd-eb79-447a-b1dc-112c35a93982
        [HttpGet("{id}")] 
        public async Task<IActionResult> GetByIdAsync (string id)
        {
            if( String.IsNullOrEmpty(id) )
            {
                return BadRequest("The argument cannot be null or empty!");
            }

            Advertisement Advertisement = await _repository.GetByIdAsync(id);

            if ( Advertisement == null || Advertisement.GetType() != typeof( Advertisement ))
            {
                return NotFound();
            }
            
            return Ok(Advertisement);
        }
    }
}