using Nhafo.Code.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nhafo.WPF.Controls {
    public class DraggableUserControl : UserControl {
        public virtual bool Draggable { get; set; }
        public bool Dragging { get; protected set; }
        
        protected object LastSender { get; private set; }

        protected Panel ParentPanel => Parent as Panel;

        protected void SubscribeElement(UIElement element) {
            element.MouseDown += __MouseDown;
            element.MouseUp += __MouseUp;
        }

        protected void UnsubscribeElement(UIElement element) {
            element.MouseDown -= __MouseDown;
            element.MouseUp -= __MouseUp;
        }

        private void __MouseDown(object sender, MouseButtonEventArgs e) {
            if(e.ChangedButton != MouseButton.Left || !Draggable)
                return;

            Dragging = true;
            LastSender = sender;
            Mouse.AddMouseMoveHandler(MainWindow.Instance, __MouseMove);
            (Parent as UIElement).MouseUp += __MouseUp;
            OnDragStart();
        }

        private void __MouseUp(object sender, MouseButtonEventArgs e) {
            if(e.ChangedButton != MouseButton.Left)
                return;

            if(Dragging) 
                DragStop();
        }

        private void __MouseMove(object sender, MouseEventArgs e) {
            if(!Draggable || !Dragging || e.LeftButton != MouseButtonState.Pressed) {
                DragStop();
                return;
            }

            OnDragMove(e);
        }

        private void DragStop() {
            Mouse.RemoveMouseMoveHandler(MainWindow.Instance, __MouseMove);
            (Parent as UIElement).MouseUp -= __MouseUp;
            Dragging = false;
            OnDragStop();
        }

        protected virtual void OnDragStart() { }
        protected virtual void OnDragMove(MouseEventArgs e) { }
        protected virtual void OnDragStop() { }

        protected void SetPosition(Point position) {
            SetPosition(this, position);
        }
        protected void SetPosition(UIElement element, Point position) {
            if(ParentPanel is CartesianPlane cp)
                CartesianPlane.SetLocation(element, position);
            else if(ParentPanel is Canvas) {
                Canvas.SetLeft(element, position.X);
                Canvas.SetTop(element, position.Y);
            }
        }

        protected Point GetRelativePosition(Point position) {
            if(ParentPanel is CartesianPlane cp)
                return position.Sum(cp.Origin);
            return position;
        }

        public void BringToFront() {
            int aux;
            int max = int.MinValue;
            int count = 0;
            UIElement top = null;
            foreach(UIElement elem in ParentPanel.Children) {
                if((aux = Panel.GetZIndex(elem)) > max) {
                    max = aux;
                    top = elem;
                    count = 1;
                }
                else if(aux == max)
                    count++;
            }

            if(top != null) {
                aux = Panel.GetZIndex(this);
                Panel.SetZIndex(top, aux);
                
                Panel.SetZIndex(this, count > 1 ? max + 1 : max);
            }
        }
    }
}