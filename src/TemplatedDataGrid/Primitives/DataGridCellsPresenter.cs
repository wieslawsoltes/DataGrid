﻿using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace TemplatedDataGrid.Primitives
{
    public class DataGridCellsPresenter : TemplatedControl
    {
        internal static readonly DirectProperty<DataGridCellsPresenter, object?> SelectedItemProperty =
            AvaloniaProperty.RegisterDirect<DataGridCellsPresenter, object?>(
                nameof(SelectedItem), 
                o => o.SelectedItem, 
                (o, v) => o.SelectedItem = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<DataGridCellsPresenter, object?> SelectedCellProperty =
            AvaloniaProperty.RegisterDirect<DataGridCellsPresenter, object?>(
                nameof(SelectedCell), 
                o => o.SelectedCell, 
                (o, v) => o.SelectedCell = v,
                defaultBindingMode: BindingMode.TwoWay);

        internal static readonly DirectProperty<DataGridCellsPresenter, AvaloniaList<DataGridColumn>?> ColumnsProperty =
            AvaloniaProperty.RegisterDirect<DataGridCellsPresenter, AvaloniaList<DataGridColumn>?>(
                nameof(Columns), 
                o => o.Columns, 
                (o, v) => o.Columns = v);

        internal static readonly DirectProperty<DataGridCellsPresenter, AvaloniaList<DataGridCell>> CellsProperty =
            AvaloniaProperty.RegisterDirect<DataGridCellsPresenter, AvaloniaList<DataGridCell>>(
                nameof(Cells), 
                o => o.Cells, 
                (o, v) => o.Cells = v);

        private object? _selectedItem;
        private object? _selectedCell;
        private AvaloniaList<DataGridColumn>? _columns;
        private AvaloniaList<DataGridCell> _cells = new AvaloniaList<DataGridCell>();
        private Grid? _root;
        private readonly List<Control> _rootChildren = new List<Control>();

        internal object? SelectedItem
        {
            get => _selectedItem;
            set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
        }

        internal object? SelectedCell
        {
            get => _selectedCell;
            set => SetAndRaise(SelectedCellProperty, ref _selectedCell, value);
        }

        internal AvaloniaList<DataGridColumn>? Columns
        {
            get => _columns;
            set => SetAndRaise(ColumnsProperty, ref _columns, value);
        }

        internal AvaloniaList<DataGridCell> Cells
        {
            get => _cells;
            set => SetAndRaise(CellsProperty, ref _cells, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _root = e.NameScope.Find<Grid>("PART_Root");

            InvalidateRoot();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SelectedItemProperty)
            {
                // TODO:
            }

            if (change.Property == SelectedCellProperty)
            {
                // TODO:
            }

            if (change.Property == ColumnsProperty)
            {
                InvalidateRoot();
            }
        }

        private void InvalidateRoot()
        {
            if (_root is null)
            {
                return;
            }

            foreach (var child in _rootChildren)
            {
                _root.Children.Remove(child);
            }

            _cells.Clear();

            var columns = Columns;
            if (columns is null)
            {
                return;
            }
 
            // Generate ColumnDefinitions

            var columnDefinitions = new List<ColumnDefinition>();

            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                var isAutoWidth = column.Width == GridLength.Auto;

                var columnDefinition = new ColumnDefinition()
                {
                    [!ColumnDefinition.WidthProperty] = column[!DataGridColumn.WidthProperty],
                    [!ColumnDefinition.MinWidthProperty] = column[!DataGridColumn.MinWidthProperty],
                    [!ColumnDefinition.MaxWidthProperty] = column[!DataGridColumn.MaxWidthProperty]
                };

                if (isAutoWidth)
                {
                    columnDefinition.SetValue(DefinitionBase.SharedSizeGroupProperty, $"Column{i}");
                }

                columnDefinitions.Add(columnDefinition);

                // Generate DataGridCell's

                DataGridCell cell;

                if (column is DataGridTemplateColumn templateColumn)
                {
                    cell = new DataGridCell()
                    {
                        [!!DataGridCell.SelectedItemProperty] = this[!!DataGridCellsPresenter.SelectedItemProperty],
                        [!!DataGridCell.SelectedCellProperty] = this[!!DataGridCellsPresenter.SelectedCellProperty],
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!DataGridCell.ContentProperty] = this[!DataGridCellsPresenter.DataContextProperty],
                        [!DataGridCell.CellTemplateProperty] = templateColumn[!DataGridColumn.CellTemplateProperty]
                    };
                }
                else if (column is DataGridTextColumn textColumn)
                {
                    cell = new DataGridCell()
                    {
                        [!!DataGridCell.SelectedItemProperty] = this[!!DataGridCellsPresenter.SelectedItemProperty],
                        [!!DataGridCell.SelectedCellProperty] = this[!!DataGridCellsPresenter.SelectedCellProperty],
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!DataGridCell.ContentProperty] = this[!DataGridCellsPresenter.DataContextProperty],
                        [!DataGridCell.CellTemplateProperty] = textColumn[!DataGridColumn.CellTemplateProperty]
                    };
                }
                else if (column is DataGridCheckBoxColumn checkBoxColumn)
                {
                    cell = new DataGridCell()
                    {
                        [!!DataGridCell.SelectedItemProperty] = this[!!DataGridCellsPresenter.SelectedItemProperty],
                        [!!DataGridCell.SelectedCellProperty] = this[!!DataGridCellsPresenter.SelectedCellProperty],
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!DataGridCell.ContentProperty] = this[!DataGridCellsPresenter.DataContextProperty],
                        [!DataGridCell.CellTemplateProperty] = checkBoxColumn[!DataGridColumn.CellTemplateProperty]
                    };
                }
                else
                {
                    cell = new DataGridCell()
                    {
                        [!!DataGridCell.SelectedItemProperty] = this[!!DataGridCellsPresenter.SelectedItemProperty],
                        [!!DataGridCell.SelectedCellProperty] = this[!!DataGridCellsPresenter.SelectedCellProperty],
                        [Grid.ColumnProperty] = columnDefinitions.Count - 1,
                        [!DataGridCell.ContentProperty] = this[!DataGridCellsPresenter.DataContextProperty],
                        [!DataGridCell.CellTemplateProperty] = column[!DataGridColumn.CellTemplateProperty]
                    };
                }

                _cells.Add(cell);
                _rootChildren.Add(cell);

                if (i < columns.Count)
                {
                    columnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Pixel)));
                }
            }

            columnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            _root.ColumnDefinitions.Clear();
            _root.ColumnDefinitions.AddRange(columnDefinitions);

            foreach (var child in _rootChildren)
            {
                _root.Children.Add(child);
            }
        }
    }
}
