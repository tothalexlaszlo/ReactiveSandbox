using ReactiveSandbox.Shared.ViewModels;
using System.Windows;


namespace ReativeSandbox.Wpf;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
