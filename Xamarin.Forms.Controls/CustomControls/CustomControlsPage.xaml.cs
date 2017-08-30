﻿using Xamarin.Forms;

namespace Xamarin.Forms.Controls.CustomControls
{
    public partial class CustomControlsPage : ContentPage
    {
        public CustomControlsPage()
        {
            InitializeComponent();

            CalendarBtn.Clicked += (sender, args) => Navigation.PushAsync(new CalendarPage());
            ColorButtonBtn.Clicked += (sender, args) => Navigation.PushAsync(new ColorButtonPage());
            ColorPickerBtn.Clicked += (sender, args) => Navigation.PushAsync(new ColorPickerPage());
            ChartsBtn.Clicked += (sender, args) => Navigation.PushAsync(new ChartPage());
            DataGridBtn.Clicked += (sender, args) => Navigation.PushAsync(new DataGridPage());
            DateTimePickerBtn.Clicked += (sender, args) => Navigation.PushAsync(new DateTimePickerPage());
            ExpanderBtn.Clicked += (sender, args) => Navigation.PushAsync(new ExpanderPage());
            FileButtonBtn.Clicked += (sender, args) => Navigation.PushAsync(new FileButtonPage());
            FontButtonBtn.Clicked += (sender, args) => Navigation.PushAsync(new FontButtonPage());
            GridSplitterBtn.Clicked += (sender, args) => Navigation.PushAsync(new GridSplitterPage());
            HyperLinkBtn.Clicked += (sender, args) => Navigation.PushAsync(new HyperLinkPage());
            ImageCheckBoxBtn.Clicked += (sender, args) => Navigation.PushAsync(new ImageCheckBoxPage());
            ScaleButtonBtn.Clicked += (sender, args) => Navigation.PushAsync(new ScaleButtonPage());
            SeparatorBtn.Clicked += (sender, args) => Navigation.PushAsync(new SeparatorPage());
            StatusBarBtn.Clicked += (sender, args) => Navigation.PushAsync(new StatusBarPage());
            TextEditorBtn.Clicked += (sender, args) => Navigation.PushAsync(new TextEditorPage());
            ToggleButtonBtn.Clicked += (sender, args) => Navigation.PushAsync(new ToggleButtonPage());
            TreeViewBtn.Clicked += (sender, args) => Navigation.PushAsync(new TreeViewPage());
        }
    }
}