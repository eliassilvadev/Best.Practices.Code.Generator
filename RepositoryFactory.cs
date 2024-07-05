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
    public static class RepositoryFactory
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

            return CreateRepositoryClass(fileContent, originalClassName, classProperties, filePath);
        }

        private static string CreateRepositoryClass(string fileContent, string originalClassName, IList<PropertyInfo> properties, string filePath)
        {
            var content = new StringBuilder();

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine("using Best.Practices.Core.Domain.Repositories;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Domain.Entities;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Domain.Repositories.Interfaces;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Domain.Cqrs.CommandProviders;");
            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

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
            var propertiesToPreventDuplication = properties.Where(p => p.PreventDuplication).ToList();

            foreach (var property in propertiesToPreventDuplication)
            {
                content.AppendLine($"\t\tpublic async Task<{className}> Get{className}By{property.Name}({property.Type} {property.Name.GetWordWithFirstLetterDown()})");
                content.AppendLine("\t\t{");
                content.AppendLine($"\t\t\t return HandleAfterGetFromCommandProvider(await _{className.GetWordWithFirstLetterDown()}CqrsCommandProvider.Get{className}By{property.Name}({property.Name.GetWordWithFirstLetterDown()}));");
                content.AppendLine("\t\t}");
                content.AppendLine();
                content.AppendLine($"\t\tpublic async Task<{className}> GetAnother{className}By{property.Name}({className} {className.GetWordWithFirstLetterDown()},{property.Type} {property.Name.GetWordWithFirstLetterDown()})");
                content.AppendLine("\t\t{");
                content.AppendLine($"\t\t\t return HandleAfterGetFromCommandProvider(await _{className.GetWordWithFirstLetterDown()}CqrsCommandProvider.GetAnother{className}By{property.Name}({className.GetWordWithFirstLetterDown()}, {property.Name.GetWordWithFirstLetterDown()}));");
                content.AppendLine("\t\t}");
                content.AppendLine();
            }

            var propertiesToCreateGetMethod = properties.Where(p => p.GenerateGetMethodOnRepository)
                .Except(propertiesToPreventDuplication)
                .ToList();

            foreach (var property in propertiesToCreateGetMethod)
            {
                content.AppendLine($"\t\tpublic async Task<{className}> Get{className}By{property.Name}({property.Type} {property.Name.GetWordWithFirstLetterDown()})");
                content.AppendLine("\t\t{");
                content.AppendLine($"\t\t\t return HandleAfterGetFromCommandProvider(await _{className.GetWordWithFirstLetterDown()}CqrsCommandProvider.Get{className}By{property.Name}({property.Name.GetWordWithFirstLetterDown()}));");
                content.AppendLine("\t\t}");
                content.AppendLine();
            }
        }

        private static void GeneratePrivateVariables(StringBuilder content, string originalClassName)
        {
            content.AppendLine($"\t\tprivate readonly I{originalClassName}CqrsCommandProvider _{originalClassName.GetWordWithFirstLetterDown()}CqrsCommandProvider;");
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