﻿using System.Data;
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
                if (_vm.SizeTableInfo is null)
                    return;
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
            _vm.PasteFromClipboard();
        }

        private int? SelectedRowIndex()
        {
            if (dg_Table.SelectedCells[0].Item is DataRowView rowView)
            {
                return _vm.SizeTableInfo?.Table.Rows.IndexOf(rowView.Row);
            }
            else
            {
                return _vm.SizeTableInfo?.Table.Rows.Count;
            }
        }

        private void dg_Table_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var indx = _vm.SizeTableInfo?.Table.Columns.IndexOf(
                _vm.SizeTableInfo.Table.Columns[dg_Table.CurrentColumn.Header.ToString()]
            );
            _vm.SelectedColumnIndex = indx ?? 0;

            _vm.SelectedRowIndex = SelectedRowIndex();
        }

        private void dg_Table_ColumnReordering(object sender, DataGridColumnReorderingEventArgs e)
        {
            if (e.Column.Header.ToString() == "_")
            {
                e.Cancel = true;
                return;
            }

            _vm.SizeTableInfo.Table.Columns[e.Column.Header.ToString()]
                .SetOrdinal(e.Column.DisplayIndex);
        }
    }
}
