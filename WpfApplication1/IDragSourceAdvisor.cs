using System.Windows;

namespace WpfApplication1
{
    public interface IDragSourceAdvisor
    {
        UIElement SourceUI { get; set; }
        bool IsDraggable { get; }
        DragDropEffects SupportedEffects { get; }
        DataObject GetDataObject();
        void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects);
    }
}