using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvertisementPrediction.Model
{
    public class Example {
        public Example(string category, string example, bool isPositive, string id = null)
        {
            if ( string.IsNullOrEmpty(id) ){
                this.Id = Guid.NewGuid().ToString();
            } else {
                this.Id = id;
            }
            this.category = category;
            this.example = example;
            this.isPositive = isPositive;
        }

        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "category is required.")]
        public string category { get; set; }

        [Required(ErrorMessage = "example is required.")]
        public string example { get; set; }

        [Required(ErrorMessage = "isPositive is required.")]
        public bool isPositive { get; set; }
    }
}