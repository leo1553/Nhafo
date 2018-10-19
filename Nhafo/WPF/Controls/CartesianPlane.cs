using Nhafo.Code.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Nhafo.WPF.Controls {
    public class CartesianPlane : Panel {
        public static readonly DependencyProperty LocationProperty = DependencyProperty.RegisterAttached("Location", typeof(Point), typeof(CartesianPlane),
                    new PropertyMetadata(PointUtils.Zero, new PropertyChangedCallback(OnPositioningChanged)));
        /*public static readonly DependencyProperty HideIfOutOfBoundsProperty = DependencyProperty.RegisterAttached("HideIfOutOfBounds", typeof(bool), typeof(CartesianPlane),
                    new PropertyMetadata(true));*/
        public static readonly DependencyProperty OriginProperty = DependencyProperty.Register("Origin", typeof(Point), typeof(CartesianPlane),
                    new PropertyMetadata(PointUtils.Zero, new PropertyChangedCallback(OnOriginChanged)));
        public static readonly DependencyProperty DraggableProperty = DependencyProperty.Register("Draggable", typeof(bool), typeof(CartesianPlane),
            new PropertyMetadata(false, new PropertyChangedCallback(OnDraggableChanged)));

        public Point Origin {
            get => (Point)GetValue(OriginProperty);
            set => SetValue(OriginProperty, value);
        }

        public bool Draggable {
            get => (bool)GetValue(DraggableProperty);
            set => SetValue(DraggableProperty, value);
        }

        public bool Dragging { get; private set; }

        private Point dragStartOrigin;
        private Point dragStartPoint;

        private static void OnPositioningChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(d is UIElement uie && VisualTreeHelper.GetParent(uie) is CartesianPlane p) 
                p.InvalidateArrange();
        }

        private static void OnOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(d is CartesianPlane cc)
                cc.InvalidateArrange();
        }

        private static void OnDraggableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(d is CartesianPlane cc) {
                bool oldValue = (bool)e.OldValue;
                bool value = (bool)e.NewValue;

                if(oldValue == value)
                    return;

                if(oldValue == true) {
                    cc.MouseDown -= cc._MouseDown;
                    cc.MouseUp -= cc._MouseUp;
                }
                if(value == true) {
                    cc.MouseDown += cc._MouseDown;
                    cc.MouseUp += cc._MouseUp;
                }
                else
                    cc.Dragging = false;
            }
        }

        [AttachedPropertyBrowsableForChildren()]
        public static Point GetLocation(UIElement element) {
            if(element == null) { throw new ArgumentNullException("element"); }
            return (Point)element.GetValue(LocationProperty);
        }

        public static void SetLocation(UIElement element, Point location) {
            if(element == null) { throw new ArgumentNullException("element"); }
            element.SetValue(LocationProperty, location);
        }

        /*[AttachedPropertyBrowsableForChildren()]
        public static bool GetHideIfOutOfBounds(UIElement element) {
            if(element == null) { throw new ArgumentNullException("element"); }
            return (bool)element.GetValue(HideIfOutOfBoundsProperty);
        }

        public static void SetHideIfOutOfBounds(UIElement element, bool hide) {
            if(element == null) { throw new ArgumentNullException("element"); }
            element.SetValue(HideIfOutOfBoundsProperty, hide);
        }*/

        public CartesianPlane() : base() {
            Background = Brushes.Transparent;
            IsHitTestVisible = true;
            ClipToBounds = true;
        }

        protected override Size MeasureOverride(Size constraint) {
            Size childConstraint = new Size(Double.PositiveInfinity, Double.PositiveInfinity);

            foreach(UIElement child in InternalChildren) {
                if(child == null) { continue; }
                child.Measure(childConstraint);
            }

            return new Size();
        }

        protected override Size ArrangeOverride(Size arrangeSize) {
            Rect rect = new Rect(Origin, new Size(ActualWidth, ActualHeight));
            Point origin = Origin;
            Point location;
            //ChildBounds bounds;
            foreach(UIElement child in InternalChildren) {
                if(child == null)
                    continue;
                
                location = GetLocation(child);
                /*if(GetHideIfOutOfBounds(child)) {
                    bounds = new ChildBounds(child, location);
                    if(!bounds.VisibleFor(rect)) {
                        child.Visibility = Visibility.Hidden;
                        continue;
                    }
                    else
                        child.Visibility = Visibility.Visible;
                }*/
                
                child.Arrange(new Rect(location.Sub(origin), child.DesiredSize));
            }
            return arrangeSize;
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize) {
            if(ClipToBounds)
                return new RectangleGeometry(new Rect(RenderSize));
            else
                return null;
        }

        // Drag

        private void _MouseDown(object sender, MouseButtonEventArgs e) {
            if(e.ChangedButton != MouseButton.Left || !Draggable)
                return;

            HitTestResult htr = VisualTreeHelper.HitTest(this, e.GetPosition((UIElement)sender));
            if(htr != null)
                if(htr.VisualHit != this)
                    return;

            dragStartOrigin = Origin;
            dragStartPoint = e.MouseDevice.GetPosition(this);
            Dragging = true;
            Mouse.AddMouseMoveHandler(MainWindow.Instance, _MouseMove);
            (Parent as UIElement).MouseUp += _MouseUp;
        }

        private void _MouseUp(object sender, MouseButtonEventArgs e) {
            if(e.ChangedButton != MouseButton.Left)
                return;

            if(Dragging)
                DragStop();
        }

        private void _MouseMove(object sender, MouseEventArgs e) {
            if(!Draggable || !Dragging || e.LeftButton != MouseButtonState.Pressed) {
                DragStop();
                return;
            }

            Point mousePosition = e.MouseDevice.GetPosition(this);
            Point delta = mousePosition.Sub(dragStartPoint);
            Origin = dragStartOrigin.Sub(delta);
        }

        private void DragStop() {
            Mouse.RemoveMouseMoveHandler(MainWindow.Instance, _MouseMove);
            (Parent as UIElement).MouseUp -= _MouseUp;
            Dragging = false;
        }

        /*private struct ChildBounds {
            public Point topLeft;
            public Point topRight;
            public Point bottomLeft;
            public Point bottomRight;

            public ChildBounds(UIElement uie, Point location) {
                topLeft = location;
                topRight = 
                    new Point(
                        location.X + uie.DesiredSize.Width,
                        location.Y);
                bottomLeft = 
                    new Point(
                        location.X,
                        location.Y + uie.DesiredSize.Height);
                bottomRight = location.Sum(uie.DesiredSize);
            }

            public bool VisibleFor(Rect rect) {
                return rect.Contains(bottomRight) || rect.Contains(bottomLeft) || rect.Contains(topRight) || rect.Contains(topLeft);
            }
        }*/
    }
}
