﻿using Gdk;
using Gtk;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class TabbedPageHeader : HBox
    {
        private Gtk.Label _label;
        private Gtk.Image _image;

        public TabbedPageHeader(string title, Pixbuf icon = null)
        {
            Spacing = 0;

            _image = new Gtk.Image();
            _image.Pixbuf = icon;
            Add(_image);

            _label = new Gtk.Label();
            _label.Text = title;
            Add(_label);

            ShowAll();
        }

        public Gtk.Label Label => _label;

        public Gtk.Image Icon => _image;
    }
}