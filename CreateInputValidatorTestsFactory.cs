﻿using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class CreateInputValidatorTestsFactory
    {
        public static string Create(string fileContent, IList<PropertyInfo> classProperties, string filePath)
        {
            Validate(fileContent);

            if (!classProperties.Any())
                throw new ValidationException("It wasn't identified public properties to generate builder class");

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateUseCaseTestsClass(fileContent, originalClassName, classProperties, filePath);
        }

        private static string CreateUseCaseTestsClass(string fileContent, string originalClassName, IList<PropertyInfo> properties, string filePath)
        {
            var content = new StringBuilder();

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine("using Best.Practices.Core.Common;");
            content.AppendLine("using FluentAssertions;");
            content.AppendLine("using Xunit;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Tests.Application.Dtos.Builders;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Application.Dtos.Validators;");

            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

            content.AppendLine("{");

            var newClassName = string.Concat("Create", originalClassName, "InputValidatorTests");

            content.AppendLine(string.Concat("\tpublic class ", newClassName));

            content.AppendLine("\t{");

            GeneratePrivateVariables(content, originalClassName);

            GenerateTestConstructor(content, originalClassName, newClassName);

            GenerateInternalExecuteMethod(content, originalClassName, properties);

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
            content.AppendLine($"\t\t\t_validator = new Create{originalClassName}InputValidator();");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GenerateInternalExecuteMethod(StringBuilder content, string className, IList<PropertyInfo> properties)
        {
            var firstProperty = properties.First();

            content.AppendLine("\t\t[Fact]");
            content.AppendLine($"\t\tpublic void Validate_InputIsValid_ReturnsIsValid()");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\tvar input = new Create{className}InputBuilder()");
            content.AppendLine($"\t\t\t\t.With{firstProperty.Name}(\"{firstProperty.Name} value Test\")");
            content.AppendLine($"\t\t\t\t.Build();");
            content.AppendLine("");
            content.AppendLine($"\t\t\tvar validationResult = _validator.Validate(input);");
            content.AppendLine("");
            content.AppendLine("\t\t\tvalidationResult.IsValid.Should().BeTrue();");
            content.AppendLine("\t\t}");
            content.AppendLine();

            foreach (PropertyInfo property in properties)
            {
                content.AppendLine("\t\t[Fact]");
                content.AppendLine($"\t\tpublic void Validate_Input{property.Name}IsInvalid_ReturnsIsInvalid()");
                content.AppendLine("\t\t{");
                content.AppendLine($"\t\t\tvar input = new Create{className}InputBuilder()");
                content.AppendLine($"\t\t\t\t.With{property.Name}(\"Set an invalid value or null\")");
                content.AppendLine($"\t\t\t\t.Build();");
                content.AppendLine("");
                content.AppendLine($"\t\t\tvar validationResult = _validator.Validate(input);");
                content.AppendLine("");
                content.AppendLine("\t\t\tvalidationResult.IsValid.Should().BeFalse();");
                content.AppendLine("\t\t}");
                content.AppendLine();
            }
        }

        private static void GeneratePrivateVariables(StringBuilder content, string originalClassName)
        {
            content.AppendLine($"\t\tprivate readonly Create{originalClassName}InputValidator _validator;");
            content.AppendLine($"");
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