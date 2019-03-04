using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Blish_HUD.Controls {

    // TODO: The ColorPicker needs updates for events, it should probably inherit from FlowPanel, and needs to get reconnected once we have Gorrik.NET included in the project
    public class ColorPicker:Container {

        private const int COLOR_SIZE = 32;
        private const int COLOR_PADDING = 0;
        
        public event EventHandler<ColorBox.ColorChangedEventArgs> SelectedColorChanged;

        protected virtual void OnSelectedColorChanged(ColorBox.ColorChangedEventArgs e) {
            this.SelectedColorChanged?.Invoke(this, e);
        }

        public ObservableCollection<Color> Colors { get; protected set; }

        private Dictionary<string, ColorBox> ColorBoxes;
        
        private int hColors;

        private Color _selectedColor;
        public Color SelectedColor {
            get => _selectedColor;
            protected set {
                if (_selectedColor == value) return;

                var previousColor = _selectedColor;

                _selectedColor = value;
                if (this.AssociatedColorBox != null) {
                    this.AssociatedColorBox.Color = value;
                    OnSelectedColorChanged(new ColorBox.ColorChangedEventArgs(previousColor, _selectedColor));
                }
            }
        }

        private ColorBox _associatedColorBox;
        public ColorBox AssociatedColorBox {
            get => _associatedColorBox;
            set {
                if (_associatedColorBox == value) return;

                if (_associatedColorBox != null) _associatedColorBox.Selected = false;

                _associatedColorBox = value;
                _associatedColorBox.Selected = true;
                this.SelectedColor = this.AssociatedColorBox.Color;
            }
        }

        //public ColorPicker() : base() {
        //    this.Colors = new ObservableCollection<Color>();
        //    ColorBoxes = new Dictionary<string, ColorBox>();

        //    this.ContentRegion = new Rectangle(COLOR_PADDING, COLOR_PADDING, this.Width - (COLOR_PADDING * 2), this.Height - (COLOR_PADDING * 2));

        //    this.Colors.CollectionChanged += Colors_CollectionChanged;
        //}

        //private void Colors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        //    // Remove any items that were removed (first because moving an item in a collection will put
        //    // that item in both OldItems and NewItems)
        //    if (e.OldItems != null) {
        //        foreach (Color delItem in e.OldItems) {
        //            if (ColorBoxes.ContainsKey(delItem)) {
        //                ColorBoxes[delItem].Dispose();
        //                ColorBoxes.Remove(delItem);
        //            }
        //        }
        //    }

        //    if (e.NewItems != null) {
        //        foreach (BHGw2Api.DyeColor addItem in e.NewItems) {
        //            if (!ColorBoxes.ContainsKey(addItem)) {
        //                var cb = new ColorBox() {
        //                    Color = addItem,
        //                    Parent = this
        //                };
        //                ColorBoxes.Add(addItem, cb);
                        
        //                cb.LeftMouseButtonPressed += delegate {
        //                    ColorBoxes.Values.ToList().ForEach(box => box.Selected = false);

        //                    cb.Selected = true;
        //                    this.SelectedColor = cb.Color;
        //                };
        //            }
        //        }
        //    }

        //    // Relayout the color grid
        //    for (int i = 0; i < this.Colors.Count; i++) {
        //        var curColor = this.Colors[i];
        //        var curBox = ColorBoxes[curColor];

        //        int hPos = i % hColors;
        //        int vPos = i / hColors;

        //        curBox.Location = new Point(
        //            hPos * (curBox.Width + COLOR_PADDING),
        //            vPos * (curBox.Width + COLOR_PADDING)
        //        );
        //    }
        //}

        public override void Invalidate() {
            base.Invalidate();

            hColors = this.Width / (COLOR_SIZE + COLOR_PADDING);
            this.Width = Math.Max(hColors * (COLOR_SIZE + COLOR_PADDING) + COLOR_PADDING, COLOR_SIZE + COLOR_PADDING * 2);
            this.ContentRegion = new Rectangle(COLOR_PADDING, COLOR_PADDING, this.Width - (COLOR_PADDING * 2), this.Height - (COLOR_PADDING * 2));
        }

        public override void PaintContainer(SpriteBatch spriteBatch, Rectangle bounds) {
            // Draw background
            spriteBatch.Draw(Content.GetTexture(@"common\solid"), bounds, Color.Black * 0.3f);
        }
    }
}
