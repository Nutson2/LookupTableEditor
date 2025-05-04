using System.Windows.Controls;
using LookupTableEditor.Models;
using LookupTableEditor.ViewModels.Dialog;

namespace LookupTableEditor.Views.Pages;

/// <summary>
///     Логика взаимодействия для TableDefinitionPage.xaml
/// </summary>
public partial class SelectNewColumnPage : Page
{
    public SelectNewColumnPage(SelectNewColumnViewModel selectNewColumnViewModel)
    {
        InitializeComponent();
        DataContext = selectNewColumnViewModel;
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (FamilyParameterModel fp in e.AddedItems)
        {
            fp.IsSelected = true;
        }

        foreach (FamilyParameterModel fp in e.RemovedItems)
        {
            fp.IsSelected = false;
        }
    }
}
