using System.Collections.Generic;

namespace BestPracticesCodeGenerator.Dtos.Postman
{
    public record PostmanCollectionItemEventScript
    {
        public IList<string> Exec { get; set; }
        public string Type { get; set; }
        public PostmanCollectionItemEventScriptPackage Packages { get; set; }
    }
}