using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using LookupTableEditor.Models;
using LookupTableEditor.ViewModels;

namespace LookupTableEditor.Views;

/// <summary>
///     Логика взаимодействия для MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private BlurEffect _blurEffect = new BlurEffect() { Radius = 6 };
	private readonly MainViewModel _viewModel;
	private DataColumnCollection? columns;
	public MainWindow(MainViewModel viewModel)
	{
		InitializeComponent();
		DataContext = viewModel;
		_viewModel = viewModel;
		_viewModel.OnPageLoaded += ViewModel_OnPageLoaded;
		columns = _viewModel.SizeTableInfo?.Table.Columns;

	}

	private void ViewModel_OnPageLoaded(System.Windows.Controls.Page? obj)
	{
		MainBody.Effect = obj is null ? null : _blurEffect;
		MainBody.IsHitTestVisible = obj is null;
	}

	private void Page_PreviewKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
		{
			_viewModel.PasteFromClipboard();
		}
		if (e.Key == Key.Delete)
		{
			var cells = new List<Cell>();
			foreach (var sc in dg_Table.SelectedCells)
			{
				if (sc.Item is not DataRowView drv)
					continue;
				var ri = GetRowIndex(drv);
				var ci = GetColumnIndex(sc.Column.Header.ToString());
				if (!ci.HasValue || !ri.HasValue)
					continue;

				cells.Add(new Cell(string.Empty, ri.Value, ci.Value));
			}

			_viewModel.ClearSelection(cells);
		}
	}

	private int? SelectedRowIndex()
	{
		if (dg_Table.SelectedCells.Count == 0)
			return 0;

		if (dg_Table.SelectedCells[0].Item is DataRowView rowView)
		{
			return GetRowIndex(rowView);
		}

		return _viewModel.SizeTableInfo?.Table.Rows.Count;
	}

	private int? GetRowIndex(DataRowView rowView) =>
		_viewModel.SizeTableInfo?.Table.Rows.IndexOf(rowView.Row);
	private int? GetColumnIndex(string columnName)
	{
		int? indx = columns?.IndexOf(columns[columnName]);
		return indx;
	}

	private void dg_Table_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
	{
		if (dg_Table.CurrentColumn is null)
			return;

		var indx = GetColumnIndex(dg_Table.CurrentColumn.Header.ToString() ?? string.Empty);

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
