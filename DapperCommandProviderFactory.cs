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
    public static class DapperCommandProviderFactory
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

            content.AppendLine("using Best.Practices.Core.Cqrs.Dapper.CommandProviders;");
            content.AppendLine("using Best.Practices.Core.Cqrs.Dapper.Extensions;");
            content.AppendLine("using Best.Practices.Core.Common;");
            content.AppendLine("using Best.Practices.Core.Domain.Cqrs;");
            content.AppendLine("using Dapper;");
            content.AppendLine("using System.Data;");
            content.AppendLine("using MySql.Data.MySqlClient;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Domain.Models;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Domain.Cqrs.CommandProviders;");
            content.AppendLine($"using {GetNameRootProjectName()}.Cqrs.Dapper.EntityCommands;");

            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

            content.AppendLine("{");

            var newClassName = string.Concat(originalClassName, "CqrsCommandProvider");

            content.AppendLine(string.Concat("\tpublic class ", newClassName, $" : DapperCqrsCommandProvider<{originalClassName}>, I{originalClassName}CqrsCommandProvider"));

            content.AppendLine("\t{");

            GeneratePrivateVariables(content, originalClassName, properties);

            GenerateRepositoryConstructor(content, originalClassName, newClassName);

            GenerateDapperCqrsCommandProviderMethodsImplementation(content, originalClassName);

            GenerateMethodsToGetEntity(content, originalClassName, newClassName, properties);

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
            content.AppendLine($"\t\tpublic {newClassName}(IDbConnection connection) : base(connection)");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GenerateMethodsToGetEntity(StringBuilder content, string originalClassName, string className, IList<PropertyInfo> properties)
        {
            var propertiesToPreventDuplication = properties.Where(p => p.PreventDuplication).ToList();

            foreach (var property in propertiesToPreventDuplication)
            {
                content.AppendLine($"\t\tpublic async Task<{originalClassName}> Get{originalClassName}By{property.Name}({property.Type} {property.Name.GetWordWithFirstLetterDown()})");
                content.AppendLine("\t\t{");
                content.AppendLine($"\t\t\tvar sql = SqlSelectCommand");
                content.AppendLine($"\t\t\t\t + CommonConstants.StringEnter");
                content.AppendLine($"\t\t\t\t + \"Where\" + CommonConstants.StringEnter");
                content.AppendLine($"\t\t\t\t + \"t.{property.Name} = @{property.Name};\";");
                content.AppendLine();
                content.AppendLine($"\t\t\tvar parameters = new DynamicParameters();");
                content.AppendLine();
                content.AppendLine($"\t\t\tparameters.Add(\"@{property.Name}\", {property.Name.GetWordWithFirstLetterDown()});");
                content.AppendLine();
                content.AppendLine($"\t\t\treturn await _connection.QueryFirstOrDefaultAsync<{originalClassName}>(sql, parameters);");
                content.AppendLine("\t\t}");
                content.AppendLine();
                content.AppendLine($"\t\tpublic async Task<{originalClassName}> GetAnother{originalClassName}By{property.Name}({originalClassName} {originalClassName.GetWordWithFirstLetterDown()}, {property.Type} {property.Name.GetWordWithFirstLetterDown()})");
                content.AppendLine("\t\t{");
                content.AppendLine($"\t\t\tvar sql = SqlSelectCommand");
                content.AppendLine($"\t\t\t\t + CommonConstants.StringEnter");
                content.AppendLine($"\t\t\t\t + \"Where\" + CommonConstants.StringEnter");
                content.AppendLine($"\t\t\t\t + \"t.{property.Name} = @{property.Name} and t.Id <> @Id\";");
                content.AppendLine();
                content.AppendLine($"\t\t\tvar parameters = new DynamicParameters();");
                content.AppendLine();
                content.AppendLine($"\t\t\tparameters.Add(\"@{property.Name}\", {property.Name.GetWordWithFirstLetterDown()});");
                content.AppendLine($"\t\t\tparameters.Add(\"@Id\", {originalClassName.GetWordWithFirstLetterDown()}.Id);");
                content.AppendLine();
                content.AppendLine($"\t\t\treturn await _connection.QueryFirstOrDefaultAsync<{originalClassName}>(sql, parameters);");
                content.AppendLine("\t\t}");
                content.AppendLine();
            }

            var propertiesToCreateGetMethod = properties.Where(p => p.GenerateGetMethodOnRepository)
                .Except(propertiesToPreventDuplication)
                .ToList();

            foreach (var property in propertiesToCreateGetMethod)
            {
                content.AppendLine($"\t\tpublic async Task<{originalClassName}> Get{originalClassName}By{property.Name}({property.Type} {property.Name.GetWordWithFirstLetterDown()})");
                content.AppendLine("\t\t{");
                content.AppendLine($"\t\t\tvar sql = SqlSelectCommand");
                content.AppendLine($"\t\t\t\t + CommonConstants.StringEnter");
                content.AppendLine($"\t\t\t\t + \"Where\" + CommonConstants.StringEnter");
                content.AppendLine($"\t\t\t\t + \"t.{property.Name} = @{property.Name};\";");
                content.AppendLine();
                content.AppendLine($"\t\t\tvar parameters = new DynamicParameters();");
                content.AppendLine();
                content.AppendLine($"\t\t\tparameters.Add(\"@{property.Name}\", {property.Name.GetWordWithFirstLetterDown()});");
                content.AppendLine();
                content.AppendLine($"\t\t\treturn await _connection.QueryFirstOrDefaultAsync<{originalClassName}>(sql, parameters);");
                content.AppendLine("\t\t}");
                content.AppendLine();
            }
        }

        private static void GenerateDapperCqrsCommandProviderMethodsImplementation(StringBuilder content, string originalClassName)
        {
            content.AppendLine($"\t\tpublic override IEntityCommand GetAddCommand({originalClassName} entity)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\t return new {originalClassName}Command(_connection, entity);");
            content.AppendLine("\t\t}");
            content.AppendLine();

            content.AppendLine($"\t\tpublic override IEntityCommand GetDeleteCommand({originalClassName} entity)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\t return new {originalClassName}Command(_connection, entity);");
            content.AppendLine("\t\t}");
            content.AppendLine();

            content.AppendLine($"\t\tpublic override IEntityCommand GetUpdateCommand({originalClassName} entity)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\t return new {originalClassName}Command(_connection, entity);");
            content.AppendLine("\t\t}");
            content.AppendLine();

            content.AppendLine($"\t\tpublic override async Task<{originalClassName}> GetById(Guid id)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\tvar sql = SqlSelectCommand");
            content.AppendLine($"\t\t\t\t + CommonConstants.StringEnter");
            content.AppendLine($"\t\t\t\t + \"Where\" + CommonConstants.StringEnter");
            content.AppendLine($"\t\t\t\t + \"t.Id = @Id;\";");
            content.AppendLine();
            content.AppendLine($"\t\t\tvar parameters = new DynamicParameters();");
            content.AppendLine();
            content.AppendLine($"\t\t\tparameters.Add(\"@Id\", id);");
            content.AppendLine();
            content.AppendLine($"\t\t\treturn await _connection.QueryFirstOrDefaultAsync<{originalClassName}>(sql, parameters);");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GeneratePrivateVariables(StringBuilder content, string originalClassName, IList<PropertyInfo> properties)
        {
            content.AppendLine($"\t\tprivate readonly string SqlSelectCommand = @\"SELECT");

            int propertyIndex = 0;

            foreach (var property in properties)
            {
                var separator = (propertyIndex != properties.Count - 1) && (properties.Count > 1) ? "," : string.Empty;

                content.AppendLine($"\t\t\t\t\t\t\t\t\t\t\t t.{property.Name}" + separator);

                propertyIndex++;
            }

            content.AppendLine($"\t\t\t\t\t\t\t\t\t\t\t FROM {originalClassName} t\";");
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