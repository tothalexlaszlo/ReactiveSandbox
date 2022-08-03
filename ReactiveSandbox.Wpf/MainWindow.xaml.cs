using ReactiveSandbox.Shared.ViewModels;
using System;
using System.Reactive.Disposables;
using System.Windows;

namespace ReativeSandbox.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IDisposable
{
    private CompositeDisposable _cleanup = new();
    private bool _disposedValue;

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        _ = viewModel.ConfirmClean.RegisterHandler(
            async context =>
            {
                var result = MessageBox.Show("Are you sure you want to clear tracks'?", "Confirm Clear", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                context.SetOutput(result);
            })
            .DisposeWith(_cleanup);
    }

    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _cleanup.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
