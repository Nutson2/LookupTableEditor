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
            _vm.OnColumnNameChanged = () =>
            {
                for (int i = 0; i < dg_Table.Columns.Count; i++)
                {
                    dg_Table.Columns[i].Header = _vm.SizeTableInfo.Table.Columns[i].Caption;
                }
            };
        }

        private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.V || Keyboard.Modifiers != ModifierKeys.Control)
                return;
            var columnIndex = dg_Table.CurrentColumn.DisplayIndex;
            int rowIndex = SelectedRowIndex();

            _vm.PasteFromClipboard(rowIndex, columnIndex);
        }

        private int SelectedRowIndex() =>
            _vm.SizeTableInfo.Table.Rows.IndexOf(((DataRowView)dg_Table.SelectedCells[0].Item).Row);

        private void dg_Table_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            _vm.SelectedColumnIndex = dg_Table.CurrentColumn.DisplayIndex;
        }
    }
}
