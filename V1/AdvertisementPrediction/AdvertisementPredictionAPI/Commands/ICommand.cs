using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using AdvertisementPrediction.Model;

namespace AdvertisementPrediction.Commands
{
    public interface IAbstractExampleCommand
    {
        Task Execute(Example example);
    }
    
    public interface IAbstractPredictionCommand
    {
        Task Execute(Prediction prediction);
    }
}