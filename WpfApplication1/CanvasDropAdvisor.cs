using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using Windows.Foundation;

namespace WpfApplication1
{
    public class CanvasDropAdvisor : IDropTargetAdvisor, IDragSourceAdvisor
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
            get{ return !(_sourceTargetElt is Canvas); }
        }

        public DragDropEffects SupportedEffects
        {
            get { return DragDropEffects.Move; }
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

        #region IDropTargetAdvisor

        public UIElement TargetUI
        {
            get { return _sourceTargetElt; }
            set { _sourceTargetElt = value; }
        }

        public bool ApplyMouseOffset
        {
            get { return true; }
        }

        public bool IsValidDataObject(IDataObject obj)
        {
            return (obj.GetDataPresent("CanvasExample"));
        }

        public UIElement GetVisualFeedback(IDataObject obj)
        {
            UIElement elt = ExtractElement(obj);
            Type t = elt.GetType();
            Rectangle rect = new Rectangle();
            rect.Width = (double)t.GetProperty("Width").GetValue(elt, null);
            rect.Height = (double)t.GetProperty("Height").GetValue(elt, null);
            rect.Fill = new VisualBrush(elt);
            rect.Opacity = 0.5;
            rect.IsHitTestVisible = false;
            return rect;
        }

        public void OnDropCompleted(IDataObject obj, Point dropPoint)
        {
            Canvas canvas = _sourceTargetElt as Canvas;
            UIElement elt = ExtractElement(obj);
            canvas.Children.Add(elt);
            Canvas.SetLeft(elt, dropPoint.X);
            Canvas.SetTop(elt, dropPoint.Y);
        }

        #endregion

        private UIElement ExtractElement(IDataObject obj)
        {
            string xamlString = obj.GetData("CanvasExample") as string;
            XmlReader reader = XmlReader.Create(new StringReader(xamlString));
            UIElement elt = XamlReader.Load(reader) as UIElement;
            return elt;
        }
    }
}
