using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class DapperTableDefinitionFactory
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

            content.AppendLine("Best.Practices.Core.CommandProvider.Dapper.CommandProviders;");
            content.AppendLine("System.Data;");
            content.AppendLine("");
            content.Append(GetNameSpace(fileContent));

            content.AppendLine("{");

            var newClassName = string.Concat(originalClassName, "TableDefinition");

            content.AppendLine(string.Concat("\tpublic class ", newClassName));

            content.AppendLine("\t{");

            GenerateDapperDefinitionStaticProperty(content, originalClassName, properties);

            content.AppendLine("\t}");

            content.AppendLine("}");

            return content.ToString();
        }

        private static void Validate(string fileContent)
        {
            if (fileContent.IndexOf("namespace ") < 0)
                throw new ValidationException("The file selected is not valid.");
        }

        private static void GenerateDapperDefinitionStaticProperty(StringBuilder content, string originalClassName, IList<PropertyInfo> properties)
        {
            content.AppendLine();
            content.AppendLine($"\t\tpublic static readonly DapperTableDefinition TableDefinition = new DapperTableDefinition");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\tTableName = \"{originalClassName}\",");
            content.AppendLine($"\t\t\tColumnDefinitions = new List<DapperTableColumnDefinition>()");
            content.AppendLine("\t\t\t{");


            foreach (var item in properties)
            {
                content.AppendLine("\t\t\t\tnew DapperTableColumnDefinition");
                content.AppendLine("\t\t\t\t\t{");
                content.AppendLine($"\t\t\t\t\t\tDbFieldName = \"{item.Name}\",");
                content.AppendLine($"\t\t\t\t\t\tEntityFieldName = nameof({originalClassName}.{item.Name}),");
                content.AppendLine($"\t\t\t\t\t\tType = DbType.{item.Type}");
                content.AppendLine("\t\t\t\t\t},");
            }
            content.AppendLine("\t\t\t}");
            content.AppendLine("\t\t};");
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