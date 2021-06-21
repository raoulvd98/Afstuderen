using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdvertisementPrediction.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvertisementPrediction.Commands
{
    public abstract class AbstractPredictionCommand : IAbstractPredictionCommand
    {
        public AdvertisementPredictionContext _context;
        public AbstractPredictionCommand(AdvertisementPredictionContext context)
        {
            this._context = context;
        }
        public abstract Task Execute(Prediction prediction);
    }

    public class AddPredictionCommand : AbstractPredictionCommand
    {
        public AddPredictionCommand(AdvertisementPredictionContext context) : base(context) { }
        public async override Task Execute(Prediction prediction)
        {
            await this._context.Predictions.AddAsync(prediction);

            await this._context.SaveChangesAsync();
        }
    }

    public class UpdatePredictionCommand : AbstractPredictionCommand
    {
        public UpdatePredictionCommand(AdvertisementPredictionContext context) : base(context) { }
        public async override Task Execute(Prediction prediction)
        {
            this._context.Predictions.Update(prediction);

            await this._context.SaveChangesAsync();
        }
    }

    public class DeletePredictionCommand : AbstractPredictionCommand
    {
        public DeletePredictionCommand(AdvertisementPredictionContext context) : base(context) { }
        public async override Task Execute(Prediction prediction)
        {
            this._context.Predictions.Remove(prediction);

            await this._context.SaveChangesAsync();
        }
    }
}