using System;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Wpf.Adorners.Generic;

namespace System.Wpf.Adorners.Generic
{
    public class GenericAdorners : DependencyObject
    {
        #region Private Variables
        
        private static Dictionary<FrameworkElement, GenericAdorner> _genericAdorners;
        
        #endregion
        
        #region Dependency Properties
        
        public static readonly DependencyProperty VisibleProperty;
        public static readonly DependencyProperty AdornerElementProperty;
        public static readonly DependencyProperty VerticalAlignmentProperty;
        public static readonly DependencyProperty HorizontalAlignmentProperty;
        public static readonly DependencyProperty PlacementProperty;
        public static readonly DependencyProperty MarginProperty;
        public static readonly DependencyProperty VisibleOnFocusProperty;
        
        #endregion
        
        #region Constructors
        
        static GenericAdorners()
        {
            VisibleProperty = DependencyProperty.RegisterAttached(
                "Visible", typeof(bool), typeof(GenericAdorners),
                new PropertyMetadata(false, OnVisibleChanged));
            AdornerElementProperty = DependencyProperty.RegisterAttached(
                "AdornerElement", typeof(UIElement), typeof(GenericAdorners),
                new PropertyMetadata(null, OnAdornerElementChanged));
            VerticalAlignmentProperty = DependencyProperty.RegisterAttached(
                "VerticalAlignment", typeof(AdornerVerticalAlignment), typeof(GenericAdorners),
                new PropertyMetadata(AdornerVerticalAlignment.Top, OnVerticalAlignmentChanged));
            HorizontalAlignmentProperty = DependencyProperty.RegisterAttached(
                "HorizontalAlignment", typeof(AdornerHorizontalAlignment), typeof(GenericAdorners),
                new PropertyMetadata(AdornerHorizontalAlignment.Left, OnHorizontalAlignmentChanged));
            PlacementProperty = DependencyProperty.RegisterAttached(
                "Placement", typeof(AdornerPlacement), typeof(GenericAdorners),
                new PropertyMetadata(AdornerPlacement.VerticalOuterHorizontalOuter, OnPlacementChanged));
            MarginProperty = DependencyProperty.RegisterAttached(
                "Margin", typeof(Thickness), typeof(GenericAdorners),
                new PropertyMetadata(new Thickness(), OnMarginChanged));
            VisibleOnFocusProperty = DependencyProperty.RegisterAttached(
                "VisibleOnFocus", typeof(bool), typeof(GenericAdorners),
                new PropertyMetadata(false, OnVisibleOnFocusChanged));
            _genericAdorners = new Dictionary<FrameworkElement, GenericAdorner>();
        }
        
        #endregion
        
        #region Private Functions
        
        static void AttachGenericAdorner(FrameworkElement element, bool visible)
        {
            GenericAdorner adorner = new GenericAdorner(
                element,
                GetAdornerElement(element),
                visible,
                GetVerticalAlignment(element),
                GetHorizontalAlignment(element),
                GetPlacement(element),
                GetMargin(element),
                GetVisibleOnFocus(element));
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);
            layer.Add(adorner);
            _genericAdorners.Add(element, adorner);
        }
        
        #endregion
        
        #region Visible
        
        public static bool GetVisible(UIElement element)
        {
            return (bool)element.GetValue(VisibleProperty);
        }
        
        public static void SetVisible(UIElement element, bool value)
        {
            element.SetValue(VisibleProperty, value);
        }
        
        static void OnVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null)
                return;
            bool visible = (bool)e.NewValue;
            if (_genericAdorners.ContainsKey(element))
            {
                _genericAdorners[element].Visible = visible;
            }
        }
        
        #endregion
        
        #region AdornerElement
        
        public static UIElement GetAdornerElement(UIElement element)
        {
            return (UIElement)element.GetValue(AdornerElementProperty);
        }
        
        public static void SetAdornerElement(UIElement element, UIElement value)
        {
            element.SetValue(AdornerElementProperty, value);
        }
        
        static void OnAdornerElementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null)
                return;
            if (_genericAdorners.ContainsKey(element))
            {
                _genericAdorners[element].AdornerElement = (UIElement)e.NewValue;
            }
            else
            {
                if (!element.IsLoaded)
                    element.Loaded += (s, ee) => AttachGenericAdorner(element, GetVisible(element));
                else
                    AttachGenericAdorner(element, GetVisible(element));
            }
        }
        
        #endregion
        
        #region VerticalAlignment
        
        public static AdornerVerticalAlignment GetVerticalAlignment(UIElement element)
        {
            return (AdornerVerticalAlignment)element.GetValue(VerticalAlignmentProperty);
        }
        
        public static void SetVerticalAlignment(UIElement element, AdornerVerticalAlignment value)
        {
            element.SetValue(VerticalAlignmentProperty, value);
        }
        
        static void OnVerticalAlignmentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null)
                return;
            if (_genericAdorners.ContainsKey(element))
            {
                _genericAdorners[element].VerticalAdornerAlignment = (AdornerVerticalAlignment)e.NewValue;
            }
        }
        
        #endregion
        
        #region HorizontalAlignment
        
        public static AdornerHorizontalAlignment GetHorizontalAlignment(UIElement element)
        {
            return (AdornerHorizontalAlignment)element.GetValue(HorizontalAlignmentProperty);
        }
        
        public static void SetHorizontalAlignment(UIElement element, AdornerHorizontalAlignment value)
        {
            element.SetValue(HorizontalAlignmentProperty, value);
        }
        
        static void OnHorizontalAlignmentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null)
                return;
            if (_genericAdorners.ContainsKey(element))
            {
                _genericAdorners[element].HorizontalAdornerAlignment = (AdornerHorizontalAlignment)e.NewValue;
            }
        }
        
        #endregion
        
        #region Placement
        
        public static AdornerPlacement GetPlacement(UIElement element)
        {
            return (AdornerPlacement)element.GetValue(PlacementProperty);
        }
        
        public static void SetPlacement(UIElement element, AdornerPlacement value)
        {
            element.SetValue(PlacementProperty, value);
        }
        
        static void OnPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null)
                return;
            if (_genericAdorners.ContainsKey(element))
            {
                _genericAdorners[element].AdornerPlacement = (AdornerPlacement)e.NewValue;
            }
        }
        
        #endregion
        
        #region Margin
        
        public static Thickness GetMargin(UIElement element)
        {
            return (Thickness)element.GetValue(MarginProperty);
        }
        
        public static void SetMargin(UIElement element, Thickness value)
        {
            element.SetValue(MarginProperty, value);
        }
        
        static void OnMarginChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null)
                return;
            if (_genericAdorners.ContainsKey(element))
            {
                _genericAdorners[element].AdornerMargin = (Thickness)e.NewValue;
            }
        }
        
        #endregion
        
        #region VisibleOnFocus
        
        public static bool GetVisibleOnFocus(UIElement element)
        {
            return (bool)element.GetValue(VisibleOnFocusProperty);
        }
        
        public static void SetVisibleOnFocus(UIElement element, bool value)
        {
            element.SetValue(VisibleOnFocusProperty, value);
        }
        
        static void OnVisibleOnFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null)
                return;
            if (_genericAdorners.ContainsKey(element))
            {
                _genericAdorners[element].VisibleOnFocus = (bool)e.NewValue;
            }
        }
    
        #endregion
    }
}