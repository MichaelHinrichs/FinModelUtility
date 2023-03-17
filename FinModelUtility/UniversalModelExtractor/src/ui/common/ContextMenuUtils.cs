using fin.util.asserts;

namespace uni.ui.common {
  public static class ContextMenuUtils {
    public static EventHandler CreateRightClickEventHandler(
        Func<IEnumerable<(string, Action)>> itemGenerator)
      => (sender, e) => {
        var mouseEventArgs = (MouseEventArgs) e;
        if (mouseEventArgs.Button != MouseButtons.Right) {
          return;
        }

        var items = itemGenerator().ToArray();
        if (items.Length == 0) {
          return;
        }

        var contextMenu = new ContextMenuStrip();
        foreach (var (itemText, itemHandler) in items) {
          contextMenu.Items.Add(itemText, null, (s, e) => itemHandler());
        }

        contextMenu.Show(Asserts.AsA<Control>(sender), mouseEventArgs.Location);
      };
  }
}