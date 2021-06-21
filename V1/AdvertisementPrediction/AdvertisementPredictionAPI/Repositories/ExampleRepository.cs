using System;
using System.Net;
using System.IO;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AdvertisementPrediction.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvertisementPrediction.Repositories
{
    public class ExampleRepository : IExampleRepository 
    {
        private string _context;
        public ExampleRepository()
        {
            this._context = @"JsonData/Examples.json";
        }

        public async Task<List<Example>> FindAllAsync(string category)
        {
            var ExamplesJson =  await Task.Run(
                () => File.ReadAllText(this._context)
            );

            List<Example> Examples = JsonConvert.DeserializeObject<List<Example>>(ExamplesJson)
                .Where(x => category == null || x.category == category)
                .ToList();
            return Examples;
        }
        
        public async Task<List<Example>> FindAllPaginationAsync(string category, int page, int pageSize)
        {
            var ExamplesJson =  await Task.Run(
                () => File.ReadAllText(this._context)
            );

            List<Example> Examples = JsonConvert.DeserializeObject<List<Example>>(ExamplesJson)
                .Where(x => category == null || x.category == category)
                .Skip( (page - 1) * pageSize )
                .Take(pageSize)
                .ToList();
            return Examples;
        }

        public async Task<int> GetNumberOfExamplesAsync(string category)
        {
            var ExamplesJson =  await Task.Run(
                () => File.ReadAllText(this._context)
            );

            int NumberOfExamples = JsonConvert.DeserializeObject<List<Example>>(ExamplesJson)
                .Where(x => category == null || x.category == category)
                .Count();
            return NumberOfExamples;
        }

        public async Task<Example> GetByIdAsync (string id) 
        {
            var ExamplesJson =  await Task.Run(
                () => File.ReadAllText(this._context)
            );

            Example example = JsonConvert.DeserializeObject<List<Example>>(ExamplesJson)
                .Where(x => x.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
            return example;
        }
    }
}