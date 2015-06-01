using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Wpf.Adorners.Generic;

namespace System.Wpf.Adorners.Generic
{
    public class GenericAdorner : Adorner, IDisposable
    {
        #region Private Variables
        
        protected bool _visible;
        protected FrameworkElement _adornedElement;
        protected FrameworkElement _adornerElement;
        protected FrameworkElement _focusElement;
        protected AdornerVerticalAlignment _verticalAdornerAlignment;
        protected AdornerHorizontalAlignment _horizontalAdornerAlignment;
        protected AdornerPlacement _adornerPlacement;
        protected Thickness _adornerMargin;
        protected bool _visibleOnFocus;
        protected bool _focusHandlerAdded;
        protected Size _adornerSize;
        
        #endregion
        
        #region Constructors
        
        public GenericAdorner(
            UIElement adornedElement,
            UIElement adornerElement,
            bool visible,
            AdornerVerticalAlignment verticalAlignment,
            AdornerHorizontalAlignment horizontalAlignment,
            AdornerPlacement placement,
            Thickness adornerMargin,
            bool visibleOnFocus) : base(adornedElement)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            if (adornerElement == null)
                throw new ArgumentNullException("adornerElement");
            if (!(adornedElement is FrameworkElement))
                throw new ArgumentException("adornedElement must be a FrameworkElement");
            if (!(adornerElement is FrameworkElement))
                throw new ArgumentException("adornerElement must be a FrameworkElement");
            
            _adornedElement = adornedElement as FrameworkElement;
            _adornerElement = adornerElement as FrameworkElement;
            
            if (!_adornedElement.IsLoaded)
                throw new ArgumentException("adornedElement not loaded. Create the adorner at the Loaded event.");
            
            _adornerElement.DataContext = _adornedElement;
            _visible = visible;
            _verticalAdornerAlignment = verticalAlignment;
            _horizontalAdornerAlignment = horizontalAlignment;
            _adornerPlacement = placement;
            _adornerMargin = adornerMargin;
            _visibleOnFocus = visibleOnFocus;
            
            this.InitializeAdorner();
        }
        
        #endregion
        
        public void SetAutoSizing()
        {
            if (this.AdornedElement is FrameworkElement)
                ((FrameworkElement)this.AdornedElement).SizeChanged += this.OnAdornedElementSizeChanged;
        }
        
        private void OnAdornedElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var element = this.AdornerElement as FrameworkElement;
            if (element != null)
            {
                _adornerSize = this.AdornedElement.RenderSize;
                element.Height = this.AdornedElement.RenderSize.Height;
                element.Width = this.AdornedElement.RenderSize.Width;
            }
        }
        
        #region Public Properties
        
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
                this.ToggleFocusHandler();
                this.InvalidateVisual();
            }
        }
        
        public UIElement AdornerElement
        {
            get
            {
                return _adornerElement;
            }
            set
            {
                _adornerElement = value as FrameworkElement;
                this.InvalidateVisual();
            }
        }
        
        public AdornerVerticalAlignment VerticalAdornerAlignment
        {
            get
            {
                return _verticalAdornerAlignment;
            }
            set
            {
                _verticalAdornerAlignment = value;
                this.InvalidateVisual();
            }
        }
        
        public AdornerHorizontalAlignment HorizontalAdornerAlignment
        {
            get
            {
                return _horizontalAdornerAlignment;
            }
            set
            {
                _horizontalAdornerAlignment = value;
                this.InvalidateVisual();
            }
        }
        
        public AdornerPlacement AdornerPlacement
        {
            get
            {
                return _adornerPlacement;
            }
            set
            {
                _adornerPlacement = value;
                this.InvalidateVisual();
            }
        }
        
        public Thickness AdornerMargin
        {
            get
            {
                return _adornerMargin;
            }
            set
            {
                _adornerMargin = value;
                this.InvalidateVisual();
            }
        }
        
        public bool VisibleOnFocus
        {
            get
            {
                return _visibleOnFocus;
            }
            set
            {
                _visibleOnFocus = value;
                this.ToggleFocusHandler();
                this.InvalidateVisual();
            }
        }
        
        #endregion
        
        #region Protected Functions
        
        protected virtual void InitializeAdorner()
        {
            _adornerElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            _adornerSize = _adornerElement.DesiredSize;
            _adornerElement.Height = _adornerSize.Height;
            _adornerElement.Width = _adornerSize.Width;
            
            this.SetPropertyRecursive(_adornerElement, element => element.Focusable = false);
            
            this.AddVisualChild(_adornerElement);
            this.ToggleFocusHandler();
        }
        
        protected void SetPropertyRecursive(UIElement element, Action<UIElement> setter)
        {
            if (element != null && element is UIElement)
            {
                setter((UIElement)element);
            }
            
            foreach (object subnode in LogicalTreeHelper.GetChildren(element))
            {
                if (subnode is UIElement)
                {
                    UIElement subElement = subnode as UIElement;
                    this.SetPropertyRecursive(subElement, setter);
                }
            }
        }
        
        protected virtual void ToggleFocusHandler()
        {
            _focusElement = GetFocusElement(_adornedElement);
            if (_visibleOnFocus && !_focusHandlerAdded)
            {
                _focusElement.AddHandler(FrameworkElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus));
                _focusElement.AddHandler(FrameworkElement.LostFocusEvent, new RoutedEventHandler(OnLostFocus));
                _focusHandlerAdded = true;
            }
            else if (!_visibleOnFocus && _focusHandlerAdded)
            {
                _focusElement.RemoveHandler(FrameworkElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus));
                _focusElement.RemoveHandler(FrameworkElement.LostFocusEvent, new RoutedEventHandler(OnLostFocus));
                _focusHandlerAdded = false;
            }
            if (_visibleOnFocus == false && _visible == false)
                this.Visibility = Visibility.Collapsed;
            else if (_visibleOnFocus == false && _visible == true)
                this.Visibility = Visibility.Visible;
            else
                this.Visibility = (_adornedElement.IsFocused) ? Visibility.Visible : ((_visibleOnFocus) ? Visibility.Collapsed : Visibility.Visible);
        }
        
        protected virtual void OnGotFocus(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }
        
        protected virtual void OnLostFocus(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        
        protected virtual FrameworkElement GetFocusElement(FrameworkElement element)
        {
            if (element is ComboBox)
            {
                ComboBox comboBox = element as ComboBox;
                if (comboBox.Template != null)
                    return comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
                else
                    System.Diagnostics.Debug.WriteLine("Can't find ComboBox editor!");
            }
            else if (element is DatePicker)
            {
                DatePicker picker = element as DatePicker;
                if (picker.Template != null)
                    return picker.Template.FindName("PART_TextBox", picker) as TextBox;
                else
                    System.Diagnostics.Debug.WriteLine("Can't find DatePicker editor!");
            }
            return element;
        }
        
        #endregion
        
        #region Overrides
        
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_adornerElement != null)
            {
                // Set vertical alignment
                var pos = new Point();
                switch (_verticalAdornerAlignment)
                {
                    case AdornerVerticalAlignment.Top:
                        if (_adornerPlacement == AdornerPlacement.VerticalOuterHorizontalInner || _adornerPlacement == AdornerPlacement.VerticalOuterHorizontalOuter)
                            pos.Y -= _adornerSize.Height;
                        else
                            pos.Y = 0;
                        break;
                    case AdornerVerticalAlignment.Center:
                        pos.Y = (_adornedElement.ActualHeight - _adornerSize.Height) / 2;
                        break;
                    case AdornerVerticalAlignment.Bottom:
                        if (_adornerPlacement == AdornerPlacement.VerticalOuterHorizontalInner || _adornerPlacement == AdornerPlacement.VerticalOuterHorizontalOuter)
                            pos.Y = _adornedElement.ActualHeight;
                        else
                            pos.Y = _adornedElement.ActualHeight - _adornerSize.Height;
                        break;
                }
                
                // Set horizontal alignment
                switch (_horizontalAdornerAlignment)
                {
                    case AdornerHorizontalAlignment.Left:
                        if (_adornerPlacement == AdornerPlacement.VerticalInnerHorizontalOuter || _adornerPlacement == AdornerPlacement.VerticalOuterHorizontalOuter)
                            pos.X -= _adornerSize.Width;
                        else
                            pos.X = 0;
                        break;
                    case AdornerHorizontalAlignment.Center:
                        pos.X = (_adornedElement.ActualWidth - _adornerSize.Width) / 2;
                        break;
                    case AdornerHorizontalAlignment.Right:
                        if (_adornerPlacement == AdornerPlacement.VerticalInnerHorizontalOuter || _adornerPlacement == AdornerPlacement.VerticalOuterHorizontalOuter)
                            pos.X = _adornedElement.ActualWidth;
                        else
                            pos.X = _adornedElement.ActualWidth - _adornerSize.Width;
                        break;
                }
                
                // Set Margin
                pos.Y += _adornerMargin.Top;
                pos.Y -= _adornerMargin.Bottom;
                pos.X += _adornerMargin.Left;
                pos.X -= _adornerMargin.Right;
                
                // Arange element
                _adornerElement.Arrange(new Rect(pos, _adornerSize));
            }
            return finalSize;
        }
        
        protected override int VisualChildrenCount
        {
            get
            {
                return _adornerElement == null ? 0 : 1;
            }
        }
        
        protected override Visual GetVisualChild(int index)
        {
            if (index == 0 && _adornerElement != null)
            {
                return _adornerElement;
            }
            return base.GetVisualChild(index);
        }
        
        #endregion
        
        #region IDisposable Members
        
        public void Dispose()
        {
            if (this.AdornedElement != null && this.AdornedElement is FrameworkElement)
                ((FrameworkElement)this.AdornedElement).SizeChanged -= this.OnAdornedElementSizeChanged;
            
            if (_focusElement != null)
            {
                _focusElement.RemoveHandler(FrameworkElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus));
                _focusElement.RemoveHandler(FrameworkElement.LostFocusEvent, new RoutedEventHandler(OnLostFocus));
            }
            this.RemoveVisualChild(_adornerElement);
        }
    
        #endregion
    }
}