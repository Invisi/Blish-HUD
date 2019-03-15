using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blish_HUD.BHGw2Api;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Humanizer;
using Microsoft.Xna.Framework.Input;
using Praeclarum.Bind;

namespace Blish_HUD.Modules.MouseUsability {
    public class MouseUsability : Module {

        public MouseHighlight HorizontalHighlight;
        public MouseHighlight VerticalHighlight;
        public Image CircleHighlight;

        public override ModuleInfo GetModuleInfo() {
            return new ModuleInfo(
                "(General) Mouse Module",
                "bh.general.mouse",
                "Provides various mouse QoL features.",
                "LandersXanders.1235",
                "1"
            );
        }

        #region Settings
        private SettingEntry<Color> _settingHighlightColor;
        private SettingEntry<Color> _settingHighlightOutlineColor;
        private SettingEntry<int>   _settingHighlightThickness;
        private SettingEntry<int>   _settingHighlightOutlineThickness;
        private SettingEntry<float> _settingHighlightOpacity;

        private SettingEntry<bool> _settingShowCursorHighlight;
        private SettingEntry<bool> _settingShowHighlightOverBlishHud;

        public override void DefineSettings(Settings settings) {
            // Define settings
            _settingHighlightColor = settings.DefineSetting<Color>("mhl.lines.highlightColor", Color.Red, Color.Red, true, "The color of the mouse highlight.");
            _settingHighlightOutlineColor = settings.DefineSetting<Color>("mhl.lines.highlightOutlineColor", Color.Black, Color.Black, true, "The color of the mouse highlight outline.");
            _settingHighlightThickness = settings.DefineSetting<int>("mhl.lines.highlightThickness", 2, 2, true, "The thickness of the mouse highlight.");
            _settingHighlightOutlineThickness = settings.DefineSetting<int>("mhl.lines.outlineThickness", 1, 1, true, "The thickness of mouse highlight outline.");
            _settingHighlightOpacity = settings.DefineSetting<float>("mhl.lines.highlightOpacity", 1.0f, 1.0f, true, "The opacity of the mouse highlight.");

            _settingShowCursorHighlight = settings.DefineSetting<bool>("mhl.showHighlight", false, false, true, "If the highlight should be shown or not.");
            _settingShowHighlightOverBlishHud = settings.DefineSetting<bool>("mhl.showOverBh", true, true, true, "Show the mouse highlight over top of the Blish HUD UI?");
        }

        #endregion

        protected override void OnLoad() {
            HorizontalHighlight?.Dispose();
            VerticalHighlight?.Dispose();
            CircleHighlight?.Dispose();
        }

        private MouseHighlight _top;
        private MouseHighlight _right;
        private MouseHighlight _bottom;
        private MouseHighlight _left;

        public override void OnStart() {
            HorizontalHighlight = new MouseHighlight {
                HighlightOrientation = MouseHighlight.Orientation.Horizontal,
                HighlightColor       = Color.Red,
                Visible = false
            };

            VerticalHighlight = new MouseHighlight {
                HighlightOrientation = MouseHighlight.Orientation.Vertical,
                HighlightColor       = Color.Blue,
                Visible = false
            };

            CircleHighlight = new Image(GameService.Content.GetTexture("1032331-thick")) {
                Location = GameService.Input.MouseState.Position - new Point(32, 32),
                Parent   = GameService.Graphics.SpriteScreen
            };

            _top = new MouseHighlight() {
                HighlightOrientation = MouseHighlight.Orientation.Vertical,
                HighlightColor       = _settingHighlightColor.Value,
                Visible              = true,
                Height               = 1080
            };

            _right = new MouseHighlight() {
                HighlightOrientation = MouseHighlight.Orientation.Horizontal,
                HighlightColor       = _settingHighlightColor.Value,
                Visible              = true,
                Width                = 1080
            };

            _bottom = new MouseHighlight() {
                HighlightOrientation = MouseHighlight.Orientation.Vertical,
                HighlightColor       = _settingHighlightColor.Value,
                Visible              = true,
                Height               = 1080
            };
            
            _left = new MouseHighlight() {
                HighlightOrientation = MouseHighlight.Orientation.Horizontal,
                HighlightColor       = _settingHighlightColor.Value,
                Visible              = true,
                Width                = 1080,
            };

            var tbinding = new Library.Adhesive.OneWayBinding<Point, MouseState>(
                                                                                 () => _top.Location,
                                                                                 () => GameService.Input.MouseState,
                                                                                 (ms) => new Point(GameService.Input.MouseState.Position.X - _top.Width / 2, GameService.Input.MouseState.Y - _top.Height - CircleHighlight.Height / 2 + 9),
                                                                                 true
                                                                                );

            var rbinding = new Library.Adhesive.OneWayBinding<Point, MouseState>(
                                                                                 () => _right.Location,
                                                                                 () => GameService.Input.MouseState,
                                                                                 (ms) => new Point(GameService.Input.MouseState.Position.X + CircleHighlight.Height / 2 - 9, GameService.Input.MouseState.Y - _right.Height / 2),
                                                                                 true
                                                                                );

            var bbinding = new Library.Adhesive.OneWayBinding<Point, MouseState>(
                                                                                 () => _bottom.Location,
                                                                                 () => GameService.Input.MouseState,
                                                                                 (ms) => new Point(GameService.Input.MouseState.Position.X - _bottom.Width / 2, GameService.Input.MouseState.Y + CircleHighlight.Height / 2 - 9),
                                                                                 true
                                                                                );

            var lbinding = new Library.Adhesive.OneWayBinding<Point, MouseState>(
                                                                                 () => _left.Location,
                                                                                 () => GameService.Input.MouseState,
                                                                                 (ms) => new Point(GameService.Input.MouseState.Position.X - _left.Width - CircleHighlight.Height / 2 + 9, GameService.Input.MouseState.Y - _left.Height / 2),
                                                                                 true
                                                                                );

            new Library.Adhesive.OneWayBinding<Color, Color>(
                                                             () => _top.HighlightColor,
                                                             () => _settingHighlightColor.Value,
                                                             applyLeft: true
                                                            );

            new Library.Adhesive.OneWayBinding<Color, Color>(
                                                             () => _right.HighlightColor,
                                                             () => _settingHighlightColor.Value,
                                                             applyLeft: true
                                                            );

            new Library.Adhesive.OneWayBinding<Color, Color>(
                                                             () => _bottom.HighlightColor,
                                                             () => _settingHighlightColor.Value,
                                                             applyLeft: true
                                                            );

            new Library.Adhesive.OneWayBinding<Color, Color>(
                                                             () => _left.HighlightColor,
                                                             () => _settingHighlightColor.Value,
                                                             applyLeft: true
                                                            );

            //Binding.Create(() => _top.Location == (GameService.Input.MouseState != null ? new Point(GameService.Input.MouseState.X, GameService.Input.MouseState.Y) : Point.Zero));

            _top.Parent = GameService.Graphics.SpriteScreen;
            _right.Parent = GameService.Graphics.SpriteScreen;
            _bottom.Parent = GameService.Graphics.SpriteScreen;
            _left.Parent = GameService.Graphics.SpriteScreen;
            HorizontalHighlight.Parent = GameService.Graphics.SpriteScreen;
            VerticalHighlight.Parent = GameService.Graphics.SpriteScreen;

            // Update features to match saved settings state
            Binding.Create(() => HorizontalHighlight.Visible == _settingShowCursorHighlight.Value);
            Binding.Create(() => VerticalHighlight.Visible == _settingShowCursorHighlight.Value);
            //Binding.Create(() => HorizontalHighlight.HighlightThickness == _settingHighlightThickness.Value);
            //Binding.Create(() => VerticalHighlight.HighlightThickness == _settingHighlightThickness.Value);

            HorizontalHighlight.ZIndex = _settingShowHighlightOverBlishHud.Value ? int.MaxValue : int.MinValue;
            VerticalHighlight.ZIndex   = _settingShowHighlightOverBlishHud.Value ? int.MaxValue : int.MinValue;

            GameService.Director.BlishHudWindow.AddTab("Mouse Usability", "mouse-icon", BuildSettingPanel());
        }


        private Panel BuildSettingPanel() {
            var muPanel = new Panel();

            var lblMouseHighlight = new Label() {
                Parent = muPanel,
                Location = new Point(15, 25),
                Text = "Mouse Highlight",
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                TextColor = Color.White,
                Font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size18, ContentService.FontStyle.Regular),
            };

            var colorPicker = new ColorPicker() {
                Width = 384 + 57,
                Height = 128,
                Location = new Point(lblMouseHighlight.Left + 10, lblMouseHighlight.Bottom + 5),
                Parent = muPanel
            };

            var cpScroller = new Scrollbar(colorPicker) {
                Parent = muPanel,
                Left = colorPicker.Right,
                Top = colorPicker.Top,
                Height = colorPicker.Height
            };

            var lblHighlightColor = new Label() {
                Parent = muPanel,
                Top = colorPicker.Bottom + 10,
                Left = colorPicker.Left,
                Text = "Highlight Color",
                Height = 32,
                VerticalAlignment = Utils.DrawUtil.VerticalAlignment.Middle,
                AutoSizeWidth = true,
                TextColor = Color.White,
                Font = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            };

            var cdd = new ColorDropdown() {
                Parent = muPanel,
                Top = lblHighlightColor.Bottom + 10,
                Left = lblHighlightColor.Left,
                Width = 150,
            };

            var cb = new Checkbox() {
                Parent = muPanel,
                Top = cdd.Bottom + 10,
                Left = cdd.Left,
                Width = 120,
                Text = "Show Highlight"
            };

            Binding.Create(() => cb.Checked == _settingShowCursorHighlight.Value);


            cdd.AddColorItem(Color.Red, "Red");
            cdd.AddColorItem(Color.Orange, "Orange");
            cdd.AddColorItem(Color.Yellow, "Yellow");
            cdd.AddColorItem(Color.Green, "Green");
            cdd.AddColorItem(Color.Blue, "Blue");
            cdd.AddColorItem(Color.Purple, "Purple");
            cdd.AddColorItem(Color.Brown, "Brown");
            cdd.AddColorItem(Color.Magenta, "Magenta");
            cdd.AddColorItem(Color.Tan, "Tan");
            cdd.AddColorItem(Color.Cyan, "Cyan");
            cdd.AddColorItem(Color.Navy, "Navy");
            cdd.AddColorItem(Color.Lime, "Lime");
            cdd.AddColorItem(Color.White, "White");
            cdd.AddColorItem(Color.Gray, "Gray");
            cdd.AddColorItem(Color.Black, "Black");

            Binding.Create(() => cdd.SelectedColor == _settingHighlightColor.Value);

            var lblOpacity = new Label() {
                Parent         = muPanel,
                Top            = cb.Bottom + 6,
                Left           = cb.Left,
                Text           = "Opacity",
                AutoSizeHeight = true,
                AutoSizeWidth  = true,
                TextColor      = Color.White,
                Font           = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            };

            var tbOpacity = new TrackBar() {
                Parent   = muPanel,
                Top      = lblOpacity.Bottom + 5,
                Left     = lblOpacity.Left,
                MinValue = 0,
                MaxValue = 1,
                Value    = 1.0f,
            };

            Binding.Create(() => tbOpacity.Value == _settingHighlightOpacity.Value);

            var lblThickness = new Label() {
                Parent         = muPanel,
                Top            = tbOpacity.Bottom + 6,
                Left           = tbOpacity.Left,
                Text           = "Thickness",
                AutoSizeHeight = true,
                AutoSizeWidth  = true,
                TextColor      = Color.White,
                Font           = GameService.Content.GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            };

            var tbThickness = new TrackBar() {
                Parent   = muPanel,
                Top      = lblThickness.Bottom + 5,
                Left     = lblThickness.Left,
                MinValue = 1,
                MaxValue = 7,
                Value    = 1,
            };

            //Binding.Create(() => tbThickness.IntValue == _settingHighlightThickness.Value);

            var nbinding = new Library.Adhesive.TwoWayBinding<int, int>(
                                                         () => tbThickness.IntValue,
                                                         () => _settingHighlightThickness.Value
                                                        );

            //new Library.Adhesive.TwoWayBinding<Color, Color>(() => HorizontalHighlight.HighlightColor, () => _settingHighlightColor.Value);
            //new Library.Adhesive.TwoWayBinding<int, int>(() => HorizontalHighlight.HighlightThickness, () => _settingHighlightThickness.Value);
            //new Library.Adhesive.TwoWayBinding<float, float>(() => HorizontalHighlight.Opacity, () => _settingHighlightOpacity.Value);

            //new Library.Adhesive.TwoWayBinding<Color, Color>(() => VerticalHighlight.HighlightColor, () => _settingHighlightColor.Value);
            //new Library.Adhesive.TwoWayBinding<int, int>(() => VerticalHighlight.HighlightThickness, () => _settingHighlightThickness.Value);
            //new Library.Adhesive.TwoWayBinding<float, float>(() => VerticalHighlight.Opacity, () => _settingHighlightOpacity.Value);

            Binding.Create(() =>
                               HorizontalHighlight.HighlightColor == _settingHighlightColor.Value &&
                               HorizontalHighlight.Opacity == _settingHighlightOpacity.Value
                             &&
                               VerticalHighlight.HighlightColor == _settingHighlightColor.Value &&
                               VerticalHighlight.Opacity == _settingHighlightOpacity.Value
                             &&
                               CircleHighlight.Color == _settingHighlightColor.Value
                          );
            

            #region old code
            //var highlightColor = new ColorBox() {
            //    Parent = muPanel,
            //    Top = colorPicker.Bottom + 10,
            //    Left = lblHighlightColor.Right + 5,
            //    ColorId = setting_hl_highlightColorId.Value
            //};

            //GameServices.GetService<DataBindingService>().AddBinding(
            // setting_hl_highlightColorId, "Value",
            // highlightColor, "ColorId"
            //);

            //Binding.Create(() => setting_hl_highlightColorId.Value == highlightColor.ColorId);

            //var lblOutlineColor = new Label() {
            //    Parent = muPanel,
            //    Top = colorPicker.Bottom + 10,
            //    Left = highlightColor.Right + 25,
            //    Text = "Outline Color",
            //    Height = 32,
            //    VerticalAlignment = Utils.DrawUtil.VerticalAlignment.Middle,
            //    AutoSizeWidth = true,
            //    TextColor = Color.White,
            //    Font = GameServices.GetService<ContentService>().GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            //};

            //var outlineColor = new ColorBox() {
            //    Parent = muPanel,
            //    Left = lblOutlineColor.Right + 5,
            //    Top = colorPicker.Bottom + 10,
            //    ColorId = setting_hl_outlineColorId.Value
            //};

            //var cbMouseHighlight = new Checkbox() {
            //    Parent = muPanel,
            //    Top = highlightColor.Bottom + 10,
            //    Left = colorPicker.Left,
            //    Text = "Enable Mouse Highlight",
            //    Font = GameServices.GetService<ContentService>().GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            //    Checked = setting_hl_showHighlight.Value,
            //};

            //var cbShowOverUI = new Checkbox() {
            //    Parent = muPanel,
            //    Top = cbMouseHighlight.Bottom + 5,
            //    Left = cbMouseHighlight.Left,
            //    Text = "Show Mouse Highlight Over Blish HUD UI",
            //    Font = GameServices.GetService<ContentService>().GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            //    Checked = setting_hl_showOverBlishHud.Value,
            //};

            //var lblHighlightThickness = new Label() {
            //    Parent = muPanel,
            //    Top = lblOpacity.Bottom + 6,
            //    Left = colorPicker.Left,
            //    Text = "Highlight Thickness",
            //    AutoSizeHeight = true,
            //    AutoSizeWidth = true,
            //    TextColor = Color.White,
            //    Font = GameServices.GetService<ContentService>().GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            //};

            //var tbHighlightThickness = new TrackBar() {
            //    Parent = muPanel,
            //    Top = lblHighlightThickness.Top + lblHighlightThickness.Height / 2 - 8,
            //    Left = cpScroller.Right - 256,
            //    Value = 2,
            //    MinValue = 1,
            //    MaxValue = 15,
            //};

            // Wire settings to control so they stay in sync
            //GameServices.GetService<DataBindingService>().AddBinding(
            //    setting_hl_hightlightThickness, "Value",
            //    tbHighlightThickness, "Value", new OneWayBinding[] {
            //        new OneWayBinding(HorizontalHighlight, "HighlightThickness"),
            //        new OneWayBinding(VerticalHighlight, "HighlightThickness"),
            //    }
            //);

            //var lblOutlineThickness = new Label() {
            //    Parent = muPanel,
            //    Top = tbHighlightThickness.Bottom + 6,
            //    Left = colorPicker.Left,
            //    Text = "Outline Thickness",
            //    AutoSizeHeight = true,
            //    AutoSizeWidth = true,
            //    TextColor = Color.White,
            //    Font = GameServices.GetService<ContentService>().GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            //};

            //var tbOutlineThickness = new TrackBar() {
            //    Parent = muPanel,
            //    Top = lblOutlineThickness.Top + lblOutlineThickness.Height / 2 - 8,
            //    Left = cpScroller.Right - 256,
            //    Value = 2,
            //    MinValue = 0,
            //    MaxValue = 5,
            //};

            //cbMouseHighlight.CheckedChanged += delegate {
            //    HorizontalHighlight.Visible = cbMouseHighlight.Checked;
            //    VerticalHighlight.Visible = cbMouseHighlight.Checked;

            //    setting_hl_showHighlight.Value = cbMouseHighlight.Checked;
            //};

            //cbShowOverUI.CheckedChanged += delegate {
            //    HorizontalHighlight.ZIndex = cbShowOverUI.Checked ? int.MaxValue : 0;
            //    VerticalHighlight.ZIndex = cbShowOverUI.Checked ? int.MaxValue : 0;

            //    setting_hl_showOverBlishHud.Value = cbShowOverUI.Checked;
            //};

            //highlightColor.LeftMouseButtonPressed += delegate { colorPicker.AssociatedColorBox = highlightColor; };
            //outlineColor.LeftMouseButtonPressed += delegate { colorPicker.AssociatedColorBox = outlineColor; };

            //highlightColor.ColorChanged += delegate { HorizontalHighlight.HighlightColor = highlightColor.Color.Leather.Rgb.ToXnaColor(); VerticalHighlight.HighlightColor = highlightColor.Color.Leather.Rgb.ToXnaColor(); };
            //outlineColor.ColorChanged += delegate { HorizontalHighlight.OutlineColor = outlineColor.Color.Leather.Rgb.ToXnaColor(); VerticalHighlight.OutlineColor = outlineColor.Color.Leather.Rgb.ToXnaColor(); };

            //colorPicker.AssociatedColorBox = highlightColor;
            #endregion

            return muPanel;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            
            if (GameService.Input.MouseHidden) return;
            
            HorizontalHighlight.Top = GameService.Input.MouseState.Position.Y - HorizontalHighlight.Height / 2;
            VerticalHighlight.Left = GameService.Input.MouseState.Position.X - VerticalHighlight.Width / 2;
            CircleHighlight.Location = GameService.Input.MouseState.Position - new Point(32, 32);
        }

        protected override void OnStop() {
            base.OnStop();

            HorizontalHighlight?.Dispose();
            VerticalHighlight?.Dispose();
        }

    }
}
