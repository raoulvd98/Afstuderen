using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AdvertisementPrediction.Model
{
    public class Advertisement {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "PartitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty(PropertyName = "Advertisement")]
        public AdvertisementContent AdvertisementContent { get; set; }

        public const string DefaultDocumentType = "AdvertisementVersion";

        [JsonProperty(PropertyName = "DocumentType")]
        public string DocumentType { get; set; }

        [JsonProperty(PropertyName = "CreateDateTime")]
        public DateTime DateTime { get; set; }
    }

    public class AdvertisementContent {
        [JsonProperty(PropertyName = "Auto")]
        public Car Car { get; set; }

        [JsonProperty(PropertyName = "CommercieelEigenaar")]
        public Owner Owner { get; set; }

        [JsonProperty(PropertyName = "Documentatie")]
        public Documentation Documentation  { get; set; }
    }

    public class Documentation {
        [JsonProperty(PropertyName = "Afbeeldingen")]
        public List<Image> Images { get; set; }
    }

    public class Image {
        [JsonProperty(PropertyName = "Uri")]
        public string Uri { get; set; }
    }

    public class Owner {
        [JsonProperty(PropertyName = "Naam")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "EmailAlgemeen")]
        public string Email { get; set; }
    }

    public class Car{
        [JsonProperty(PropertyName = "Algemeen")]
        public GeneralInformationCar General { get; set; }

        [JsonProperty(PropertyName = "Prijs")]
        public Price Price { get; set; }

        [JsonProperty(PropertyName = "Garantie")]
        public Guarantee Guarantee { get; set; }

        [JsonProperty(PropertyName = "Afleverpakketten")]
        public List<DeliveryPackage> DeliveryPackages { get; set;}
    }

    public class DeliveryPackage {
        [JsonProperty(PropertyName = "Naam")]
        public string Name { get; set;}

        [JsonProperty(PropertyName = "Prijs")]
        public int Price { get; set;}

        [JsonProperty(PropertyName = "Omschrijving")]
        public string Description { get; set;}

        [JsonProperty(PropertyName = "IsInbegrepen")]
        public Nullable<bool> IsIncluded { get; set;}
    }

    public class Guarantee {
        [JsonProperty(PropertyName = "HeeftFabrieksgarantie")]
        public string GuaranteeFactory { get; set; }

        [JsonProperty(PropertyName = "HeeftMerkgarantie")]
        public string GuaranteeBrand { get; set; }

        [JsonProperty(PropertyName = "Bovaggarantie")]
        public string GuaranteeBovag { get; set; }
    }

    public class Price {
        [JsonProperty(PropertyName = "AllInPrijs")]
        public string AllInPrice { get; set; }

        [JsonProperty(PropertyName = "ExclusiefBtw")]
        public Nullable<bool> ExclusiveBTW { get; set; }
    }

    public class GeneralInformationCar {
        [JsonProperty(PropertyName = "Merk")]
        public string Brand { get; set; }

        [JsonProperty(PropertyName = "Model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "NieuwOccasion")]
        public string NewOccasion { get; set; }

        [JsonProperty(PropertyName = "Titel")]
        public string Title { get; set; }
        
        [JsonProperty(PropertyName = "OpmerkingenConsument")]
        public string ConsumentComments { get; set; }

        [JsonProperty(PropertyName = "OpmerkingenHandel")]
        public string TradeComments { get; set; }

        [JsonProperty(PropertyName = "HeeftHonderdProcentOnderhoudenLabel")]
        public Nullable<bool> BovagMaintainedLabel { get; set; }

        [JsonProperty(PropertyName = "HeeftBovagChecklist")]
        public Nullable<bool> BovagCheckList { get; set; }

        [JsonProperty(PropertyName = "HeeftBovagAfleverbeurt")]
        public Nullable<bool> BovagDelivery { get; set; }
    }
}