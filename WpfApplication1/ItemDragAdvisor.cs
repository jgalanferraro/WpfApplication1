using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace WpfApplication1
{
    public class ItemDragAdvisor : IDragSourceAdvisor
    {
        private UIElement _sourceTargetElt;

        #region IDragSourceAdvisor

        public UIElement SourceUI
        {
            get { return _sourceTargetElt; }
            set { _sourceTargetElt = value; }
        }

        public bool IsDraggable
        {
            get { return !(_sourceTargetElt is Canvas); }
        }

        public DragDropEffects SupportedEffects
        {
            get { return DragDropEffects.Copy|DragDropEffects.Move; }
        }

        public void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
        {
            if ((finalEffects & DragDropEffects.Move) == DragDropEffects.Move)
            {
                (_sourceTargetElt as Canvas).Children.Remove(draggedElt);
            }
        }

        public DataObject GetDataObject()
        {
            string serializedElt = XamlWriter.Save(_sourceTargetElt);
            DataObject obj = new DataObject("CanvasExample", serializedElt);
            return obj;
        }

        #endregion

    }
}
