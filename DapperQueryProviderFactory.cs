using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using BestPracticesCodeGenerator.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class DapperQueryProviderFactory
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

            content.AppendLine("using Best.Practices.Core.Cqrs.Dapper.QueryProviders;");
            content.AppendLine("using Best.Practices.Core.Cqrs.Dapper.Extensions;");
            content.AppendLine("using Best.Practices.Core.Common;");
            content.AppendLine("using Best.Practices.Core.Application.Cqrs;");
            content.AppendLine($"using Best.Practices.Core.Application.Dtos.Input;");
            content.AppendLine("using Dapper;");
            content.AppendLine("using System.Data;");
            content.AppendLine("using MySql.Data.MySqlClient;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Application.Dtos;");
            content.AppendLine($"using {GetNameRootProjectName()}.Core.Application.Cqrs.QueryProviders;");

            content.AppendLine("");
            content.AppendLine(GetNameSpace(filePath));

            content.AppendLine("{");

            var newClassName = string.Concat(originalClassName, "CqrsQueryProvider");

            content.AppendLine(string.Concat("\tpublic class ", newClassName, $" : DapperCqrsQueryProvider<{originalClassName}Output>, I{originalClassName}CqrsQueryProvider"));

            content.AppendLine("\t{");

            GeneratePrivateVariables(content, originalClassName, properties);

            GenerateRepositoryConstructor(content, originalClassName, newClassName);

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
            content.AppendLine($"\t\tpublic override async Task<{originalClassName}Output> GetById(Guid id)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\tvar sqlCommand = SqlSelectCommand");
            content.AppendLine($"\t\t\t\t + CommonConstants.StringEnter");
            content.AppendLine($"\t\t\t\t + \"Where\" + CommonConstants.StringEnter");
            content.AppendLine($"\t\t\t\t + \"t.Id = @Id;\";");
            content.AppendLine();
            content.AppendLine($"\t\t\tvar parameters = new DynamicParameters();");
            content.AppendLine();
            content.AppendLine($"\t\t\tparameters.Add(\"@Id\", id);");
            content.AppendLine();
            content.AppendLine($"\t\t\treturn await _connection.QueryFirstOrDefaultAsync<{originalClassName}Output>(sqlCommand, parameters);");
            content.AppendLine("\t\t}");
            content.AppendLine();
            content.AppendLine($"\t\tpublic override async Task<int> Count(IList<SearchFilterInput> filters)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\tvar sqlCommand = SqlCountSelectCommand");
            content.AppendLine($"\t\t\t\t + CommonConstants.StringEnter;");
            content.AppendLine();
            content.AppendLine($"\t\t\tDictionary<string, object> parameters;");
            content.AppendLine($"\t\t\tstring sqlFilters;");
            content.AppendLine();
            content.AppendLine($"\t\t\tCreateParameters(filters, out parameters, out sqlFilters);");
            content.AppendLine();
            content.AppendLine($"\t\t\tsqlCommand += \" WHERE \" + sqlFilters;");
            content.AppendLine();
            content.AppendLine($"\t\t\tvar dapperParameters = new DynamicParameters(parameters);");
            content.AppendLine();
            content.AppendLine($"\t\t\treturn await _connection.ExecuteScalarAsync<int>(sqlCommand, dapperParameters);");
            content.AppendLine("\t\t}");
            content.AppendLine();
            content.AppendLine($"\t\tpublic override async Task<IList<{originalClassName}Output>> GetPaginatedResults(IList<SearchFilterInput> filters, int pageNumber, int itemsPerPage)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\tvar sqlCommand = SqlSelectCommand");
            content.AppendLine($"\t\t\t\t + CommonConstants.StringEnter;");
            content.AppendLine();
            content.AppendLine($"\t\t\tDictionary<string, object> parameters;");
            content.AppendLine($"\t\t\tstring sqlFilters;");
            content.AppendLine();
            content.AppendLine($"\t\t\tCreateParameters(filters, out parameters, out sqlFilters);");
            content.AppendLine();
            content.AppendLine($"\t\t\tsqlCommand += \" WHERE \" + sqlFilters +");
            content.AppendLine($"\t\t\t\t\t\" ORDER BY NAME LIMIT @PAGE_NUMBER, @ITENS_PER_PAGE \";");
            content.AppendLine();
            content.AppendLine($"\t\t\tparameters.Add(\"PAGE_NUMBER\", (pageNumber - 1) * itemsPerPage);");
            content.AppendLine($"\t\t\tparameters.Add(\"ITENS_PER_PAGE\", itemsPerPage);");
            content.AppendLine();
            content.AppendLine($"\t\t\tvar dapperParameters = new DynamicParameters(parameters);");
            content.AppendLine();
            content.AppendLine($"\t\t\treturn (await _connection.QueryAsync<{originalClassName}Output>(sqlCommand, parameters)).ToList();");
            content.AppendLine("\t\t}");
            content.AppendLine();
            content.AppendLine($"\t\tprivate static void CreateParameters(IList<SearchFilterInput> filters, out Dictionary<string, object> parameters, out string sqlFilters)");
            content.AppendLine("\t\t{");
            content.AppendLine("\t\t\tparameters = new Dictionary<string, object>();");
            content.AppendLine();
            content.AppendLine("\t\t\tsqlFilters = string.Empty;");
            content.AppendLine("\t\t\tforeach (var filter in filters)");
            content.AppendLine("\t\t\t{");
            content.AppendLine("\t\t\t\tif (filter.FilterValue is not null)");
            content.AppendLine("\t\t\t\t{");
            content.AppendLine("\t\t\t\t\tif (string.IsNullOrWhiteSpace(sqlFilters))");
            content.AppendLine("\t\t\t\t\t\tsqlFilters += \" AND \";");
            content.AppendLine();
            content.AppendLine("\t\t\t\t\tsqlFilters += filter.GetSqlFilter();");
            content.AppendLine();
            content.AppendLine("\t\t\t\t\tvar filterParameters = filter.GetParameters();");
            content.AppendLine();
            content.AppendLine("\t\t\t\t\tparameters.ToList().ForEach(x => filterParameters.Add(x.Key, x.Value));");
            content.AppendLine("\t\t\t\t}");
            content.AppendLine("\t\t\t}");
            content.AppendLine("\t\t}");
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
            content.AppendLine();
            content.AppendLine($"\t\tprivate readonly string SqlCountSelectCommand = @\"SELECT");
            content.AppendLine($"\t\t\t\t\t\t\t\t\t\t\t Count(t.Id) as Count");
            content.AppendLine($"\t\t\t\t\t\t\t\t\t\t\t FROM Person t;\";");
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