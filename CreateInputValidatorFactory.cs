﻿using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class CreateInputValidatorFactory
    {
        public static string Create(string fileContent, IList<PropertyInfo> classProperties, string filePath)
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

            content.Append(GetUsings(fileContent));

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine("using Best.Practices.Core.Extensions;");
            content.AppendLine("using FluentValidation;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Common;");
            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

            content.AppendLine("{");

            var newClassName = string.Concat("Create", originalClassName, "InputValidator");

            content.AppendLine(string.Concat("\tpublic class ", newClassName, $" : AbstractValidator<Create{originalClassName}Input>"));

            content.AppendLine("\t{");

            GenerateRepositoryConstructor(content, originalClassName, newClassName, properties);

            content.AppendLine("\t}");

            content.AppendLine("}");

            return content.ToString();
        }

        private static void Validate(string fileContent)
        {
            if (fileContent.IndexOf("namespace ") < 0)
                throw new ValidationException("The file selected is not valid.");
        }

        private static void GenerateRepositoryConstructor(StringBuilder content, string originalClassName, string newClassName, IList<PropertyInfo> properties)
        {
            content.AppendLine();
            content.AppendLine($"\t\tpublic {newClassName}()");
            content.AppendLine("\t\t{");

            foreach (var item in properties)
            {
                content.AppendLine($"\t\t\tRuleFor(v => v.{item.Name})");
                content.AppendLine($"\t\t\t\t.NotEmpty()");
                content.AppendLine($"\t\t\t\t.WithMessage(v => Constants.ErrorMessages.{originalClassName}{item.Name}IsInvalid.Format(v.{item.Name}));");
                content.AppendLine("");
            }

            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static string GetNameSpace(string filePath)
        {
            var solution = VS.Solutions.GetCurrentSolutionAsync().Result;

            var solutionPath = Path.GetDirectoryName(solution.FullPath);

            var namespacePath = filePath.Replace(solutionPath, "").Replace("\\", ".");

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