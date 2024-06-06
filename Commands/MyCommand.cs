using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using System.Linq;

namespace BestPracticesCodeGenerator
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var doc = VS.Documents.GetActiveDocumentViewAsync().Result;

            var solution = VS.Solutions.GetCurrentSolutionAsync().Result;

            var span = doc.TextView.Selection.SelectedSpans.FirstOrDefault();

            var window = this.Package.FindToolWindow(typeof(frmCodeGenerationOptions), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Não foi possível criar a janela de ferramentas.");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;

            var frm = ((frmCodeGenerationOptionsControl)window.Content);

            frm.Solution = solution;
            frm.OriginalFileName = doc.FilePath;
            frm.FileContent = File.ReadAllText(frm.OriginalFileName);

            frm.OriginalFilePath = Path.GetDirectoryName(frm.OriginalFileName);
            frm.FileName = Path.GetFileName(frm.OriginalFileName);

            frm.ClassProperties = frm.GetPropertiesInfo();

            frm.GRD_Properties.ItemsSource = frm.ClassProperties;
            frm.BTN_Generate.IsEnabled = true;
            frm.BTN_Reload.IsEnabled = true;
            frm.PNL_InstructionsToLoadClass.Visibility = System.Windows.Visibility.Hidden;
            frm.PNL_GenerateClasses.Visibility = System.Windows.Visibility.Visible;

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
