using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using BestPracticesCodeGenerator.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class RepositoryFactory
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

            var newClassName = string.Concat(originalClassName, "Repository");

            content.AppendLine(string.Concat("\tpublic class ", newClassName, $" : Repository<{originalClassName}>, I{originalClassName}Repository"));

            content.AppendLine("\t{");

            GeneratePrivateVariables(content, originalClassName);

            GenerateRepositoryConstructor(content, originalClassName, newClassName);

            GenerateMethodsToGetEntity(content, originalClassName, properties);

            content.AppendLine("\t}");

            content.AppendLine("}");

            return content.ToString();
        }

        private static void Validate(string fileContent)
        {
            if (fileContent.IndexOf("namespace ") < 0)
                throw new ValidationException("The file selected is not valid.");
        }

        private static void GenerateRepositoryConstructor(StringBuilder content, string originalClassName, string newClassName)
        {
            content.AppendLine();
            content.AppendLine($"\t\tpublic {newClassName}(I{originalClassName}CqrsCommandProvider commandProvider) : base(commandProvider)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\t_{originalClassName.GetWordWithFirstLetterDown()}CqrsCommandProvider = commandProvider;");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GenerateMethodsToGetEntity(StringBuilder content, string className, IList<PropertyInfo> properties)
        {
            foreach (var item in properties)
            {
                content.AppendLine($"\t\tpublic async Task<{className}> Get{className}By{item.Name}({item.Type} {item.Name.GetWordWithFirstLetterDown()})");
                content.AppendLine("\t\t{");
                content.AppendLine($"\t\t\t return HandleAfterGetFromCommandProvider(await _{className.GetWordWithFirstLetterDown()}CqrsCommandProvider.Get{className}By{item.Name}({item.Name.GetWordWithFirstLetterDown()}));");
                content.AppendLine("\t\t}");
                content.AppendLine();
            }
        }

        private static void GeneratePrivateVariables(StringBuilder content, string originalClassName)
        {
            content.AppendLine($"\t\tprivate readonly I{originalClassName}CqrsCommandProvider _{originalClassName.GetWordWithFirstLetterDown()}CqrsCommandProvider;");
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