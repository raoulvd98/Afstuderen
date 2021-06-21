using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using AdvertisementPrediction.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvertisementPrediction.Commands
{
    public abstract class AbstractExampleCommand : IAbstractExampleCommand
    {
        public string _context { get; set; }
        public AbstractExampleCommand()
        {
            this._context = @"JsonData/Examples.json";
        }
        public abstract Task Execute(Example example);
    }

    public class AddExampleCommand : AbstractExampleCommand
    {
        public AddExampleCommand() : base() { }

        public async override Task Execute(Example example)
        {
            var CurrentExamplesJson = await File.ReadAllTextAsync(this._context);

            List<Example> CurrentExamplesList = JsonConvert.DeserializeObject<List<Example>>(CurrentExamplesJson);

            if ( CurrentExamplesList.FindIndex(ind => ind.Id.Equals(example.Id)) >= 0 )
            {
                throw new Exception("Example cannot be null");
            }

            CurrentExamplesList.Add(example);
            string NewJsonString = JsonConvert.SerializeObject(CurrentExamplesList, Formatting.Indented);
            
            await File.WriteAllTextAsync(this._context, NewJsonString);
        }
    }

    public class UpdateExampleCommand : AbstractExampleCommand
    {
        public async override Task Execute(Example example)
        {
            var CurrentExamplesJson = await File.ReadAllTextAsync(this._context);

            List<Example> CurrentExamplesList = JsonConvert.DeserializeObject<List<Example>>(CurrentExamplesJson);
            
            int index = CurrentExamplesList.FindIndex(ind => ind.Id.Equals(example.Id));
            if ( index < 0 )
            {
                throw new Exception("Example cannot be null");
            }

            CurrentExamplesList[index] = example;
            string NewJsonString = JsonConvert.SerializeObject(CurrentExamplesList, Formatting.Indented);

            await File.WriteAllTextAsync(this._context, NewJsonString);
        }
    }

    public class DeleteExampleCommand : AbstractExampleCommand
    {
        public async override Task Execute(Example example)
        {
            var CurrentExamplesJson = await File.ReadAllTextAsync(this._context);

            List<Example> CurrentExamplesList = JsonConvert.DeserializeObject<List<Example>>(CurrentExamplesJson);

            var ExampleToRemove = CurrentExamplesList.SingleOrDefault(i => i.Id == example.Id);
            if ( ExampleToRemove == null )
            {
                throw new Exception("Example cannot be null");
            }

            CurrentExamplesList.Remove(ExampleToRemove);
            string NewJsonString = JsonConvert.SerializeObject(CurrentExamplesList, Formatting.Indented);

            await File.WriteAllTextAsync(this._context, NewJsonString);
        }
    }
}