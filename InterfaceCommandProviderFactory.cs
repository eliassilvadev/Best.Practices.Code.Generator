﻿using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using BestPracticesCodeGenerator.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class InterfaceCommandProviderFactory
    {
        public static string Create(
            string fileContent,
            string filePath,
            IList<PropertyInfo> classProperties,
            IList<MethodInfo> methods,
            FileContentGenerationOptions options)
        {
            Validate(fileContent);

            if (!classProperties.Any())
                throw new ValidationException("It wasn't identified public properties to generate builder class");

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateCommandProviderInterface(fileContent, originalClassName, classProperties, filePath);
        }

        private static string CreateCommandProviderInterface(string fileContent, string originalClassName, IList<PropertyInfo> properties, string filePath)
        {
            var content = new StringBuilder();

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine("using Best.Practices.Core.Domain.Cqrs.CommandProviders;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Domain.Models;");
            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

            content.AppendLine("{");

            var newClassName = string.Concat("I", originalClassName, "CqrsCommandProvider");

            content.AppendLine(string.Concat("\tpublic interface ", newClassName, $" : ICqrsCommandProvider<{originalClassName}>"));

            content.AppendLine("\t{");

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

        private static void GenerateMethodsToGetEntity(StringBuilder content, string className, IList<PropertyInfo> properties)
        {
            var propertiesToPreventDuplication = properties.Where(p => p.PreventDuplication).ToList();

            foreach (var property in propertiesToPreventDuplication)
            {
                content.AppendLine($"\t\tTask<{className}> Get{className}By{property.Name}({property.Type} {property.Name.GetWordWithFirstLetterDown()});");
                content.AppendLine("");
                content.AppendLine($"\t\tTask<{className}> GetAnother{className}By{property.Name}({className} {className.GetWordWithFirstLetterDown()}, {property.Type} {property.Name.GetWordWithFirstLetterDown()});");
                content.AppendLine("");
            }

            var propertiesToCreateGetMethod = properties.Where(p => p.GenerateGetMethodOnRepository)
                .Except(propertiesToPreventDuplication)
                .ToList();

            foreach (var property in propertiesToCreateGetMethod)
            {
                content.AppendLine($"\t\tTask<{className}> Get{className}By{property.Name}({property.Type} {property.Name.GetWordWithFirstLetterDown()});");
            }
        }

        private static void GeneratePrivateVariables(StringBuilder content, IList<PropertyInfo> properties)
        {
            foreach (var item in properties)
            {
                content.AppendLine($"\t\tprivate {item.Type} _{item.Name.GetWordWithFirstLetterDown()};");
            }
        }

        private static string GetNameSpace(string filePath)
        {
            var solution = VS.Solutions.GetCurrentSolutionAsync().Result;

            var solutionPath = Path.GetDirectoryName(solution.FullPath);

            var namespacePath = filePath.Replace(solutionPath, "").Replace("\\", ".");
            var solutionName = solution.Name.Replace(".sln", "");

            int count = Regex.Matches(namespacePath, Regex.Escape(solutionName)).Count;

            if (count > 1)
                namespacePath = namespacePath.ReplaceFirstOccurrence("." + solutionName, "");

            namespacePath = namespacePath.Substring(1, namespacePath.Length - 2);

            return "namespace " + namespacePath;
        }

        private static string GetNameRootProjectName()
        {
            var solution = VS.Solutions.GetCurrentSolutionAsync().Result;

            return solution.Name.Replace(".sln", "");
        }

        private static string GetUsings(string fileContent)
        {
            return fileContent.Substring(0, fileContent.IndexOf("namespace"));
        }

        private static string GetOriginalClassName(string fileContent)
        {
            var regex = Regex.Match(fileContent, @"\s+(class)\s+(?<Name>[^\s]+)");

            return regex.Groups["Name"].Value.Replace(":", "");
        }
    }
}