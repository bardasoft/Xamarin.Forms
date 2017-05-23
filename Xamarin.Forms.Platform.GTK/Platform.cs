﻿using Gtk;
using System;
using Xamarin.Forms.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Xamarin.Forms.Platform.GTK
{
    public class Platform : BindableObject, IPlatform, INavigation, IDisposable
    {
        private bool _disposed;
        readonly List<Page> _modals;
        private readonly PlatformRenderer _renderer;

        internal static readonly BindableProperty RendererProperty =
            BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer), 
                typeof(Platform), default(IVisualElementRenderer),
            propertyChanged: (bindable, oldvalue, newvalue) =>
            {
                var view = bindable as VisualElement;
                if (view != null)
                    view.IsPlatformEnabled = newvalue != null;
            });

        internal PlatformRenderer PlatformRenderer => _renderer;

        internal static NativeToolbarTracker NativeToolbarTracker = new NativeToolbarTracker();

        Page Page { get; set; }

        IReadOnlyList<Page> INavigation.ModalStack
        {
            get { return _modals; }
        }

        IReadOnlyList<Page> INavigation.NavigationStack
        {
            get { return new List<Page>(); }
        }

        internal Platform()
        {
            _renderer = new PlatformRenderer(this);
            _modals = new List<Page>();
            Application.Current.NavigationProxy.Inner = this;

            MessagingCenter.Subscribe(this, Page.AlertSignalName, (Page sender, AlertArguments arguments) =>
            {
                MessageDialog messageDialog = new MessageDialog(
                    PlatformRenderer.Toplevel as Window,
                    DialogFlags.DestroyWithParent,
                    MessageType.Other,
                    ButtonsType.Ok,
                    arguments.Message);

                messageDialog.Title = arguments.Title;

                ResponseType result = (ResponseType)messageDialog.Run();

                if(result == ResponseType.Ok)
                {
                    messageDialog.Destroy();
                    arguments.SetResult(true);
                }

                arguments.SetResult(false);
            });

            MessagingCenter.Subscribe(this, Page.ActionSheetSignalName, (Page sender, ActionSheetArguments arguments) =>
            {
                MessageDialog messageDialog = new MessageDialog(
                   PlatformRenderer.Toplevel as Window,
                   DialogFlags.DestroyWithParent,
                   MessageType.Other,
                   ButtonsType.Ok,
                   arguments.Title);

                ResponseType result = (ResponseType)messageDialog.Run();

                if (result == ResponseType.Ok)
                {
                    messageDialog.Destroy();
                    arguments.SetResult(string.Empty);
                }

                arguments.SetResult(string.Empty);
            });
        }

        internal static void DisposeModelAndChildrenRenderers(Element view)
        {
            IVisualElementRenderer renderer;

            foreach (VisualElement child in view.Descendants())
                DisposeModelAndChildrenRenderers(child);

            renderer = GetRenderer((VisualElement)view);

            renderer?.Dispose();

            view.ClearValue(RendererProperty);
        }

        SizeRequest IPlatform.GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            var renderView = GetRenderer(view);

            if (renderView == null || renderView.Container == null)
                return new SizeRequest(Size.Zero);

            return renderView.GetDesiredSize(widthConstraint, heightConstraint);
        }

        public static IVisualElementRenderer GetRenderer(VisualElement element)
        {
            return (IVisualElementRenderer)element.GetValue(RendererProperty);
        }

        public static void SetRenderer(VisualElement element, IVisualElementRenderer value)
        {
            if (element != null)
            {
                element.SetValue(RendererProperty, value);
                element.IsPlatformEnabled = value != null;
            }
        }

        public static IVisualElementRenderer CreateRenderer(VisualElement element)
        {
            var elementType = element.GetType();
            var renderer = Registrar.Registered.GetHandler<IVisualElementRenderer>(elementType) ?? new DefaultRenderer();
            renderer.SetElement(element);

            return renderer;
        }

        void IDisposable.Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            MessagingCenter.Unsubscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName);
            MessagingCenter.Unsubscribe<Page, AlertArguments>(this, Page.AlertSignalName);
            MessagingCenter.Unsubscribe<Page, bool>(this, Page.BusySetSignalName);

            PlatformRenderer.Dispose();
        }

        internal void SetPage(Page newRoot)
        {
            if (newRoot == null)
                return;

            if (Page != null)
                throw new NotImplementedException();

            Page = newRoot;
            Page.Platform = this;
            AddChild(Page);

            Application.Current.NavigationProxy.Inner = this;
        }

        private void AddChild(Page mainPage)
        {
            var viewRenderer = GetRenderer(mainPage);

            if (viewRenderer == null)
            {
                viewRenderer = CreateRenderer(mainPage);
                SetRenderer(mainPage, viewRenderer);

                PlatformRenderer.Add(viewRenderer.Container);
                PlatformRenderer.ShowAll();
            }
        }
        void INavigation.InsertPageBefore(Page page, Page before)
        {
            throw new InvalidOperationException("InsertPageBefore is not supported globally on GTK, please use a NavigationPage.");
        }

        Task<Page> INavigation.PopAsync()
        {
            return ((INavigation)this).PopAsync(true);
        }

        Task<Page> INavigation.PopAsync(bool animated)
        {
            throw new InvalidOperationException("PopAsync is not supported globally on GTK, please use a NavigationPage.");
        }

        Task<Page> INavigation.PopModalAsync()
        {
            return ((INavigation)this).PopModalAsync(true);
        }

        Task<Page> INavigation.PopModalAsync(bool animated)
        {
            var modal = _modals.Last();
            _modals.Remove(modal);

            var modalPage = GetRenderer(modal) as Container;

            // TODO:

            DisposeModelAndChildrenRenderers(modal);

            return Task.FromResult<Page>(null);
        }

        Task INavigation.PopToRootAsync()
        {
            return ((INavigation)this).PopToRootAsync(true);
        }

        Task INavigation.PopToRootAsync(bool animated)
        {
            throw new InvalidOperationException("PopToRootAsync is not supported globally on GTK, please use a NavigationPage.");
        }

        Task INavigation.PushAsync(Page root)
        {
            return ((INavigation)this).PushAsync(root, true);
        }

        Task INavigation.PushAsync(Page root, bool animated)
        {
            throw new InvalidOperationException("PushAsync is not supported globally on GTK, please use a NavigationPage.");
        }

        Task INavigation.PushModalAsync(Page modal)
        {
            return ((INavigation)this).PushModalAsync(modal, true);
        }

        Task INavigation.PushModalAsync(Page modal, bool animated)
        {
            _modals.Add(modal);
            modal.Platform = this;

            // TODO:

            return Task.FromResult<object>(null);
        }

        void INavigation.RemovePage(Page page)
        {
            throw new InvalidOperationException("RemovePage is not supported globally on GTK, please use a NavigationPage.");
        }

        internal class DefaultRenderer : VisualElementRenderer<VisualElement, Widget>
        {

        }
    }
}