using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blish_HUD.BHGw2Api;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Blish_HUD.Controls {
    
    public class ColorBox:Control {

        public class ColorChangedEventArgs : EventArgs {
            public Color PreviousColor { get; }
            public Color CurrentColor { get; }

            public ColorChangedEventArgs(Color previousColor, Color currentColor) {
                this.PreviousColor = previousColor;
                this.CurrentColor = currentColor;
            }
        }
        public event EventHandler<ColorChangedEventArgs> ColorChanged;
        protected virtual void OnColorChanged(ColorChangedEventArgs e) {
            this.ColorChanged?.Invoke(this, e);
        }

        public event EventHandler<EventArgs> BoxSelected;
        protected virtual void OnBoxSelected(EventArgs e) {
            if (this.Visible) Content.PlaySoundEffectByName(@"audio\color-change");

            this.BoxSelected?.Invoke(this, e);
        }

        private const int COLOR_SIZE = 32;

        private bool _selected = false;
        public bool Selected {
            get => _selected;
            set {
                if (_selected == value) return;

                _selected = value;
                OnPropertyChanged();

                OnBoxSelected(EventArgs.Empty);
            }
        }

        private Color _color;

        public Color Color {
            get => _color;
            set {
                if (_color == value) return;

                var previousColor = _color;

                _color = value;
                OnPropertyChanged();

                OnColorChanged(new ColorChangedEventArgs(previousColor, _color));
            }
        }

        #region "Statics (Sprites & Shared Resources)"

        private static TextureRegion2D[] spriteBoxes;
        private static TextureRegion2D spriteHighlight;

        private static void LoadStatics() {
            if (spriteBoxes != null) return;

            // Load static sprite regions
            spriteBoxes = new TextureRegion2D[] {
                ControlAtlas.GetRegion("colorpicker/cp-clr-v1"),
                ControlAtlas.GetRegion("colorpicker/cp-clr-v2"),
                ControlAtlas.GetRegion("colorpicker/cp-clr-v3"),
                ControlAtlas.GetRegion("colorpicker/cp-clr-v4"),
            };
            spriteHighlight = ControlAtlas.GetRegion("colorpicker/cp-clr-active");
        }

        #endregion


        public void SetColorFromDye(DyeColor setColor) {
            this.Color = setColor.Fur.Rgb.ToXnaColor();
            this.BasicTooltipText = setColor.Name;
        }

        private readonly int _drawVariation;

        public ColorBox() : base() {
            LoadStatics();

            this.Size = new Point(COLOR_SIZE);

            _drawVariation = Utils.Calc.GetRandom(0, 3);
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Mouse;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            spriteBatch.Draw(spriteBoxes[_drawVariation], bounds, this.Color);

            if (this.MouseOver || this.Selected)
                spriteBatch.Draw(spriteHighlight, bounds, Microsoft.Xna.Framework.Color.White * 0.7f);
        }

    }

}
