using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using BestPracticesCodeGenerator.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class CreateInputBuilderFactory
    {
        public static string Create(string fileContent)
        {
            Validate(fileContent);

            var properties = GetPropertiesInfo(fileContent);

            if (!properties.Any())
                throw new ValidationException("It wasn't identified public properties to generate builder class");

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateBuilderClass(fileContent, originalClassName, properties);
        }

        private static string CreateBuilderClass(string fileContent, string originalClassName, IList<PropertyInfo> properties)
        {
            var content = new StringBuilder();

            content.Append(GetUsings(fileContent));

            fileContent = fileContent.Substring(content.Length);

            content.Append(GetNameSpace(fileContent));

            content.AppendLine("{");

            var newClassName = string.Concat("Create", originalClassName, "InputBuilder");

            content.AppendLine(string.Concat("\tpublic class ", newClassName));

            content.AppendLine("\t{");

            GeneratePrivateVariables(content, properties);

            GenerateBuilderConstructor(content, newClassName);

            GenerateMethodsToSetValues(content, newClassName, properties);

            GenerateMethodBuild(content, originalClassName, properties);

            content.AppendLine("\t}");

            content.AppendLine("}");

            return content.ToString();
        }

        private static void Validate(string fileContent)
        {
            if (fileContent.IndexOf("namespace ") < 0)
                throw new ValidationException("The file selected is not valid.");
        }

        private static void GenerateMethodBuild(StringBuilder content, string originalClassName, IList<PropertyInfo> properties)
        {
            content.AppendLine($"\t\tpublic Create{originalClassName}Input Build()");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\treturn new Create{originalClassName}Input");
            content.AppendLine("\t\t\t{");

            for (int i = 0; i < properties.Count; i++)
            {
                var setValue = $"\t\t\t\t{properties[i].Name} = _{properties[i].Name.GetWordWithFirstLetterDown()}";
                if (i + 1 != properties.Count)
                    setValue = string.Concat(setValue, ",");

                content.AppendLine(setValue);
            }

            content.AppendLine("\t\t\t};");
            content.AppendLine("\t\t}");
        }

        private static void GenerateBuilderConstructor(StringBuilder content, string newClassName)
        {
            content.AppendLine();
            content.AppendLine($"\t\tpublic {newClassName}()" + " { }");
            content.AppendLine();
        }

        private static void GenerateMethodsToSetValues(StringBuilder content, string className, IList<PropertyInfo> properties)
        {
            foreach (var item in properties)
            {
                content.AppendLine($"\t\tpublic {className} With{item.Name}({item.Type} {item.Name.GetWordWithFirstLetterDown()})");
                content.AppendLine("\t\t{");
                content.AppendLine($"\t\t\t_{item.Name.GetWordWithFirstLetterDown()} = {item.Name.GetWordWithFirstLetterDown()};");
                content.AppendLine("\t\t\treturn this;");
                content.AppendLine("\t\t}");
                content.AppendLine();
            }
        }

        private static void GeneratePrivateVariables(StringBuilder content, IList<PropertyInfo> properties)
        {
            foreach (var item in properties)
            {
                content.AppendLine($"\t\tprivate {item.Type} _{item.Name.GetWordWithFirstLetterDown()};");
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