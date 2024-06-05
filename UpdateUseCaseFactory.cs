using BestPracticesCodeGenerator.Dtos;
using BestPracticesCodeGenerator.Exceptions;
using BestPracticesCodeGenerator.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BestPracticesCodeGenerator
{
    public static class UpdateUseCaseFactory
    {
        public static string Create(string fileContent)
        {
            Validate(fileContent);

            var properties = GetPropertiesInfo(fileContent);

            if (!properties.Any())
                throw new ValidationException("It wasn't identified public properties to generate builder class");

            var originalClassName = GetOriginalClassName(fileContent);

            return CreateUseCaseClass(fileContent, originalClassName, properties);
        }

        private static string CreateUseCaseClass(string fileContent, string originalClassName, IList<PropertyInfo> properties)
        {
            var content = new StringBuilder();

            content.Append(GetUsings(fileContent));

            fileContent = fileContent.Substring(content.Length);

            content.AppendLine("using FluentValidation;");
            content.AppendLine("using Best.Practices.Core.Extensions;");
            content.AppendLine("using Best.Practices.Core.UnitOfWork.Interfaces;");
            content.AppendLine("");
            content.Append(GetNameSpace(fileContent));

            content.AppendLine("{");

            var newClassName = string.Concat(originalClassName, "UpdateUseCase");

            content.AppendLine(string.Concat("\tpublic class ", newClassName, $" : CommandUseCase<Update{originalClassName}Input, {originalClassName}Output>"));

            content.AppendLine("\t{");

            GeneratePrivateVariables(content, originalClassName);

            GenerateRepositoryConstructor(content, originalClassName, newClassName);

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

        private static void GenerateRepositoryConstructor(StringBuilder content, string originalClassName, string newClassName)
        {
            content.AppendLine();
            content.AppendLine($"\t\tpublic {newClassName}(IValidator<Create{originalClassName}Input> validator, I{originalClassName}Repository {originalClassName.GetWordWithFirstLetterDown()}Repository, IUnitOfWork unitOfWork) : base(unitOfWork)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\t_validator = validator;");
            content.AppendLine($"\t\t\t_{originalClassName.GetWordWithFirstLetterDown()}Repository = {originalClassName.GetWordWithFirstLetterDown()}Repository;");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GenerateInternalExecuteMethod(StringBuilder content, string className, IList<PropertyInfo> properties)
        {
            content.AppendLine($"\t\tpublic async Task<UseCaseOutput<{className}Output>> InternalExecuteAsync(Update{className}Input input)");
            content.AppendLine("\t\t{");
            content.AppendLine($"\t\t\t_validator.ValidateAndThrow(input);");
            content.AppendLine("");
            content.AppendLine($"\t\t\tvar previous{className} = _{className.GetWordWithFirstLetterDown()}Repository.GetById(input.Id.Value).Result");
            content.AppendLine($"\t\t\t\t.ThrowResourceNotFoundIfIsNull(Constants.ErrorMessages.{className}WithIdDoesNotExists.Format(input.Id));");
            content.AppendLine("");
            content.AppendLine($"\t\t\t await SaveChangesAsync();");
            content.AppendLine("");
            content.AppendLine($"\t\t\t return CreateSuccessOutput(new {className}Output(new{className}));");
            content.AppendLine("\t\t}");
            content.AppendLine();
        }

        private static void GeneratePrivateVariables(StringBuilder content, string originalClassName)
        {
            content.AppendLine($"\t\tprivate readonly IValidator<Create{originalClassName}Input> _validator;");
            content.AppendLine($"\t\tprivate readonly I{originalClassName}Repository _{originalClassName.GetWordWithFirstLetterDown()}Repository;");
            content.AppendLine($"");
            content.AppendLine($"\t\tprotected override string SaveChangesErrorMessage => \"An error occurred while creating the application.\";");
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