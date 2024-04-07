using System.Windows.Controls;

namespace LookupTableEditor.Views
{
    /// <summary>
    /// Логика взаимодействия для TableDefinitionPage.xaml
    /// </summary>
    public partial class TableDefinitionPage : Page
    {
        public TableDefinitionPage(TableDefinitionPageViewModel tableDefinitionPageViewModel)
        {
            InitializeComponent();
            DataContext = tableDefinitionPageViewModel;
        }
    }
}
