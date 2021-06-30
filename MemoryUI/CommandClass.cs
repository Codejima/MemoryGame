using System.Windows.Input;

namespace MemoryUI
{
    static class CommandClass
    {
        public static readonly RoutedUICommand ChangeTheme = new("Changes Dark Theme","ChangeTheme",typeof(CommandClass),new InputGestureCollection() { new KeyGesture(Key.U, ModifierKeys.Control) } );
    }
}
