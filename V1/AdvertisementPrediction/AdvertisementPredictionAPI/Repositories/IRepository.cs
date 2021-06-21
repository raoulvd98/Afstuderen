using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using AdvertisementPrediction.Model;

namespace AdvertisementPrediction.Repositories
{
    public interface IAdvertisementRepository
    {
        Task<List<Advertisement>> FindAllAsync(string brand, string company, int page, int pageSize = defaultSetting.defaultPageSize);
        Task<int> GetNumberOfAdvertisementsAsync(string brand, string company);
        Task<Advertisement> GetByIdAsync (string id);
    }

    public interface IExampleRepository
    {
        Task<List<Example>> FindAllAsync(string category);
        Task<List<Example>> FindAllPaginationAsync(string category, int page, int pageSize = defaultSetting.defaultPageSize);
        Task<int> GetNumberOfExamplesAsync(string category);
        Task<Example> GetByIdAsync (string id);
    }

    public interface IPredictionRepository
    {
        Task<List<Prediction>> FindAllAsync(string model, bool? positive, bool orderByModel, int page, int pageSize = defaultSetting.defaultPageSize);
        int GetNumberOfPredictions(string model, bool? positive);
        Task<List<Prediction>> GetByIdAsync (string id);
    }
}