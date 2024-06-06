using System.IO;
using System.Linq;

namespace BestPracticesCodeGenerator
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var doc = await VS.Documents.GetActiveDocumentViewAsync();

            var solution = await VS.Solutions.GetCurrentSolutionAsync();

            var span = doc.TextView.Selection.SelectedSpans.FirstOrDefault();
            var originalFileName = doc.FilePath;

            var fileContent = File.ReadAllText(originalFileName);

            var originalFilePath = Path.GetDirectoryName(originalFileName);
            var fileName = Path.GetFileName(originalFileName);

            var newFileName = string.Concat("I", fileName.Substring(0, fileName.Length - 3), "Repository.cs");

            var filePath = GetInterfaceRepositoryPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceRepositoryFactory.Create(fileContent, filePath));

            newFileName = string.Concat("I", fileName.Substring(0, fileName.Length - 3), "CommandProvider.cs");

            filePath = GetInterfaceCommandProviderPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceCommandProviderFactory.Create(fileContent, filePath));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "Repository.cs");

            filePath = GetRepositoryPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), RepositoryFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Create", fileName.Substring(0, fileName.Length - 3), "UseCase.cs");

            filePath = GetUseCasesPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), CreateUseCaseFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "UseCase.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateUseCaseFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Delete", fileName.Substring(0, fileName.Length - 3), "UseCase.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), DeleteUseCaseFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Create", fileName.Substring(0, fileName.Length - 3), "Input.cs");

            filePath = GetInputDtoPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputDtoFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "Input.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputDtoFactory.Create(fileContent, filePath));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "Output.cs");

            filePath = GetOutputDtoPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), OutputDtoFactory.Create(fileContent, filePath));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "Command.cs");

            filePath = GetDapperCommandPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), DapperCommandFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Create", fileName.Substring(0, fileName.Length - 3), "UseCaseTests.cs");

            filePath = GetUseCasesTestsPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), CreateUseCaseTestsFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Delete", fileName.Substring(0, fileName.Length - 3), "UseCaseTests.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), DeleteUseCaseTestsFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "UseCaseTests.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateUseCaseTestsFactory.Create(fileContent, filePath));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "Tests.cs");

            filePath = GetEntityTestsPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), EntityTestsFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Create", fileName.Substring(0, fileName.Length - 3), "InputValidator.cs");

            filePath = GetInputValidatorPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputValidatorFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "InputValidator.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputValidatorFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Dapper", fileName.Substring(0, fileName.Length - 3), "TableDefinition.cs");

            filePath = GetDapperTableDefinitionPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), DapperTableDefinitionFactory.Create(fileContent, filePath));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "InputValidatorTests.cs");

            filePath = GetInputValidatorTestsPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), InputValidatorTestsFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Create", fileName.Substring(0, fileName.Length - 3), "InputBuilder.cs");

            filePath = GetInputBuilderPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputBuilderFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "InputBuilder.cs");

            File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputBuilderFactory.Create(fileContent, filePath));

            newFileName = string.Concat("Update", fileName.Substring(0, fileName.Length - 3), "InputBuilder.cs");

            filePath = GetOutputBuilderPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), OutputBuilderFactory.Create(fileContent, filePath));

            newFileName = string.Concat(fileName.Substring(0, fileName.Length - 3), "CqrsCommandProvider.cs");

            filePath = GetDapperCommandProviderPath(solution, originalFilePath);

            File.WriteAllText(Path.Combine(filePath, newFileName), DapperCommandProviderFactory.Create(fileContent, filePath));

            await VS.MessageBox.ShowWarningAsync("MyCommand", doc.WindowFrame.Caption);
        }

        private string GetDapperCommandProviderPath(Solution solution, string originalFilePath)
        {
            var commandProviderProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".CommandProvider.Dapper"))
               .FirstOrDefault();

            var commandProvidersFolder = commandProviderProject?.Children.Where(n => n.Text.Equals("CommandProviders"))
                .FirstOrDefault();

            return (commandProvidersFolder is not null) ? commandProvidersFolder.FullPath : originalFilePath;
        }

        private string GetOutputBuilderPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
                .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            var buildersFolder = dtosFolder?.Children.Where(n => n.Text.Equals("Builders"))
               .FirstOrDefault();

            return (buildersFolder is not null) ? buildersFolder.FullPath : originalFilePath;
        }

        private string GetInputBuilderPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
                 .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            var buildersFolder = dtosFolder?.Children.Where(n => n.Text.Equals("Builders"))
               .FirstOrDefault();

            return (buildersFolder is not null) ? buildersFolder.FullPath : originalFilePath;
        }

        private string GetInputValidatorTestsPath(Solution solution, string originalFilePath)
        {
            var coreTestsProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
              .FirstOrDefault();

            var applicationFolder = coreTestsProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            var validatorsFolder = dtosFolder?.Children.Where(n => n.Text.Equals("Validators"))
               .FirstOrDefault();

            return (validatorsFolder is not null) ? validatorsFolder.FullPath : originalFilePath;
        }

        private string GetDapperTableDefinitionPath(Solution solution, string originalFilePath)
        {
            var commandProviderProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".CommandProvider.Dapper"))
               .FirstOrDefault();

            var tableDefinitionsFolder = commandProviderProject?.Children.Where(n => n.Text.Equals("TableDefinitions"))
                .FirstOrDefault();

            return (tableDefinitionsFolder is not null) ? tableDefinitionsFolder.FullPath : originalFilePath;
        }

        private string GetInputValidatorPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                 .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            var validatorsFolder = dtosFolder?.Children.Where(n => n.Text.Equals("Validators"))
               .FirstOrDefault();

            return (validatorsFolder is not null) ? validatorsFolder.FullPath : originalFilePath;
        }

        private string GetEntityTestsPath(Solution solution, string originalFilePath)
        {
            var coreTestsProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
               .FirstOrDefault();

            var domainFolder = coreTestsProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var modelsFolder = domainFolder?.Children.Where(n => n.Text.Equals("Models"))
                .FirstOrDefault();

            return (modelsFolder is not null) ? modelsFolder.FullPath : originalFilePath;
        }

        private string GetUseCasesTestsPath(Solution solution, string originalFilePath)
        {
            var coreTestsProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core.Tests"))
                .FirstOrDefault();

            var applicationFolder = coreTestsProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var useCasesFolder = applicationFolder?.Children.Where(n => n.Text.Equals("UseCases"))
                .FirstOrDefault();

            return (useCasesFolder is not null) ? useCasesFolder.FullPath : originalFilePath;
        }

        private string GetDapperCommandPath(Solution solution, string originalFilePath)
        {
            var commandProviderProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".CommandProvider.Dapper"))
               .FirstOrDefault();

            var entityCommandsFolder = commandProviderProject?.Children.Where(n => n.Text.Equals("EntityCommands"))
                .FirstOrDefault();

            return (entityCommandsFolder is not null) ? entityCommandsFolder.FullPath : originalFilePath;
        }

        private string GetOutputDtoPath(Solution solution, string originalFilePath)
        {
            return GetInputDtoPath(solution, originalFilePath);
        }

        private string GetInputDtoPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                  .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var dtosFolder = applicationFolder?.Children.Where(n => n.Text.Equals("Dtos"))
                .FirstOrDefault();

            return (dtosFolder is not null) ? dtosFolder.FullPath : originalFilePath;
        }

        private string GetUseCasesPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                  .FirstOrDefault();

            var applicationFolder = coreProject?.Children.Where(n => n.Text.Equals("Application"))
                .FirstOrDefault();

            var useCasesFolder = applicationFolder?.Children.Where(n => n.Text.Equals("UseCases"))
                .FirstOrDefault();

            return (useCasesFolder is not null) ? useCasesFolder.FullPath : originalFilePath;
        }

        private string GetRepositoryPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                 .FirstOrDefault();

            var domainFolder = coreProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var repositoryFolder = domainFolder?.Children.Where(n => n.Text.Equals("Repositories"))
                .FirstOrDefault();

            return (repositoryFolder is not null) ? repositoryFolder.FullPath : originalFilePath;
        }

        private string GetInterfaceCommandProviderPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
               .FirstOrDefault();

            var domainFolder = coreProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var cqrsFolder = domainFolder?.Children.Where(n => n.Text.Equals("Cqrs"))
                .FirstOrDefault();

            var commandProvidersFolder = cqrsFolder?.Children.Where(n => n.Text.Equals("CommandProviders"))
                .FirstOrDefault();

            return (commandProvidersFolder is not null) ? commandProvidersFolder.FullPath : originalFilePath;
        }

        private string GetInterfaceRepositoryPath(Solution solution, string originalFilePath)
        {
            var coreProject = solution.Children.ToList().Where(c => c.Name.EndsWith(".Core"))
                .FirstOrDefault();

            var domainFolder = coreProject?.Children.Where(n => n.Text.Equals("Domain"))
                .FirstOrDefault();

            var repositoryFolder = domainFolder?.Children.Where(n => n.Text.Equals("Repositories"))
                .FirstOrDefault();

            var repositoryInterfaceFolder = repositoryFolder?.Children.Where(n => n.Text.Equals("Interfaces"))
                .FirstOrDefault();

            return (repositoryInterfaceFolder is not null) ? repositoryInterfaceFolder.FullPath : originalFilePath;
        }
    }
}
