using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication1
{
    public class DragDropManager
    {
        public static readonly DependencyProperty DragSourceAdvisorProperty = DependencyProperty.RegisterAttached("DragSourceAdvisor",
                                                                                                                typeof(IDragSourceAdvisor),
                                                                                                                typeof(DragDropManager),
                                                                                                                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDragSourceAdvisorChanged)));

        public static readonly DependencyProperty DropTargetAdvisorProperty = DependencyProperty.RegisterAttached("DropTargetAdvisor",
                                                                                                                    typeof(IDropTargetAdvisor),
                                                                                                                    typeof(DragDropManager),
                                                                                                                    new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDropTargetAdvisorChanged)));

        static IDropTargetAdvisor _dropAdvisor;
        static DragAdorner _adorner = null;
        static AdornerLayer _layer;

        public static void SetUseDragSourceAdvisor(DependencyObject d, IDragSourceAdvisor dragSource)
        {
            d.SetValue(DragSourceAdvisorProperty, dragSource);
        }

        public static void SetUseDropTargetAdvisor(DependencyObject d, IDropTargetAdvisor dropSource)
        {
            d.SetValue(DropTargetAdvisorProperty, dropSource);
        }

        public static IDragSourceAdvisor GetUseDragSourceAdvisor(DependencyObject d)
        {
           return d.GetValue(DragSourceAdvisorProperty) as IDragSourceAdvisor;
        }

        public static IDropTargetAdvisor GetUseDropTargetAdvisor(DependencyObject d)
        {
           return d.GetValue(DropTargetAdvisorProperty) as IDropTargetAdvisor;
        }

        #region OnDragSourceAdvisorChanged
        private static void OnDragSourceAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            UIElement sourceElt = depObj as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                sourceElt.PreviewMouseLeftButtonDown += DragSource_PreviewMouseLeftButtonDown;
                sourceElt.PreviewMouseMove += DragSource_PreviewMouseMove;
                sourceElt.PreviewMouseLeftButtonUp += DragSource_PreviewMouseLeftButtonUp;
                sourceElt.GiveFeedback += DragSource_PreviewGiveFeedback;
                //Set the Drag source UI
                IDragSourceAdvisor _dragAdvisor = args.NewValue as IDragSourceAdvisor;
                _dragAdvisor.SourceUI = sourceElt;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                sourceElt.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
                sourceElt.PreviewMouseMove -= DragSource_PreviewMouseMove;
                sourceElt.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
                sourceElt.GiveFeedback -= DragSource_PreviewGiveFeedback;
            }
        }

        private static void DragSource_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            e.Handled = true;
        }

        private static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IDragSourceAdvisor dragSourceAdvisor = GetUseDragSourceAdvisor(sender as DependencyObject);
        }

        //If mouse move over the to drag item, then active move action.
        private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var dragAdvisor = GetUseDragSourceAdvisor(sender as DependencyObject);

                IDataObject data = dragAdvisor.GetDataObject();

                if (_dropAdvisor.IsValidDataObject(data))
                {
                    UIElement dropElement = _dropAdvisor.GetVisualFeedback(data);
                    _adorner = new DragAdorner(_dropAdvisor.TargetUI, dropElement);
                    _layer = AdornerLayer.GetAdornerLayer(_dropAdvisor.TargetUI as Visual);
                    _layer.Add(_adorner);
                }

                DragDrop.DoDragDrop(dragAdvisor.SourceUI, data, dragAdvisor.SupportedEffects);
            }
        }

        //If selected item is draggable made it the to drag item
        private static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dragAdvisor = GetUseDragSourceAdvisor(sender as DependencyObject);
        }

        #endregion

        #region OnDropTargetAdvisorChanged
        private static void OnDropTargetAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            UIElement targetElt = depObj as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                targetElt.PreviewDragEnter += DropTarget_PreviewDragEnter;
                targetElt.DragOver += DropTarget_PreviewDragOver;
                targetElt.PreviewDragLeave += DropTarget_PreviewDragLeave;
                targetElt.PreviewDrop += DropTarget_PreviewDrop;
                targetElt.AllowDrop = true;
                // Set the Drag source UI
                _dropAdvisor = args.NewValue as IDropTargetAdvisor;
                _dropAdvisor.TargetUI = targetElt;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                targetElt.PreviewDragEnter -= DropTarget_PreviewDragEnter;
                targetElt.DragOver -= DropTarget_PreviewDragOver;
                targetElt.PreviewDragLeave -= DropTarget_PreviewDragLeave;
                targetElt.PreviewDrop -= DropTarget_PreviewDrop;
                targetElt.AllowDrop = false;
            }
        }

        private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            var dropAdvisor = GetUseDropTargetAdvisor(sender as DependencyObject);

            var data = e.Data;
            if (dropAdvisor.IsValidDataObject(data))
            {
                Point position = e.GetPosition(dropAdvisor.TargetUI);
                dropAdvisor.OnDropCompleted(data, position);
            }
        }

        private static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
           
        }

        private static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            var dropAdvisor = GetUseDropTargetAdvisor(sender as DependencyObject);

            var data = e.Data;
            if (dropAdvisor.IsValidDataObject(data))
            {
                _adorner.LeftOffset = e.GetPosition(dropAdvisor.TargetUI).X /* - _startPoint.X */ ;
                _adorner.TopOffset = e.GetPosition(dropAdvisor.TargetUI).Y /* - _startPoint.Y */ ;
            }
        }

        private static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            //var dropAdvisor = GetUseDropTargetAdvisor(sender as DependencyObject);

            //var data = e.Data;
            //if (dropAdvisor.IsValidDataObject(data))
            //{
            //    UIElement dropElement = dropAdvisor.GetVisualFeedback(data);        
            //    DragAdorner _adorner = new DragAdorner(dropAdvisor.TargetUI, dropElement);
            //    AdornerLayer _layer = AdornerLayer.GetAdornerLayer(dropAdvisor.TargetUI as Visual);
            //    _layer.Add(_adorner);

                
            //}
        }

        #endregion
    }
}
