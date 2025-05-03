using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using LookupTableEditor.ViewModels;

namespace LookupTableEditor.Views;

/// <summary>
///     Логика взаимодействия для MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private BlurEffect _blurEffect = new BlurEffect() { Radius = 6 };
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        _viewModel.OnPageLoaded += ViewModel_OnPageLoaded;
    }

    private void ViewModel_OnPageLoaded(System.Windows.Controls.Page? obj)
    {
        MainBody.Effect = obj is null ? null : _blurEffect;
        MainBody.IsHitTestVisible = obj is null;
    }

    private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.V || Keyboard.Modifiers != ModifierKeys.Control)
            return;
        _viewModel.PasteFromClipboard();
    }

    private int? SelectedRowIndex()
    {
        if (dg_Table.SelectedCells.Count == 0)
            return 0;

        if (dg_Table.SelectedCells[0].Item is DataRowView rowView)
        {
            return _viewModel.SizeTableInfo?.Table.Rows.IndexOf(rowView.Row);
        }

        return _viewModel.SizeTableInfo?.Table.Rows.Count;
    }

    private void dg_Table_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (dg_Table.CurrentColumn is null)
            return;
        DataColumnCollection? columns = _viewModel.SizeTableInfo?.Table.Columns;
        int? indx = columns?.IndexOf(
            columns[dg_Table.CurrentColumn.Header.ToString() ?? string.Empty]
        );

        _viewModel.SelectedColumnIndex = indx ?? 0;
        _viewModel.SelectedRowIndex = SelectedRowIndex();
    }

    private void dg_Table_ColumnReordering(object sender, DataGridColumnReorderingEventArgs e)
    {
        if (e.Column.Header.ToString() == "_")
        {
            e.Cancel = true;
        }
    }

    private void dg_Table_ColumnReordered(object sender, DataGridColumnEventArgs e)
    {
        int newPosition = e.Column.DisplayIndex;

        if (newPosition == 0)
        {
            newPosition++;
            e.Column.DisplayIndex = newPosition;
        }

        _viewModel
            .SizeTableInfo?.Table.Columns[e.Column.Header.ToString() ?? string.Empty]
            ?.SetOrdinal(newPosition);
    }
}
