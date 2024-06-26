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
    public static class DapperTableDefinitionFactory
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

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine("using Best.Practices.Core.Cqrs.Dapper.TableDefinitions;");
            content.AppendLine("using System.Data;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Domain.Models;");
            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

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
                content.AppendLine("\t\t\t\t{");
                content.AppendLine($"\t\t\t\t\tDbFieldName = \"{item.Name}\",");
                content.AppendLine($"\t\t\t\t\tEntityFieldName = nameof({originalClassName}.{item.Name}),");
                content.AppendLine($"\t\t\t\t\tType = DbType.{GetDbTypeName(item.Type)}");
                content.AppendLine("\t\t\t\t},");
            }
            content.AppendLine("\t\t\t}");
            content.AppendLine("\t\t};");
        }

        private static object GetDbTypeName(string type)
        {
            return type switch
            {
                "string" => "AnsiString",
                "int" => "Int32",
                "int?" => "Int32",
                "DateTime" => "DateTime",
                "DateTime?" => "DateTime",
                "decimal" => "Decimal",
                "decimal?" => "Decimal",
                "bool" => "Boolean",
                "bool?" => "Boolean",
                "Guid" => "Guid",
                "Guid?" => "Guid",
                _ => "AnsiString",
            };
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