using System.Windows.Controls;
using LookupTableEditor.ViewModels.Dialog;

namespace LookupTableEditor.Views.Pages;

/// <summary>
/// Логика взаимодействия для RequestTableName.xaml
/// </summary>
public partial class RequestTableName : Page
{
    public RequestTableName(RequestTableNameVM vm)
    {
        InitializeComponent();
        DataContext = vm;
        input.Focus();
    }
}
