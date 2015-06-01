using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Threading;
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace System.Wpf.Adorners
{
    public class CueBannerAdorner : Adorner, IDisposable
    {
        private string _cueBanner = string.Empty;
        private TextBox _textBox;
        private ComboBox _comboBox;
        private DependencyPropertyDescriptor _textDescriptor;
        private Brush _textBrush;
        private FormattedText _formattedText;

        public CueBannerAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.IsHitTestVisible = false;
            this.Focusable = false;
            _textBrush = new SolidColorBrush(Colors.Black);
            _textBrush.Opacity = 0.5;
            Control ctl = this.AdornedElement as Control;
            _formattedText = new FormattedText(_cueBanner, Thread.CurrentThread.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(ctl.FontFamily, ctl.FontStyle, ctl.FontWeight, ctl.FontStretch), ctl.FontSize, _textBrush);
            this.Initialize();
        }
        
        public CueBannerAdorner(UIElement adornedElement, string cueBanner) : this(adornedElement)
        {
            this.CueBanner = cueBanner;
        }
        
        private void Initialize()
        {
            Control ctl = (Control)this.AdornedElement;
            string text = string.Empty;
            if (ctl is TextBox)
            {
                _textBox = ctl as TextBox;
                text = _textBox.Text;
                _textBox.AddHandler(TextBoxBase.TextChangedEvent, new RoutedEventHandler(this.OnTextChanged));
            }
            else if (ctl is ComboBox)
            {
                _comboBox = ctl as ComboBox;
                text = _comboBox.Text;
                _textDescriptor = DependencyPropertyDescriptor.FromProperty(ComboBox.TextProperty, typeof(ComboBox));
                if (_textDescriptor != null)
                {
                    _textDescriptor.AddValueChanged(_comboBox, this.OnTextChanged);
                }
            }
            else if (ctl is DatePicker)
            {
                DatePicker picker = this.AdornedElement as DatePicker;
                _textBox = picker.Template.FindName("PART_TextBox", picker) as TextBox;
                ContentControl watermark = _textBox.Template.FindName("PART_Watermark", _textBox) as ContentControl;
                if (watermark != null)
                    watermark.Content = null;
                text = _textBox.Text;
                _textBox.AddHandler(TextBoxBase.TextChangedEvent, new RoutedEventHandler(this.OnTextChanged));
            }
        }

        #region Event Handling

        private void OnTextChanged(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }
    
        #endregion

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
            string text = string.Empty;
            if (_textBox != null)
            {
                text = _textBox.Text;
            }
            else if (_comboBox != null)
            {
                text = _comboBox.Text;
            }
            else
                return;
            if (string.IsNullOrEmpty(text))
            {
                drawingContext.DrawText(_formattedText, new Point(4, 3));
            }
        }

        public string CueBanner
        {
            get
            {
                return _cueBanner;
            }
            set
            {
                _cueBanner = value; 
                _formattedText = new FormattedText(_cueBanner, Thread.CurrentThread.CurrentCulture, FlowDirection.LeftToRight,
                    new Typeface(((Control)this.AdornedElement).FontFamily, ((Control)this.AdornedElement).FontStyle, ((Control)this.AdornedElement).FontWeight, ((Control)this.AdornedElement).FontStretch), ((Control)this.AdornedElement).FontSize, _textBrush);
                this.InvalidateVisual();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_textDescriptor != null)
            {
                _textDescriptor.RemoveValueChanged(_comboBox, this.OnTextChanged);
                _textDescriptor = null;
            }
            if (_textBox != null)
            {
                _textBox.RemoveHandler(TextBoxBase.TextChangedEvent, new RoutedEventHandler(this.OnTextChanged));
            }
            _textBox = null;
            _comboBox = null;
            _textBrush = null;
            _formattedText = null;
        }
        
        #endregion
    }
}