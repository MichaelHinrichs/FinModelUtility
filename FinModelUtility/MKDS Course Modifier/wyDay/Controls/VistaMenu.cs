// Decompiled with JetBrains decompiler
// Type: wyDay.Controls.VistaMenu
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace wyDay.Controls
{
  [ProvideProperty("Image", typeof (MenuItem))]
  public class VistaMenu : Component, IExtenderProvider, ISupportInitialize
  {
    private readonly Hashtable properties = new Hashtable();
    private readonly Hashtable menuParents = new Hashtable();
    private readonly MENUINFO mnuInfo = new MENUINFO();
    private const int SEPARATOR_HEIGHT = 9;
    private const int BORDER_VERTICAL = 4;
    private const int LEFT_MARGIN = 4;
    private const int RIGHT_MARGIN = 6;
    private const int SHORTCUT_MARGIN = 20;
    private const int ARROW_MARGIN = 12;
    private const int ICON_SIZE = 16;
    private Container components;
    private bool formHasBeenIntialized;
    private readonly bool isVistaOrLater;
    private ContainerControl ownerForm;
    private bool isUsingKeyboardAccel;
    private static Font menuBoldFont;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool SetMenuItemInfo(
      HandleRef hMenu,
      int uItem,
      bool fByPosition,
      MENUITEMINFO_T_RW lpmii);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool SetMenuInfo(HandleRef hMenu, MENUINFO lpcmi);

    [DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    public VistaMenu()
    {
      this.isVistaOrLater = Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6;
      this.InitializeComponent();
    }

    public VistaMenu(IContainer container)
      : this()
    {
      container.Add((IComponent) this);
    }

    private void InitializeComponent()
    {
      this.components = new Container();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        foreach (DictionaryEntry property in this.properties)
        {
          if (((Properties) property.Value).renderBmpHbitmap != IntPtr.Zero)
            VistaMenu.DeleteObject(((Properties) property.Value).renderBmpHbitmap);
        }
        if (this.components != null)
          this.components.Dispose();
      }
      base.Dispose(disposing);
    }

    bool IExtenderProvider.CanExtend(object o)
    {
      switch (o)
      {
        case MenuItem _:
          return ((MenuItem) o).Parent == null || ((MenuItem) o).Parent.GetType() != typeof (MainMenu);
        case Form _:
          return true;
        default:
          return false;
      }
    }

    private Properties EnsurePropertiesExists(MenuItem key)
    {
      Properties properties = (Properties) this.properties[(object) key];
      if (properties == null)
      {
        properties = new Properties();
        this.properties[(object) key] = (object) properties;
      }
      return properties;
    }

    [Description("The Image for the MenuItem")]
    [DefaultValue(null)]
    [Category("Appearance")]
    public Image GetImage(MenuItem mnuItem)
    {
      return this.EnsurePropertiesExists(mnuItem).Image;
    }

    [DefaultValue(null)]
    public void SetImage(MenuItem mnuItem, Image value)
    {
      Properties properties = this.EnsurePropertiesExists(mnuItem);
      properties.Image = value;
      if (!this.DesignMode && this.isVistaOrLater)
      {
        if (properties.renderBmpHbitmap != IntPtr.Zero)
        {
          VistaMenu.DeleteObject(properties.renderBmpHbitmap);
          properties.renderBmpHbitmap = IntPtr.Zero;
        }
        if (value == null)
          return;
        using (Bitmap bitmap = new Bitmap(value.Width, value.Height, PixelFormat.Format32bppPArgb))
        {
          using (Graphics graphics = Graphics.FromImage((Image) bitmap))
            graphics.DrawImage(value, 0, 0, value.Width, value.Height);
          properties.renderBmpHbitmap = bitmap.GetHbitmap(Color.FromArgb(0, 0, 0, 0));
        }
        if (this.formHasBeenIntialized)
          this.AddVistaMenuItem(mnuItem);
      }
      if (this.DesignMode || this.isVistaOrLater || !this.formHasBeenIntialized)
        return;
      this.AddPreVistaMenuItem(mnuItem);
    }

    void ISupportInitialize.BeginInit()
    {
    }

    private void AddVistaMenuItem(MenuItem mnuItem)
    {
      if (this.menuParents[(object) mnuItem.Parent] != null)
        return;
      if (mnuItem.Parent.GetType() == typeof (ContextMenu))
        ((ContextMenu) mnuItem.Parent).Popup += new EventHandler(this.MenuItem_Popup);
      else
        ((MenuItem) mnuItem.Parent).Popup += new EventHandler(this.MenuItem_Popup);
      VistaMenu.SetMenuInfo(new HandleRef((object) null, mnuItem.Parent.Handle), this.mnuInfo);
      this.menuParents[(object) mnuItem.Parent] = (object) true;
    }

    private void AddPreVistaMenuItem(MenuItem mnuItem)
    {
      if (this.menuParents[(object) mnuItem.Parent] != null)
        return;
      this.menuParents[(object) mnuItem.Parent] = (object) true;
      if (this.formHasBeenIntialized)
      {
        foreach (MenuItem menuItem in mnuItem.Parent.MenuItems)
        {
          menuItem.DrawItem += new DrawItemEventHandler(this.MenuItem_DrawItem);
          menuItem.MeasureItem += new MeasureItemEventHandler(VistaMenu.MenuItem_MeasureItem);
          menuItem.OwnerDraw = true;
        }
      }
    }

    void ISupportInitialize.EndInit()
    {
      if (this.DesignMode)
        return;
      if (this.isVistaOrLater)
      {
        foreach (DictionaryEntry property in this.properties)
          this.AddVistaMenuItem((MenuItem) property.Key);
      }
      else
      {
        VistaMenu.menuBoldFont = new Font(SystemFonts.MenuFont, FontStyle.Bold);
        if (this.ownerForm != null)
          this.ownerForm.ChangeUICues += new UICuesEventHandler(this.ownerForm_ChangeUICues);
        foreach (DictionaryEntry property in this.properties)
          this.AddPreVistaMenuItem((MenuItem) property.Key);
        foreach (DictionaryEntry menuParent in this.menuParents)
        {
          foreach (MenuItem menuItem in ((Menu) menuParent.Key).MenuItems)
          {
            menuItem.DrawItem += new DrawItemEventHandler(this.MenuItem_DrawItem);
            menuItem.MeasureItem += new MeasureItemEventHandler(VistaMenu.MenuItem_MeasureItem);
            menuItem.OwnerDraw = true;
          }
        }
      }
      this.formHasBeenIntialized = true;
    }

    private void MenuItem_Popup(object sender, EventArgs e)
    {
      MENUITEMINFO_T_RW lpmii = new MENUITEMINFO_T_RW();
      Menu.MenuItemCollection menuItemCollection = sender.GetType() == typeof (ContextMenu) ? ((Menu) sender).MenuItems : ((Menu) sender).MenuItems;
      int uItem = 0;
      for (int index = 0; index < menuItemCollection.Count; ++index)
      {
        if (menuItemCollection[index].Visible)
        {
          Properties property = (Properties) this.properties[(object) menuItemCollection[index]];
          if (property != null)
          {
            lpmii.hbmpItem = property.renderBmpHbitmap;
            VistaMenu.SetMenuItemInfo(new HandleRef((object) null, ((Menu) sender).Handle), uItem, true, lpmii);
          }
          ++uItem;
        }
      }
    }

    public VistaMenu(ContainerControl parentControl)
      : this()
    {
      this.ownerForm = parentControl;
    }

    public ContainerControl ContainerControl
    {
      get
      {
        return this.ownerForm;
      }
      set
      {
        this.ownerForm = value;
      }
    }

    public override ISite Site
    {
      set
      {
        base.Site = value;
        if (value == null || !(value.GetService(typeof (IDesignerHost)) is IDesignerHost service))
          return;
        this.ContainerControl = service.RootComponent as ContainerControl;
      }
    }

    private void ownerForm_ChangeUICues(object sender, UICuesEventArgs e)
    {
      this.isUsingKeyboardAccel = e.ShowKeyboard;
    }

    private static void MenuItem_MeasureItem(object sender, MeasureItemEventArgs e)
    {
      Font font = ((MenuItem) sender).DefaultItem ? VistaMenu.menuBoldFont : SystemFonts.MenuFont;
      if (((MenuItem) sender).Text == "-")
      {
        e.ItemHeight = 9;
      }
      else
      {
        e.ItemHeight = (SystemFonts.MenuFont.Height > 16 ? SystemFonts.MenuFont.Height : 16) + 4;
        e.ItemWidth = 26 + TextRenderer.MeasureText(((MenuItem) sender).Text, font, Size.Empty, TextFormatFlags.NoClipping | TextFormatFlags.SingleLine).Width + 20 + TextRenderer.MeasureText(VistaMenu.ShortcutToString(((MenuItem) sender).Shortcut), font, Size.Empty, TextFormatFlags.NoClipping | TextFormatFlags.SingleLine).Width + (((Menu) sender).IsParent ? 12 : 0);
      }
    }

    private void MenuItem_DrawItem(object sender, DrawItemEventArgs e)
    {
      e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
      e.Graphics.InterpolationMode = InterpolationMode.Low;
      bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
      if (isSelected)
        e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
      else
        e.Graphics.FillRectangle(SystemBrushes.Menu, e.Bounds);
      if (((MenuItem) sender).Text == "-")
      {
        Rectangle bounds = e.Bounds;
        int top = bounds.Top;
        bounds = e.Bounds;
        int num1 = bounds.Height / 2;
        int num2 = top + num1 - 1;
        Graphics graphics1 = e.Graphics;
        Pen controlDark = SystemPens.ControlDark;
        bounds = e.Bounds;
        int x1_1 = bounds.Left + 1;
        int y1_1 = num2;
        bounds = e.Bounds;
        int left1 = bounds.Left;
        bounds = e.Bounds;
        int width1 = bounds.Width;
        int x2_1 = left1 + width1 - 2;
        int y2_1 = num2;
        graphics1.DrawLine(controlDark, x1_1, y1_1, x2_1, y2_1);
        Graphics graphics2 = e.Graphics;
        Pen controlLightLight = SystemPens.ControlLightLight;
        bounds = e.Bounds;
        int x1_2 = bounds.Left + 1;
        int y1_2 = num2 + 1;
        bounds = e.Bounds;
        int left2 = bounds.Left;
        bounds = e.Bounds;
        int width2 = bounds.Width;
        int x2_2 = left2 + width2 - 2;
        int y2_2 = num2 + 1;
        graphics2.DrawLine(controlLightLight, x1_2, y1_2, x2_2, y2_2);
      }
      else
      {
        this.DrawText(sender, e, isSelected);
        if (((MenuItem) sender).Checked)
        {
          if (((MenuItem) sender).RadioCheck)
          {
            Graphics graphics = e.Graphics;
            int x = e.Bounds.Left + (26 - SystemInformation.MenuCheckSize.Width) / 2;
            int top = e.Bounds.Top;
            int height1 = e.Bounds.Height;
            Size menuCheckSize = SystemInformation.MenuCheckSize;
            int height2 = menuCheckSize.Height;
            int num = (height1 - height2) / 2;
            int y = top + num + 1;
            menuCheckSize = SystemInformation.MenuCheckSize;
            int width = menuCheckSize.Width;
            menuCheckSize = SystemInformation.MenuCheckSize;
            int height3 = menuCheckSize.Height;
            Color foreColor = isSelected ? SystemColors.HighlightText : SystemColors.MenuText;
            Color backColor = isSelected ? SystemColors.Highlight : SystemColors.Menu;
            ControlPaint.DrawMenuGlyph(graphics, x, y, width, height3, MenuGlyph.Bullet, foreColor, backColor);
          }
          else
          {
            Graphics graphics = e.Graphics;
            int x = e.Bounds.Left + (26 - SystemInformation.MenuCheckSize.Width) / 2;
            int top = e.Bounds.Top;
            int height1 = e.Bounds.Height;
            Size menuCheckSize = SystemInformation.MenuCheckSize;
            int height2 = menuCheckSize.Height;
            int num = (height1 - height2) / 2;
            int y = top + num + 1;
            menuCheckSize = SystemInformation.MenuCheckSize;
            int width = menuCheckSize.Width;
            menuCheckSize = SystemInformation.MenuCheckSize;
            int height3 = menuCheckSize.Height;
            Color foreColor = isSelected ? SystemColors.HighlightText : SystemColors.MenuText;
            Color backColor = isSelected ? SystemColors.Highlight : SystemColors.Menu;
            ControlPaint.DrawMenuGlyph(graphics, x, y, width, height3, MenuGlyph.Checkmark, foreColor, backColor);
          }
        }
        else
        {
          Image image = this.GetImage((MenuItem) sender);
          if (image != null)
          {
            if (((MenuItem) sender).Enabled)
              e.Graphics.DrawImage(image, e.Bounds.Left + 4, e.Bounds.Top + (e.Bounds.Height - 16) / 2, 16, 16);
            else
              ControlPaint.DrawImageDisabled(e.Graphics, image, e.Bounds.Left + 4, e.Bounds.Top + (e.Bounds.Height - 16) / 2, SystemColors.Menu);
          }
        }
      }
    }

    private static string ShortcutToString(Shortcut shortcut)
    {
      if (shortcut == Shortcut.None)
        return (string) null;
      Keys keys = (Keys) shortcut;
      return TypeDescriptor.GetConverter(keys.GetType()).ConvertToString((object) keys);
    }

    private void DrawText(object sender, DrawItemEventArgs e, bool isSelected)
    {
      string text = VistaMenu.ShortcutToString(((MenuItem) sender).Shortcut);
      Rectangle bounds1 = e.Bounds;
      int top = bounds1.Top;
      bounds1 = e.Bounds;
      int num = (bounds1.Height - SystemFonts.MenuFont.Height) / 2;
      int y = top + num;
      Font font = ((MenuItem) sender).DefaultItem ? VistaMenu.menuBoldFont : SystemFonts.MenuFont;
      Size size = TextRenderer.MeasureText(((MenuItem) sender).Text, font, Size.Empty, TextFormatFlags.NoClipping | TextFormatFlags.SingleLine);
      Rectangle bounds2 = new Rectangle(e.Bounds.Left + 4 + 16 + 6, y, size.Width, size.Height);
      if (!((MenuItem) sender).Enabled && !isSelected)
      {
        bounds2.Offset(1, 1);
        TextRenderer.DrawText((IDeviceContext) e.Graphics, ((MenuItem) sender).Text, font, bounds2, SystemColors.ControlLightLight, (TextFormatFlags) (32 | (this.isUsingKeyboardAccel ? 0 : 1048576) | 256));
        bounds2.Offset(-1, -1);
      }
      TextRenderer.DrawText((IDeviceContext) e.Graphics, ((MenuItem) sender).Text, font, bounds2, ((MenuItem) sender).Enabled ? (isSelected ? SystemColors.HighlightText : SystemColors.MenuText) : SystemColors.GrayText, (TextFormatFlags) (32 | (this.isUsingKeyboardAccel ? 0 : 1048576) | 256));
      if (text == null)
        return;
      size = TextRenderer.MeasureText(text, font, Size.Empty, TextFormatFlags.NoClipping | TextFormatFlags.SingleLine);
      bounds2 = new Rectangle(e.Bounds.Width - size.Width - 12, y, size.Width, size.Height);
      if (!((MenuItem) sender).Enabled && !isSelected)
      {
        bounds2.Offset(1, 1);
        TextRenderer.DrawText((IDeviceContext) e.Graphics, text, font, bounds2, SystemColors.ControlLightLight, (TextFormatFlags) (32 | (this.isUsingKeyboardAccel ? 0 : 1048576) | 256));
        bounds2.Offset(-1, -1);
      }
      TextRenderer.DrawText((IDeviceContext) e.Graphics, text, font, bounds2, ((MenuItem) sender).Enabled ? (isSelected ? SystemColors.HighlightText : SystemColors.MenuText) : SystemColors.GrayText, TextFormatFlags.NoClipping | TextFormatFlags.SingleLine);
    }
  }
}
