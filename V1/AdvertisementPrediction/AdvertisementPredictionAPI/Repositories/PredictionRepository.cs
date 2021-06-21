using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AdvertisementPrediction.Model;

namespace AdvertisementPrediction.Repositories
{
    public class PredictionRepository : IPredictionRepository 
    {
        private readonly AdvertisementPredictionContext _context;
        public PredictionRepository(AdvertisementPredictionContext context)
        {
            this._context = context;
        }

        public async Task<List<Prediction>> FindAllAsync(string model, bool? positive, bool orderByModel, int page, int pageSize)
        {
            List<Prediction> predictions = await this._context.Predictions
                .Where( x => model == null || x.Model == model)
                .Where( x => positive == null || (positive == true && x.GoodSentence > 0.85 && x.WrongSentence <= 0.15) ||
                    (positive == false && x.WrongSentence > 0.15 && x.GoodSentence <= 0.85))
                .OrderBy( x => orderByModel ? x.Model : null)
                .ThenByDescending(x => x.Date)
                .Skip( (page - 1) * pageSize )
                .Take(pageSize)
                .ToListAsync();

            return predictions;
        }

        public async Task<List<Prediction>> GetByIdAsync (string id)
        {   
            List<Prediction> predictions = await this._context.Predictions
                .Where(x => x.AdvertisementId == id)
                .OrderByDescending (x => x.WrongSentence)
                .ToListAsync();

            return predictions;
        }

        public int GetNumberOfPredictions(string model,  bool? positive)
        {
            int NumberOfPredictions = this._context.Predictions
                .Where( x => model == null || x.Model == model)
                .Where( x => positive == null || (positive == true && x.GoodSentence > 0.75 && x.WrongSentence <= 0.25) ||
                    (positive == false && x.WrongSentence > 0.25 && x.GoodSentence <= 0.75))
                .Count();

            return NumberOfPredictions;
        }
    }
}