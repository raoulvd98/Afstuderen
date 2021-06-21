using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
// using Microsoft.EntityFrameworkCore;
using AdvertisementPrediction.Model;

namespace AdvertisementPrediction.Repositories
{
    public class AdvertisementRepository : IAdvertisementRepository 
    {
        private readonly Container _container;
        private readonly CosmosClient _client;
        public AdvertisementRepository(CosmosClient client, IConfiguration configuration)
        {
            this._client = client;
            this._container = this._client.GetContainer(
                configuration["CosmosDb:DatabaseId"], 
                configuration["CosmosDb:CollectionId"]
            );
        }

        public async Task<List<Advertisement>> FindAllAsync(string brand, string company, int page, int pageSize)
        {   
            FeedIterator<Advertisement> queryable = this._container.GetItemLinqQueryable<Advertisement>()
                .Where(x => brand == null || x.AdvertisementContent.Car.General.Brand == brand)
                .Where(x => company == null || x.AdvertisementContent.Owner.Name.Contains(company))
                .Where(x => x.DocumentType == Advertisement.DefaultDocumentType)
                .Skip( (page - 1) * pageSize )
                .Take(pageSize)
                .ToFeedIterator();

            List<Advertisement> advertisements = new List<Advertisement>();
            while (queryable.HasMoreResults)
            {
                foreach(Advertisement item in await queryable.ReadNextAsync())
                {
                    advertisements.Add(item);
                }
            }

            return advertisements;
        }

        public async Task<int> GetNumberOfAdvertisementsAsync(string brand, string company)
        {
            var queryable = await this._container.GetItemLinqQueryable<Advertisement>()
                .Where(x => brand == null || x.AdvertisementContent.Car.General.Brand == brand)
                .Where(x => company == null || x.AdvertisementContent.Owner.Name.Contains(company))
                .Where(x => x.DocumentType == Advertisement.DefaultDocumentType)
                .CountAsync();

            return queryable;
        }

        public async Task<Advertisement> GetByIdAsync (string id)
        {
            FeedIterator<Advertisement> queryable = this._container.GetItemLinqQueryable<Advertisement>()
                .Where(x => x.Id == id)
                .ToFeedIterator();
            FeedResponse<Advertisement> QueryResult = await queryable.ReadNextAsync();
            Advertisement Advertisement = QueryResult.SingleOrDefault();

            return Advertisement;
        }
    }
}