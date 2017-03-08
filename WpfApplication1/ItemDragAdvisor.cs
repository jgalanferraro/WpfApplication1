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
        private UIElement _sourceElt;

        #region IDragSourceAdvisor

        public UIElement SourceUI
        {
            get { return _sourceElt; }
            set { _sourceElt = value; }
        }

        public bool IsDraggable
        {
            get { return true; }
        }

        public DragDropEffects SupportedEffects
        {
            get { return DragDropEffects.Move; }
        }

        public void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
        {
            if ((finalEffects & DragDropEffects.Move) == DragDropEffects.Move)
            {
                (_sourceElt as Canvas).Children.Remove(draggedElt);
            }
        }

        public DataObject GetDataObject()
        {
            string serializedElt = XamlWriter.Save(_sourceElt);
            DataObject obj = new DataObject("DraggedItem", serializedElt);
            return obj;
        }

        #endregion

    }
}
