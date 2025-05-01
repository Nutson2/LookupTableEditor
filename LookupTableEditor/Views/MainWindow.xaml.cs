using System.Windows;
using System.Windows.Media.Effects;
using LookupTableEditor.ViewModels;

namespace LookupTableEditor.Views;

/// <summary>
///     Логика взаимодействия для MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private BlurEffect _blurEffect = new BlurEffect() { Radius = 6 };
    private readonly BaseViewModel _viewModel;

    public MainWindow(BaseViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        _viewModel.OnPageLoaded += ViewModel_OnPageLoaded;
    }

    private void ViewModel_OnPageLoaded(System.Windows.Controls.Page? obj) =>
        MainBody.Effect = obj is null ? _blurEffect : null;
}
