using System;
using System.Collections.Generic;
using MySql.Data.Types;
using MySql.Data.EntityFrameworkCore;


namespace AdvertisementPrediction.Model
{
    public class Prediction
    {
        public string PredictionId { get; set; }
        public string AdvertisementId { get; set; }
        public string Model { get; set; }
        public double GoodSentence { get; set; }
        public double WrongSentence { get; set; }
        public DateTime Date { get; set; }
    }
}
