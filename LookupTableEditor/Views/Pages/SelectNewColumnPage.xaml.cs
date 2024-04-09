using System.Windows.Controls;
using LookupTableEditor.ViewModels;

namespace LookupTableEditor.Views
{
    /// <summary>
    /// Логика взаимодействия для TableDefinitionPage.xaml
    /// </summary>
    public partial class SelectNewColumnPage : Page
    {
        public SelectNewColumnPage(SelectNewColumnViewModel selectNewColumnViewModel)
        {
            InitializeComponent();
            DataContext = selectNewColumnViewModel;
        }
    }
}
