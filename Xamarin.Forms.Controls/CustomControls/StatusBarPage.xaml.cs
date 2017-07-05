﻿using Xamarin.Forms;

namespace Xamarin.Forms.Controls.CustomControls
{
    public partial class StatusBarPage : ContentPage
    {
        private int _counter;

        public StatusBarPage()
        {
            InitializeComponent();

            PushBtn.Clicked += (sender, args) =>
            {
                StatusBar.Push(string.Format("Message {0}", _counter + 1));
                _counter++;
            };

            PopBtn.Clicked += (sender, args) =>
            {
                StatusBar.Pop();
            };
        }
    }
}