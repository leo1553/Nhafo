using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Nhafo.WPF.Controls {
    public class ResizeThumb : Thumb {
        public static DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(ResizeThumbDirection), typeof(ResizeThumb),
            new PropertyMetadata(ResizeThumbDirection.Unset, new PropertyChangedCallback(OnDirectionChanged)));
        public static DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(double), typeof(ResizeThumb),
            new PropertyMetadata(0D));

        private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(d is Thumb thumb) {
                ResizeThumbDirection direction = (ResizeThumbDirection)e.NewValue;
                switch(direction) {
                    case ResizeThumbDirection.Top:
                    case ResizeThumbDirection.Bottom:
                        thumb.Cursor = Cursors.SizeNS;
                        break;
                    case ResizeThumbDirection.Left:
                    case ResizeThumbDirection.Right:
                        thumb.Cursor = Cursors.SizeWE;
                        break;

                    case ResizeThumbDirection.TopLeft:
                    case ResizeThumbDirection.BottomRight:
                        thumb.Cursor = Cursors.SizeNWSE;
                        break;
                    case ResizeThumbDirection.TopRight:
                    case ResizeThumbDirection.BottomLeft:
                        thumb.Cursor = Cursors.SizeNESW;
                        break;
                }
            }
        }

        public ResizeThumbDirection Direction {
            get => (ResizeThumbDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public double CornerRadius {
            get => (double)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public ResizeThumb() {
            DragDelta += _DragDelta;
        }

        private void _DragDelta(object sender, DragDeltaEventArgs e) {
            if(DataContext is Control control) {
                switch(Direction) {
                    case ResizeThumbDirection.Top:
                        ResizeTop(control, e);
                        break;
                    case ResizeThumbDirection.Bottom:
                        ResizeBottom(control, e);
                        break;
                    case ResizeThumbDirection.Left:
                        ResizeLeft(control, e);
                        break;
                    case ResizeThumbDirection.Right:
                        ResizeRight(control, e);
                        break;

                    case ResizeThumbDirection.TopLeft:
                        ResizeTop(control, e);
                        ResizeLeft(control, e);
                        break;
                    case ResizeThumbDirection.TopRight:
                        ResizeTop(control, e);
                        ResizeRight(control, e);
                        break;
                    case ResizeThumbDirection.BottomLeft:
                        ResizeBottom(control, e);
                        ResizeLeft(control, e);
                        break;
                    case ResizeThumbDirection.BottomRight:
                        ResizeBottom(control, e);
                        ResizeRight(control, e);
                        break;

                    default:
                        break;
                }
            }
            e.Handled = true;
        }

        private static void ResizeTop(Control control, DragDeltaEventArgs e) {
            double delta = Math.Min(e.VerticalChange, control.ActualHeight - control.MinHeight);
            if(control.Parent is Canvas canvas)
                Canvas.SetTop(control, Canvas.GetTop(control) + delta);
            else if(control.Parent is CartesianPlane plane) {
                Point location = CartesianPlane.GetLocation(control);
                CartesianPlane.SetLocation(control, new Point(location.X, location.Y + delta));
            }
            control.Height -= delta;
        }

        private static void ResizeBottom(Control control, DragDeltaEventArgs e) {
            double delta = Math.Min(-e.VerticalChange, control.ActualHeight - control.MinHeight);
            control.Height -= delta;
        }

        private static void ResizeLeft(Control control, DragDeltaEventArgs e) {
            double delta = Math.Min(e.HorizontalChange, control.ActualWidth - control.MinWidth);
            if(control.Parent is Canvas canvas)
                Canvas.SetLeft(control, Canvas.GetLeft(control) + delta);
            else if(control.Parent is CartesianPlane plane) {
                Point location = CartesianPlane.GetLocation(control);
                CartesianPlane.SetLocation(control, new Point(location.X + delta, location.Y));
            }
            control.Width -= delta;
        }

        private static void ResizeRight(Control control, DragDeltaEventArgs e) {
            double delta = Math.Min(-e.HorizontalChange, control.ActualWidth - control.MinWidth);
            control.Width -= delta;
        }
    }
    
    public enum ResizeThumbDirection {
        Unset = 0,
        Top,
        Bottom,
        Left,
        Right,

        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
