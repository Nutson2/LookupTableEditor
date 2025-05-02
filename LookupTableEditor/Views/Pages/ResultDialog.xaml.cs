using System.Windows.Controls;
using LookupTableEditor.ViewModels;

namespace LookupTableEditor.Views.Pages;

public partial class ResultDialog : Page
{
    public ResultDialog(ResultVM resultVM)
    {
        InitializeComponent();
        DataContext = resultVM;
    }
}
