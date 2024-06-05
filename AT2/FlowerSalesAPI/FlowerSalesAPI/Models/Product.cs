using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerSalesAPI.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]        
        public string? Id { get; set; }
                
        public string Category { get; set; } = null!;

        [BsonElement("Name")]
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string StoreLocation { get; set; } = string.Empty;

        [Required]
        public int PostCode { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public bool IsAvailable { get; set; }
    }
}
