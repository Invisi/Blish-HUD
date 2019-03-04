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
using Praeclarum.Bind;

namespace Blish_HUD.Modules.MouseUsability {
    public class MouseUsability : Module {

        public MouseHighlight HorizontalHighlight;
        public MouseHighlight VerticalHighlight;

        // TODO: Make this something useable elsewhere ("wait for pre-req" or something)
        private Thread Prereq;

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

        // TODO: Rename these so that they're easier to work with - not quite sure what I was thinking
        private SettingEntry<int> setting_hl_highlightColorId;
        private SettingEntry<int> setting_hl_outlineColorId;
        private SettingEntry<float> setting_hl_hightlightThickness;
        private SettingEntry<float> setting_hl_outlineThickness;
        private SettingEntry<float> setting_hl_highlightOpacity;

        private SettingEntry<bool> setting_hl_showHighlight;
        private SettingEntry<bool> setting_hl_showOverBlishHud;

        public override void DefineSettings(Settings settings) {
            // Define settings
            setting_hl_highlightColorId = settings.DefineSetting<int>("mhl.highlightColorId", 1541, 1541);
            setting_hl_outlineColorId = settings.DefineSetting<int>("mhl.outlineColorId", 1354, 1354);
            setting_hl_hightlightThickness = settings.DefineSetting<float>("mhl.highlightThickness", 2.0f, 2.0f);
            setting_hl_outlineThickness = settings.DefineSetting<float>("mhl.outlineThickness", 1.0f, 1.0f);
            setting_hl_highlightOpacity = settings.DefineSetting<float>("mhl.highlightOpacity", 1.0f, 1.0f);

            setting_hl_showHighlight = settings.DefineSetting<bool>("mhl.showHighlight", false, false);
            setting_hl_showOverBlishHud = settings.DefineSetting<bool>("mhl.showOverBh", true, true);

            // Wiring up settings
            //setting_hl_showHighlight.Set
        }

        #endregion

        protected override void OnLoad() {
            base.OnLoad();

            HorizontalHighlight?.Dispose();
            VerticalHighlight?.Dispose();

            HorizontalHighlight = new MouseHighlight(MouseHighlight.Orientation.Horizontal);
            VerticalHighlight = new MouseHighlight(MouseHighlight.Orientation.Vertical);

            HorizontalHighlight.Parent = GameServices.GetService<GraphicsService>().SpriteScreen;
            VerticalHighlight.Parent = GameServices.GetService<GraphicsService>().SpriteScreen;

            // Update features to match saved settings state
            HorizontalHighlight.Visible = setting_hl_showHighlight.Value;
            VerticalHighlight.Visible = setting_hl_showHighlight.Value;

            HorizontalHighlight.ZIndex = setting_hl_showOverBlishHud.Value ? int.MaxValue : 0;
            VerticalHighlight.ZIndex = setting_hl_showOverBlishHud.Value ? int.MaxValue : 0;

            Prereq = new Thread(MakeAvailable);
            Prereq.Start();
        }

        private async void MakeAvailable() {
            // TODO: this is incredibly nasty and should definitely not be here over pretty much any other location
            //AllColors = await ApiItem.CallForManyAsync<DyeColor>("/v2/colors?ids=all", 5.Days(), true);

            // Add tab with settings in the main Blish HUD window
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
                Font = GameServices.GetService<ContentService>().GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size18, ContentService.FontStyle.Regular),
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
                Font = GameServices.GetService<ContentService>().GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            };

            var cdd = new ColorDropdown() {
                Parent = muPanel,
                Top = lblHighlightColor.Bottom + 10,
                Left = lblHighlightColor.Left,
                Width = 150,
            };
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
            cdd.AddColorItem(Color.Olive, "Olive");
            cdd.AddColorItem(Color.Maroon, "Maroon");
            cdd.AddColorItem(Color.Navy, "Navy");
            cdd.AddColorItem(Color.Aquamarine, "Aquamarine");
            cdd.AddColorItem(Color.Turquoise, "Turquoise");
            cdd.AddColorItem(Color.Silver, "Silver");
            cdd.AddColorItem(Color.Lime, "Lime");
            cdd.AddColorItem(Color.Teal, "Teal");
            cdd.AddColorItem(Color.Indigo, "Indigo");
            cdd.AddColorItem(Color.Violet, "Violet");
            cdd.AddColorItem(Color.Pink, "Pink");
            cdd.AddColorItem(Color.White, "White");
            cdd.AddColorItem(Color.Gray, "Gray");
            cdd.AddColorItem(Color.Black, "Black");

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

            //var lblOpacity = new Label() {
            //    Parent = muPanel,
            //    Top = cbShowOverUI.Bottom + 6,
            //    Left = cbShowOverUI.Left,
            //    Text = "Opacity",
            //    AutoSizeHeight = true,
            //    AutoSizeWidth = true,
            //    TextColor = Color.White,
            //    Font = GameServices.GetService<ContentService>().GetFont(ContentService.FontFace.Menomonia, ContentService.FontSize.Size14, ContentService.FontStyle.Regular),
            //};

            //var tbOpacity = new TrackBar() {
            //    Parent = muPanel,
            //    Top = lblOpacity.Top + lblOpacity.Height / 2 - 8,
            //    Left = cpScroller.Right - 256,
            //    MinValue = 0,
            //    MaxValue = 1,
            //    Value = 1.0f,
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

            return muPanel;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            //if (GameServices.GetService<GraphicsService>().SpriteScreen.MouseOver) {
            if (GameServices.GetService<InputService>().MouseHidden) return;

            HorizontalHighlight.Top = GameServices.GetService<InputService>().MouseState.Position.Y - HorizontalHighlight.Height / 2;
            VerticalHighlight.Left = GameServices.GetService<InputService>().MouseState.Position.X - VerticalHighlight.Width / 2;
            //}
        }

        protected override void OnDisabled() {
            base.OnDisabled();

            HorizontalHighlight?.Dispose();
            VerticalHighlight?.Dispose();
        }

    }
}
