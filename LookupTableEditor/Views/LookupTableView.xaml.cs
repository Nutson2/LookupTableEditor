using System.Data;
using System.Windows;
using System.Windows.Input;

namespace LookupTableEditor
{
    public partial class LookupTableView : Window
    {
        private readonly LookupTableViewModel vm;

        public LookupTableView(LookupTableViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            this.vm = vm;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.V || Keyboard.Modifiers != ModifierKeys.Control)
                return;
            var columnIndex = dg_Table.CurrentColumn.DisplayIndex;
            var table = ((DataView)dg_Table.ItemsSource).Table;

            var rowIndex = table.Rows.IndexOf(((DataRowView)dg_Table.SelectedCells[0].Item).Row);

            vm.PasteFromClipboard(rowIndex, columnIndex);
        }
    }
}
