using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace System.Wpf.Adorners
{
    public class AdornerContentPresenter : Adorner
    {
        private VisualCollection _visuals;
        private ContentPresenter _contentPresenter;

        public AdornerContentPresenter(UIElement adornedElement) : base(adornedElement)
        {
            _visuals = new VisualCollection(this);
            _contentPresenter = new ContentPresenter();
            _visuals.Add(_contentPresenter);
        }

        public AdornerContentPresenter(UIElement adornedElement, Visual content) : this(adornedElement)
        {
            Content = content;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _contentPresenter.Measure(constraint);
            return _contentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _contentPresenter.Arrange(new Rect(0, 0,
                finalSize.Width, finalSize.Height));
            return _contentPresenter.RenderSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }

        public object Content
        {
            get
            {
                return _contentPresenter.Content;
            }
            set
            {
                _contentPresenter.Content = value;
            }
        }
    }
}