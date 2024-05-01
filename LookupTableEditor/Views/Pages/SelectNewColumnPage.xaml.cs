using System.Windows.Controls;
using LookupTableEditor.Models;
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (FamilyParameterExtended fp in e.AddedItems)
            {
                fp.IsSelected = true;
            }
            foreach (FamilyParameterExtended fp in e.RemovedItems)
            {
                fp.IsSelected = false;
            }
        }
    }
}
