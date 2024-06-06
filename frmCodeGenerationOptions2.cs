using BestPracticesCodeGenerator.Dtos;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BestPracticesCodeGenerator
{
    public partial class frmCodeGenerationOptions2 : Form
    {
        public Solution Solution { get; internal set; }
        public string OriginalFileName { get; internal set; }
        public string FileContent { get; private set; }
        public string OriginalFilePath { get; private set; }
        public string FileName { get; private set; }
        public IList<PropertyInfo> ClassProperties { get; private set; }

        public frmCodeGenerationOptions2()
        {
            InitializeComponent();
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

        private async Task GenerateCreateUseCaseAsync()
        {
        }

        private async Task GenerateUpdateUseCaseAsync()
        {
        }

        private async Task GenerateDeleteUseCaseAsync()
        {
        }

        private async Task GenerateAsync()
        {
            var newFileName = "";

            newFileName = string.Concat("I", FileName.Substring(0, FileName.Length - 3), "Repository.cs");
            var filePath = GetInterfaceRepositoryPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceRepositoryFactory.Create(FileContent, ClassProperties, filePath));


            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Repository.cs");
            filePath = GetRepositoryPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), RepositoryFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Command.cs");
            filePath = GetDapperCommandPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperCommandFactory.Create(FileContent, filePath));

            newFileName = string.Concat("I", FileName.Substring(0, FileName.Length - 3), "CommandProvider.cs");
            filePath = GetInterfaceCommandProviderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), InterfaceCommandProviderFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "CqrsCommandProvider.cs");
            filePath = GetDapperCommandProviderPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperCommandProviderFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat("Dapper", FileName.Substring(0, FileName.Length - 3), "TableDefinition.cs");
            filePath = GetDapperTableDefinitionPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), DapperTableDefinitionFactory.Create(FileContent, ClassProperties, filePath));

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Tests.cs");
            filePath = GetEntityTestsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), EntityTestsFactory.Create(FileContent, ClassProperties, filePath));

            if (SEL_GenerateCreateUseCase.Checked)
            {
                newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "Input.cs");
                filePath = GetInputDtoPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputDtoFactory.Create(FileContent, ClassProperties, filePath));

                newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "InputValidator.cs");
                filePath = GetInputValidatorPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputValidatorFactory.Create(FileContent, ClassProperties, filePath));

                newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "InputBuilder.cs");
                filePath = GetInputBuilderPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), CreateInputBuilderFactory.Create(FileContent, ClassProperties, filePath));


                newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "UseCase.cs");
                filePath = GetUseCasesPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), CreateUseCaseFactory.Create(FileContent, ClassProperties, filePath));


                newFileName = string.Concat("Create", FileName.Substring(0, FileName.Length - 3), "UseCaseTests.cs");
                filePath = GetUseCasesTestsPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), CreateUseCaseTestsFactory.Create(FileContent, ClassProperties, filePath));

                await GenerateCreateUseCaseAsync();
            }

            if (SEL_GenerateUpdateUseCase.Checked)
            {
                filePath = GetInputDtoPath(Solution, OriginalFilePath);
                newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "Input.cs");
                File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputDtoFactory.Create(FileContent, ClassProperties, filePath));


                filePath = GetInputValidatorPath(Solution, OriginalFilePath);
                newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "InputValidator.cs");
                File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputValidatorFactory.Create(FileContent, ClassProperties, filePath));

                newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "InputBuilder.cs");
                filePath = GetInputBuilderPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), UpdateInputBuilderFactory.Create(FileContent, ClassProperties, filePath));

                newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "UseCase.cs");
                filePath = GetUseCasesPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), UpdateUseCaseFactory.Create(FileContent, ClassProperties, filePath));


                filePath = GetUseCasesTestsPath(Solution, OriginalFilePath);
                newFileName = string.Concat("Update", FileName.Substring(0, FileName.Length - 3), "UseCaseTests.cs");
                File.WriteAllText(Path.Combine(filePath, newFileName), UpdateUseCaseTestsFactory.Create(FileContent, ClassProperties, filePath));

                await GenerateUpdateUseCaseAsync();
            }

            if (SEL_GenerateDeleteUseCase.Checked)
            {
                newFileName = string.Concat("Delete", FileName.Substring(0, FileName.Length - 3), "UseCase.cs");
                filePath = GetUseCasesPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), DeleteUseCaseFactory.Create(FileContent, ClassProperties, filePath));


                newFileName = string.Concat("Delete", FileName.Substring(0, FileName.Length - 3), "UseCaseTests.cs");
                File.WriteAllText(Path.Combine(filePath, newFileName), DeleteUseCaseTestsFactory.Create(FileContent, ClassProperties, filePath));

                await GenerateDeleteUseCaseAsync();

            }

            if ((SEL_GenerateCreateUseCase.Checked) || SEL_GenerateUpdateUseCase.Checked || SEL_GenerateDeleteUseCase.Checked)
            {
                newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "Output.cs");
                filePath = GetOutputDtoPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), OutputDtoFactory.Create(FileContent, ClassProperties, filePath));

                newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "OutputBuilder.cs");
                filePath = GetOutputBuilderPath(Solution, OriginalFilePath);
                File.WriteAllText(Path.Combine(filePath, newFileName), OutputBuilderFactory.Create(FileContent, ClassProperties, filePath));
            }

            newFileName = string.Concat(FileName.Substring(0, FileName.Length - 3), "InputValidatorTests.cs");
            filePath = GetInputValidatorTestsPath(Solution, OriginalFilePath);
            File.WriteAllText(Path.Combine(filePath, newFileName), InputValidatorTestsFactory.Create(FileContent, ClassProperties, filePath));

            await VS.MessageBox.ShowWarningAsync("MyCommand", "Classes generated with success!");
        }

        private IList<PropertyInfo> GetPropertiesInfo()
        {
            var propertyes = new List<PropertyInfo>();

            foreach (Match item in Regex.Matches(FileContent, @"(?>public)\s+(?!class)((static|readonly)\s)?(?<Type>(\S+(?:<.+?>)?)(?=\s+\w+\s*\{\s*get))\s+(?<Name>[^\s]+)(?=\s*\{\s*get)"))
            {
                propertyes.Add(new PropertyInfo(item.Groups["Type"].Value, item.Groups["Name"].Value));
            }

            return propertyes;
        }

        private void BTN_Generate_Click(object sender, EventArgs e)
        {
            GenerateAsync().Wait();
        }

        private void BTN_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmCodeGenerationOptions_Shown(object sender, EventArgs e)
        {
            this.FileContent = File.ReadAllText(OriginalFileName);

            this.OriginalFilePath = Path.GetDirectoryName(OriginalFileName);
            this.FileName = Path.GetFileName(OriginalFileName);

            this.ClassProperties = GetPropertiesInfo();

            GRD_Properties.DataSource = this.ClassProperties;

            GRD_Properties.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }
    }
}