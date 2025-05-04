using System.Windows.Controls;
using LookupTableEditor.ViewModels.Dialog;

namespace LookupTableEditor.Views.Pages;

public partial class ResultDialog : Page
{
    public ResultDialog(ResultVM resultVM)
    {
        InitializeComponent();
        DataContext = resultVM;
    }
}
