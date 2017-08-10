﻿using GLib;
using System;
using Xamarin.Forms.Maps.GTK;
using Xamarin.Forms.Platform.GTK;

namespace Xamarin.Forms.ControlGallery.GTK
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ExceptionManager.UnhandledException += OnUnhandledException;

            Gtk.Application.Init();
            Forms.Init();
            FormsMaps.Init(string.Empty);

            var app = new Controls.App();
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("Xamarin.Forms GTK# Backend");
            window.SetApplicationIcon("xamarinlogo.png");
            window.Show();
            Gtk.Application.Run();
        }

        private static void OnUnhandledException(UnhandledExceptionArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"Unhandled GTK# exception: {args.ExceptionObject}");
        }
    }
}