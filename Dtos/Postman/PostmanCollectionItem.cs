using System.Collections.Generic;

namespace BestPracticesCodeGenerator.Dtos.Postman
{
    public record PostmanCollectionItem
    {
        public string Name { get; set; }
        public IList<PostmanCollectionItemItem> Item { get; set; }
    }
}
