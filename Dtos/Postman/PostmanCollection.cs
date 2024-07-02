using System.Collections.Generic;

namespace BestPracticesCodeGenerator.Dtos.Postman
{
    public record PostmanCollection
    {
        public PostmanCollectionInfo Info { get; set; }
        public IList<PostmanCollectionItem> Item { get; set; }
    }
}