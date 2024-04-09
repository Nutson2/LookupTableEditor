using System.Data;
using System.Windows.Controls;
using System.Windows.Input;

namespace LookupTableEditor.Views
{
    /// <summary>
    /// Логика взаимодействия для TablePresenterPage.xaml
    /// </summary>
    public partial class TableContentPage : Page
    {
        private readonly TableContentPageViewModel _vm;

        public TableContentPage(TableContentPageViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            _vm = vm;
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.V || Keyboard.Modifiers != ModifierKeys.Control)
                return;
            var columnIndex = dg_Table.CurrentColumn.DisplayIndex;
            var table = ((DataView)dg_Table.ItemsSource).Table;

            var rowIndex = table.Rows.IndexOf(((DataRowView)dg_Table.SelectedCells[0].Item).Row);

            _vm.PasteFromClipboard(rowIndex, columnIndex);
        }

        private void dg_Table_ColumnReordered(object sender, DataGridColumnEventArgs e) { }
    }
}
