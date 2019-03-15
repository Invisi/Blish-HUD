﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using SharpDX.Direct3D9;

namespace Blish_HUD.Controls {
    public class Dropdown:Control {

        private class DropdownPanel:Control {

            private Dropdown _assocDropdown;

            private int _highlightedItem = -1;
            private int HighlightedItem {
                get => _highlightedItem;
                set {
                    if (_highlightedItem == value) return;

                    _highlightedItem = value;
                    OnPropertyChanged();
                }
            }

            private DropdownPanel(Dropdown assocDropdown) {
                _assocDropdown = assocDropdown;

                this.Location = _assocDropdown.AbsoluteBounds.Location + new Point(0, assocDropdown.Height - 1);
                this.Size = new Point(_assocDropdown.Width, _assocDropdown.Height * _assocDropdown.Items.Count);
                this.Parent = Graphics.SpriteScreen;
                this.ZIndex = int.MaxValue;

                Input.LeftMouseButtonPressed += Input_MousedOffDropdownPanel;
                Input.RightMouseButtonPressed += Input_MousedOffDropdownPanel;
            }

            protected override CaptureType CapturesInput() {
                return CaptureType.Mouse;
            }

            public static DropdownPanel ShowPanel(Dropdown assocDropdown) {
                return new DropdownPanel(assocDropdown);
            }

            private void Input_MousedOffDropdownPanel(object sender, MouseEventArgs e) {
                if (!this.MouseOver) {
                    if (_assocDropdown.MouseOver) _assocDropdown._hadPanel = true;
                    Dispose();
                }
            }

            protected override void OnMouseMoved(MouseEventArgs e) {
                base.OnMouseMoved(e);
                
                this.HighlightedItem = this.RelativeMousePosition.Y / _assocDropdown.Height;
            }

            protected override void OnClick(MouseEventArgs e) {
                base.OnClick(e);

                _assocDropdown.SelectedItem = _assocDropdown.Items[this.HighlightedItem];
                Dispose();
            }

            protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
                _assocDropdown.PaintItemPanel(spriteBatch, bounds, this.HighlightedItem);
            }

            protected override void Dispose(bool disposing) {
                if (_assocDropdown != null) {
                    _assocDropdown._lastPanel = null;
                    _assocDropdown = null;
                }

                Input.LeftMouseButtonPressed -= Input_MousedOffDropdownPanel;
                Input.RightMouseButtonPressed -= Input_MousedOffDropdownPanel;

                base.Dispose(disposing);
            }

        }

        protected const int  DROPDOWN_HEIGHT = 25;

        protected const int EDGE_PADDING = 5;

        public class ValueChangedEventArgs : EventArgs {
            public string PreviousValue { get; }
            public string CurrentValue { get; }

            public ValueChangedEventArgs(string previousValue, string currentValue) {
                this.PreviousValue = previousValue;
                this.CurrentValue = currentValue;
            }
        }
        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        protected virtual void OnValueChanged(ValueChangedEventArgs e) {
            this.ValueChanged?.Invoke(this, e);
        }

        public ObservableCollection<string> Items { get; protected set; }

        private string _selectedItem;
        public string SelectedItem {
            get => _selectedItem;
            set {
                string previousValue = _selectedItem;

                _selectedItem = value;
                OnPropertyChanged();

                OnValueChanged(new ValueChangedEventArgs(previousValue, _selectedItem));
            }
        }

        private DropdownPanel _lastPanel = null;
        private bool _hadPanel = false;

        private static TextureRegion2D spriteInputBox;
        private static TextureRegion2D spriteArrow;
        private static TextureRegion2D spriteArrowActive;

        public Dropdown() {
            // Load static resources
            spriteInputBox = spriteInputBox ?? ControlAtlas.GetRegion("inputboxes/input-box");
            spriteArrow = spriteArrow ?? ControlAtlas.GetRegion("inputboxes/dd-arrow");
            spriteArrowActive = spriteArrowActive ?? ControlAtlas.GetRegion("inputboxes/dd-arrow-active");

            //
            this.Items = new ObservableCollection<string>();

            this.Items.CollectionChanged += delegate {
                ItemsUpdated();
                Invalidate();
            };

            this.Size = new Point(this.Width, DROPDOWN_HEIGHT);
        }

        protected override void OnClick(MouseEventArgs e) {
            base.OnClick(e);

            if (_lastPanel == null && !_hadPanel)
                _lastPanel = DropdownPanel.ShowPanel(this);
            else if (_hadPanel)
                _hadPanel = false;
        }

        private void ItemsUpdated() {
            if (string.IsNullOrEmpty(this.SelectedItem)) this.SelectedItem = this.Items.FirstOrDefault();
        }

        protected void DrawControlBacking(SpriteBatch spriteBatch, Rectangle bounds) {
            spriteBatch.Draw(Content.GetTexture("input-box"), bounds.Subtract(new Rectangle(0, 0, 5, 0)), new Rectangle(0, 0,        Math.Min(GameServices.GetService<ContentService>().GetTexture("textbox").Width - 5, this.Width - 5), Content.GetTexture("textbox").Height), Color.White);
            spriteBatch.Draw(Content.GetTexture("input-box"), new Rectangle(bounds.Right - 5,                              bounds.Y, 5,                                                                                                   bounds.Height),                        new Rectangle(GameServices.GetService<ContentService>().GetTexture("textbox").Width - 5, 0, 5, Content.GetTexture("textbox").Height), Color.White);

            spriteBatch.Draw(this.MouseOver ? spriteArrowActive : spriteArrow, new Rectangle(bounds.Right - spriteArrow.Width - 5, bounds.Height / 2 - spriteArrow.Height / 2, spriteArrow.Width, spriteArrow.Height), Color.White);
        }

        protected void DrawSelectedItem(SpriteBatch spriteBatch, Rectangle bounds) {
            Utils.DrawUtil.DrawAlignedText(spriteBatch, Overlay.font_def12, this.SelectedItem, new Rectangle(bounds.X + EDGE_PADDING, bounds.Y, bounds.Width - (EDGE_PADDING * 2) - spriteArrow.Width, bounds.Height), Color.FromNonPremultiplied(239, 240, 239, 255), Utils.DrawUtil.HorizontalAlignment.Left, Utils.DrawUtil.VerticalAlignment.Middle);
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            DrawControlBacking(spriteBatch, bounds);
            DrawSelectedItem(spriteBatch, bounds);
        }

        #region Paining DropdownItemPanel

        protected void PaintItemPanelBacking(SpriteBatch spriteBatch, Rectangle bounds, int highlightedItem) {
            spriteBatch.Draw(ContentService.Textures.Pixel, bounds, Color.Black);
            spriteBatch.Draw(ContentService.Textures.Pixel, 
                             new Rectangle(
                                           2, 
                                           2 + this.Height * highlightedItem, 
                                           bounds.Width - 12 - spriteArrow.Width, 
                                           this.Height - 4), 
                             new Color(45, 37, 25, 255));
        }

        protected virtual void PaintItemPanelItemText(SpriteBatch spriteBatch, Rectangle bounds, string itemText, Color textColor) {
            Utils.DrawUtil.DrawAlignedText(spriteBatch,
                                           Overlay.font_def12,
                                           itemText,
                                           bounds, textColor,
                                           Utils.DrawUtil.HorizontalAlignment.Left,
                                           Utils.DrawUtil.VerticalAlignment.Middle);
        }

        protected void PaintItemPanelItems(SpriteBatch spriteBatch, Rectangle bounds, int highlightedItem) {
            for (int index = 0; index < this.Items.Count; index++) {
                Color textColor = index == highlightedItem 
                                      ? ContentService.Colors.Chardonnay
                                      : Color.FromNonPremultiplied(239, 240, 239, 255);

                PaintItemPanelItemText(spriteBatch,
                                       new Rectangle(bounds.X + 8,
                                                     this.Height * index,
                                                     bounds.Width - 13 - spriteArrow.Width,
                                                     this.Height),
                                       this.Items[index],
                                       textColor);
            }
        }

        protected virtual void PaintItemPanel(SpriteBatch spriteBatch, Rectangle bounds, int highlightedItem) {
            PaintItemPanelBacking(spriteBatch, bounds, highlightedItem);
            PaintItemPanelItems(spriteBatch, bounds, highlightedItem);
        }

        #endregion

    }
}
