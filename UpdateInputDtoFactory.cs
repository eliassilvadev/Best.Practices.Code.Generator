using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class UpdateInputDtoFactory
    {
        public static string Create(string fileContent)
        {
            Validate(fileContent);

            var properties = GetPropertiesInfo(fileContent);

            if (!properties.Any())
                throw new ValidationException("It wasn't identified public properties to generate builder class");

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateRepositoryClass(fileContent, originalClassName, properties);
        }

        private static string CreateRepositoryClass(string fileContent, string originalClassName, IList<PropertyInfo> properties)
        {
            var content = new StringBuilder();

            content.Append(GetUsings(fileContent));

            fileContent = fileContent.Substring(content.Length);

            content.Append(GetNameSpace(fileContent));

            content.AppendLine("{");

            var newClassName = string.Concat("Update", originalClassName, "Input");

            content.AppendLine(string.Concat("\tpublic record ", newClassName));

            content.AppendLine("\t{");

            GeneratePublicVariables(content, properties);

            content.AppendLine("\t}");

            content.AppendLine("}");

            return content.ToString();
        }

        private static void Validate(string fileContent)
        {
            if (fileContent.IndexOf("namespace ") < 0)
                throw new ValidationException("The file selected is not valid.");
        }

        private static void GeneratePublicVariables(StringBuilder content, IList<PropertyInfo> properties)
        {
            foreach (var item in properties)
            {
                if (item.Name.Equals("Id"))
                    content.AppendLine("\t\t[JsonIgnore]");

                content.AppendLine(string.Concat($"\t\tpublic {item.Type} {item.Name}", " { get; set; }"));
            }
        }

        private static string GetNameSpace(string fileContent)
        {
            return fileContent.Substring(fileContent.IndexOf("namespace"), fileContent.IndexOf("{"));
        }

        private static string GetUsings(string fileContent)
        {
            return fileContent.Substring(0, fileContent.IndexOf("namespace"));
        }

        private static string GetOriginalClassName(string fileContent)
        {
            var regex = Regex.Match(fileContent, @"\s+(class)\s+(?<Name>[^\s]+)");

            return regex.Groups["Name"].Value;
        }

        private static IList<PropertyInfo> GetPropertiesInfo(string fileContent)
        {
            var propertyes = new List<PropertyInfo>();

            foreach (Match item in Regex.Matches(fileContent, @"(?>public)\s+(?!class)((static|readonly)\s)?(?<Type>(\S+(?:<.+?>)?)(?=\s+\w+\s*\{\s*get))\s+(?<Name>[^\s]+)(?=\s*\{\s*get)"))
            {
                propertyes.Add(new PropertyInfo(item.Groups["Type"].Value, item.Groups["Name"].Value));
            }

            return propertyes;
        }
    }
}