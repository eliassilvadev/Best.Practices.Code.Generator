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

            var frmOptions = new frmCodeGenerationOptions();

            frmOptions.OriginalFileName = doc.FilePath;
            frmOptions.Solution = solution;

            frmOptions.ShowDialog();
        }
    }
}
