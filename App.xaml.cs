using System.Windows;
using System.Diagnostics;

namespace ProGlassAutomation
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Debug: check if MainWindow loads
            Debug.WriteLine("App started successfully.");
        }
    }
}