using System.Windows;

namespace WpfApplication1
{
    public interface IDropTargetAdvisor
    {
        UIElement TargetUI { get; set; }
        bool ApplyMouseOffset { get; }
        bool IsValidDataObject(IDataObject obj);
        void OnDropCompleted(IDataObject obj, Point dropPoint);
        UIElement GetVisualFeedback(IDataObject obj);
        UIElement GetTopContainer();
    }
}