using System.Windows.Controls;
using LookupTableEditor.ViewModels;

namespace LookupTableEditor.Views.Pages;

/// <summary>
///     Логика взаимодействия для TableFormulasPage.xaml
/// </summary>
public partial class TableFormulasPage : Page
{
    public TableFormulasPage(TableFormulasViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}