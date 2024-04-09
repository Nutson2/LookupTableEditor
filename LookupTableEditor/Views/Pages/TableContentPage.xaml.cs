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
            int rowIndex = SelecterRowIndex();

            _vm.PasteFromClipboard(rowIndex, columnIndex);
        }

        private int SelecterRowIndex() =>
            _vm.SizeTableInfo.Table.Rows.IndexOf(((DataRowView)dg_Table.SelectedCells[0].Item).Row);

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            string menuHeader = menuItem.Header.ToString();

            if (menuHeader == _vm.AddNewColumnText)
                _vm.AddColumn();
            else if (menuHeader == _vm.AddNewRowText)
                _vm.AddRowOnTop(SelecterRowIndex());
        }
    }
}
