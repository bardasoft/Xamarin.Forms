﻿using Gdk;
using Gtk;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms.Platform.GTK.Cells;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Renderers
{
    public class ListViewRenderer : ViewRenderer<ListView, ScrolledWindow>
    {
        public const int DefaultRowHeight = 44;

        private bool _disposed;
        private TreeView _treeView;
        private EventBox _header;
        private EventBox _footer;
        private IVisualElementRenderer _headerRenderer;
        private IVisualElementRenderer _footerRenderer;

        IListViewController Controller => Element;

        ITemplatedItemsView<Cell> TemplatedItemsView => Element;

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var scroller = new ScrolledWindow
                    {
                        CanFocus = true,
                        ShadowType = ShadowType.None,
                        HscrollbarPolicy = PolicyType.Never,
                        VscrollbarPolicy = PolicyType.Automatic
                    };

                    var panel = new VBox();

                    // Header
                    _header = new EventBox();
                    panel.Add(_header);

                    // List
                    _treeView = new TreeView();
                    _treeView.RulesHint = true;
                    _treeView.HeadersVisible = false;
                    panel.Add(_treeView);

                    _treeView.Selection.Changed += OnSelectionChanged;

                    // Footer
                    _footer = new EventBox();
                    panel.Add(_footer);

                    Viewport viewPort = new Viewport();
                    viewPort.ShadowType = ShadowType.None;
                    viewPort.Add(panel);

                    scroller.Add(viewPort);

                    SetNativeControl(scroller);
                }

                UpdateItems();
                UpdateHeader();
                UpdateFooter();
                UpdateRowHeight();
                UpdateSeparatorColor();
                UpdateSeparatorVisibility();
                UpdateIsRefreshing();
                UpdatePullToRefreshEnabled();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ListView.ItemsSourceProperty.PropertyName)
                UpdateItems();
            else if (e.PropertyName.Equals("HeaderElement", System.StringComparison.InvariantCultureIgnoreCase))
                UpdateHeader();
            else if (e.PropertyName.Equals("FooterElement", System.StringComparison.InvariantCultureIgnoreCase))
                UpdateFooter();
            else if (e.PropertyName == ListView.RowHeightProperty.PropertyName)
                UpdateRowHeight();
            else if (e.PropertyName == ListView.SeparatorColorProperty.PropertyName)
                UpdateSeparatorColor();
            else if (e.PropertyName == ListView.SeparatorVisibilityProperty.PropertyName)
                UpdateSeparatorVisibility();
            else if (e.PropertyName == ListView.IsRefreshingProperty.PropertyName)
                UpdateIsRefreshing();
            else if (e.PropertyName == ListView.IsPullToRefreshEnabledProperty.PropertyName)
                UpdatePullToRefreshEnabled();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && !_disposed)
            {
                if (_treeView != null)
                {
                    _treeView.Selection.Changed -= OnSelectionChanged;
                }

                _disposed = true;
            }
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return base.GetDesiredSize(widthConstraint, heightConstraint);
        }

        protected override void UpdateBackgroundColor()
        {
            base.UpdateBackgroundColor();

            if (_treeView == null)
            {
                return;
            }

            if (Element.BackgroundColor.IsDefault)
            {
                return;
            }

            var backgroundColor = Element.BackgroundColor.ToGtkColor();

            foreach (var column in _treeView.Columns)
            {
                foreach (var cell in column.Cells)
                {
                    cell.CellBackgroundGdk = backgroundColor;
                }
            }
        }

        private void UpdateItems()
        {
            var items = TemplatedItemsView.TemplatedItems;

            if (!items.Any())
            {
                return;
            }

            ListStore listStore = null;

            // Create a column 
            TreeViewColumn column = new TreeViewColumn();
            _treeView.AppendColumn(column);

            var element = items.FirstOrDefault();

            if (element == null)
            {
                return;
            }

            List<object> cells = new List<object>();

            foreach (var item in items)
            {
                var renderer =
                    (Cells.CellRenderer)Internals.Registrar.Registered.GetHandler<IRegisterable>(item.GetType());
                var nativeCell = renderer.GetCell(item, null, _treeView);
                cells.Add(nativeCell);
            }

            switch (element.GetType().Name)
            {
                case "ImageCell":
                    var gtkImageCell = new GtkImageCell();

                    column.PackStart(gtkImageCell, true);
                    column.AddAttribute(gtkImageCell, "image", 0);
                    column.AddAttribute(gtkImageCell, "text", 1);
                    column.AddAttribute(gtkImageCell, "detail", 2);

                    listStore = new ListStore(typeof(Pixbuf), typeof(string), typeof(string));

                    foreach (var cell in cells)
                    {
                        var typedCell = (GtkImageCell)cell;
                        listStore.AppendValues(typedCell.Image, typedCell.Text, typedCell.Detail);
                    }
                    break;
                case "TextCell":
                    var gtkTextCell = new GtkTextCell();

                    column.PackStart(gtkTextCell, true);
                    column.AddAttribute(gtkTextCell, "text", 0);
                    column.AddAttribute(gtkTextCell, "detail", 1);

                    listStore = new ListStore(typeof(string), typeof(string));

                    foreach (var cell in cells)
                    {
                        var typedCell = (GtkTextCell)cell;
                        listStore.AppendValues(typedCell.Text, typedCell.Detail);
                    }
                    break;
                default:
                    // Create Cell
                    var defaultCell = new GtkTextCell();

                    // Add the cell to the column	
                    column.PackStart(defaultCell, true);

                    // Tell the Cell Renderers which items in the model to display
                    column.AddAttribute(defaultCell, "text", 0);
                    column.AddAttribute(defaultCell, "detail", 1);

                    listStore = new ListStore(typeof(string), typeof(string));

                    // Add data to the store
                    foreach (var cell in cells)
                    {
                        var typedCell = (GtkTextCell)cell;
                        listStore.AppendValues(typedCell.Text, typedCell.Detail);
                    }
                    break;
            }

            // Assign the model to the TreeView
            _treeView.Model = listStore;
        }

        private void UpdateHeader()
        {
            var header = Controller.HeaderElement;
            var headerView = (View)header;

            if (headerView != null)
            {
                _headerRenderer = Platform.CreateRenderer(headerView);
                Platform.SetRenderer(headerView, _headerRenderer);

                _header.Add(_headerRenderer.Container);
            }
            else
            {
                _header.HeightRequest = 0;
            }
        }

        private void UpdateFooter()
        {
            var footer = Controller.FooterElement;
            var footerView = (View)footer;

            if (footerView != null)
            {
                _footerRenderer = Platform.CreateRenderer(footerView);
                Platform.SetRenderer(footerView, _footerRenderer);

                _footer.Add(_footerRenderer.Container);
            }
            else
            {
                _footer.HeightRequest = 0;
            }
        }

        private void UpdateRowHeight()
        {
            var rowHeight = Element.RowHeight;

            var column = _treeView.Columns.FirstOrDefault();

            if (column != null)
            {
                var cell = column.Cells.FirstOrDefault();

                if (cell != null)
                {
                    cell.Height = rowHeight > 0 ? rowHeight : DefaultRowHeight;
                }
            }
        }

        //TODO: Implement SeparatorColor
        private void UpdateSeparatorColor()
        {

        }

        private void UpdateSeparatorVisibility()
        {
            if (Element.SeparatorVisibility == SeparatorVisibility.Default)
                _treeView.EnableGridLines = TreeViewGridLines.Horizontal;
            else
                _treeView.EnableGridLines = TreeViewGridLines.None;
        }

        //TODO: Implement UpdateIsRefreshing
        private void UpdateIsRefreshing()
        {

        }

        //TODO: Implement PullToRefresh
        private void UpdatePullToRefreshEnabled()
        {

        }

        private void OnSelectionChanged(object sender, System.EventArgs e)
        {
            TreeIter selectedIter;
            if (_treeView.Selection.GetSelected(out selectedIter))
            {
                var selected = _treeView.Model.GetValue(selectedIter, 0);
                var items = TemplatedItemsView.TemplatedItems;

                if (!items.Any())
                {
                    return;
                }

                if (selected != null)
                {
                    var itemsSource = items.ItemsSource.Cast<object>().ToList();
                    object selectedItemsSource = null;

                    foreach (var item in itemsSource)
                    {
                        var properties = item.GetType().GetProperties();

                        foreach (var property in properties)
                        {
                            var value = property.GetValue(item)?.ToString();
                            if (value != null && value.Equals(selected.ToString()))
                            {
                                selectedItemsSource = item;
                                break;
                            }
                        }
                    }

                    if (selectedItemsSource != null)
                    {
                        ((IElementController)Element).SetValueFromRenderer(ListView.SelectedItemProperty, selectedItemsSource);
                    }
                }
            }
        }
    }
}