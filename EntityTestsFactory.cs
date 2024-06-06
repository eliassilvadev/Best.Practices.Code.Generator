using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class EntityTestsFactory
    {
        public static string Create(string fileContent, IList<PropertyInfo> classProperties, string filePath)
        {
            Validate(fileContent);

            var methods = GetMethodsInfo(fileContent);

            if (!classProperties.Any())
                throw new ValidationException("It wasn't identified public properties to generate builder class");

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateUseCaseTestsClass(fileContent, originalClassName, classProperties, methods, filePath);
        }

        private static string CreateUseCaseTestsClass(string fileContent, string originalClassName, IList<PropertyInfo> properties, IList<MethodInfo> methods, string filePath)
        {
            var content = new StringBuilder();

            content.Append(GetUsings(fileContent));

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine("using Best.Practices.Core.Common;");
            content.AppendLine("using Best.Practices.Core.Extensions;");
            content.AppendLine("using FluentAssertions;");
            content.AppendLine("using Moq;");
            content.AppendLine("using Xunit;");

            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

            content.AppendLine("{");

            var newClassName = string.Concat(originalClassName, "Tests");

            content.AppendLine(string.Concat("\tpublic class ", newClassName));

            content.AppendLine("\t{");

            GeneratePrivateVariables(content, originalClassName);

            GenerateTestConstructor(content, originalClassName, newClassName);

            GenerateTestMethods(content, originalClassName, properties, methods);

            content.AppendLine("\t}");

            content.AppendLine("}");

            return content.ToString();
        }

        private static void Validate(string fileContent)
        {
            if (fileContent.IndexOf("namespace ") < 0)
                throw new ValidationException("The file selected is not valid.");
        }

        private static void GenerateTestConstructor(StringBuilder content, string originalClassName, string newClassName)
        {
            content.AppendLine();
            content.AppendLine($"\t\tpublic {newClassName}()");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\t_entity = new {originalClassName}();");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GenerateTestMethods(StringBuilder content, string className, IList<PropertyInfo> properties, IList<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                content.AppendLine("\t\t[Fact]");
                content.AppendLine($"\t\tpublic void {method.Name}_EverythingIsOk_ReturnsSuccess()");
                content.AppendLine("\t\t{");
                content.AppendLine("");
                content.AppendLine($"\t\t\t_entity.{method.Name}();");
                content.AppendLine("\t\t}");
                content.AppendLine();
            }
        }

        private static void GeneratePrivateVariables(StringBuilder content, string originalClassName)
        {
            content.AppendLine($"\t\tprivate readonly {originalClassName} _entity;");
        }

        private static string GetNameSpace(string filePath)
        {
            var solution = VS.Solutions.GetCurrentSolutionAsync().Result;

            var solutionPath = Path.GetDirectoryName(solution.FullPath);

            var namespacePath = filePath.Replace(solutionPath, "").Replace("\\", ".");

            namespacePath = namespacePath.Substring(1, namespacePath.Length - 2);

            return "namespace " + namespacePath;
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

        private static IList<MethodInfo> GetMethodsInfo(string fileContent)
        {
            var propertyes = new List<MethodInfo>();

            foreach (Match item in Regex.Matches(fileContent, @"\b(\w+)\s+(\w+)\s*\(\s*\)"))
            {
                propertyes.Add(new MethodInfo(item.Groups[1].Value, item.Groups[2].Value));
            }

            return propertyes;
        }
    }
}