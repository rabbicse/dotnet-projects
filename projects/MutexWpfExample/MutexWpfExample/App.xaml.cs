using System.Diagnostics;
using System.Windows;

namespace MutexWpfExample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string AppName = "MutexExample";
    private static Mutex? _mutex;

    #region Application
    protected override void OnStartup(StartupEventArgs e)
    {
        // If it's not the new instance, exit the application
        if (!IsNewInstance())
        {
            MessageBox.Show(
                "Another instance of the application is already running.",
                "Instance Check by Mutex!",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            App.Current.Shutdown();
            return;
        }

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
        base.OnExit(e);
    }
    #endregion


    #region Mutex
    private static bool IsNewInstance()
    {
        _mutex = new Mutex(true, AppName, out var newInstance);
        Debug.WriteLine($"Checking for another instance of mutex : {newInstance}");
        return newInstance;
    }
    #endregion
}

