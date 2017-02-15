﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private static UIElement _droppingItem;

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
                //Set the Drag source UI
                IDragSourceAdvisor advisor = args.NewValue as IDragSourceAdvisor;
                advisor.SourceUI = sourceElt;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                sourceElt.PreviewMouseLeftButtonDown -= DragSource_PreviewMouseLeftButtonDown;
                sourceElt.PreviewMouseMove -= DragSource_PreviewMouseMove;
                sourceElt.PreviewMouseLeftButtonUp -= DragSource_PreviewMouseLeftButtonUp;
            }
        }

        private static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IDragSourceAdvisor dragSourceAdvisor = GetUseDragSourceAdvisor(sender as DependencyObject);
        }

        private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            IDragSourceAdvisor dragSourceAdvisor = GetUseDragSourceAdvisor(sender as DependencyObject);

            if (dragSourceAdvisor != null && dragSourceAdvisor.IsDraggable)
            {
                IDataObject data = dragSourceAdvisor.GetDataObject();
                DragDrop.DoDragDrop(sender as DependencyObject, data, DragDropEffects.Move);
            }
        }

        private static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IDragSourceAdvisor dragSourceAdvisor = GetUseDragSourceAdvisor(sender as DependencyObject);

            if (dragSourceAdvisor != null)
            {
                if (dragSourceAdvisor.IsDraggable)
                {
                    _droppingItem = dragSourceAdvisor.SourceUI;
                }
            }
        }

        #endregion

        #region OnDropTargetAdvisorChanged
        private static void OnDropTargetAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            UIElement targetElt = depObj as UIElement;
            if (args.NewValue != null && args.OldValue == null)
            {
                targetElt.PreviewDragEnter += DropTarget_PreviewDragEnter;
                targetElt.PreviewDragOver += DropTarget_PreviewDragOver;
                targetElt.PreviewDragLeave += DropTarget_PreviewDragLeave;
                targetElt.PreviewDrop += DropTarget_PreviewDrop;
                targetElt.AllowDrop = true;
                // Set the Drag source UI
                IDropTargetAdvisor advisor = args.NewValue as IDropTargetAdvisor;
                advisor.TargetUI = targetElt;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                targetElt.PreviewDragEnter -= DropTarget_PreviewDragEnter;
                targetElt.PreviewDragOver -= DropTarget_PreviewDragOver;
                targetElt.PreviewDragLeave -= DropTarget_PreviewDragLeave;
                targetElt.PreviewDrop -= DropTarget_PreviewDrop;
                targetElt.AllowDrop = false;
            }
        }

        private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            IDropTargetAdvisor dragSource = GetUseDropTargetAdvisor(sender as DependencyObject);
        }

        private static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            IDropTargetAdvisor dragSource = GetUseDropTargetAdvisor(sender as DependencyObject);
        }

        private static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            IDropTargetAdvisor dragSource = GetUseDropTargetAdvisor(sender as DependencyObject);
        }

        private static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            IDropTargetAdvisor dragSource = GetUseDropTargetAdvisor(sender as DependencyObject);
        }

        #endregion
    }
}
