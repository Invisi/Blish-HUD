using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blish_HUD.Controls;
using Praeclarum.Bind;

namespace Blish_HUD.Modules.MouseUsability {
    public class MouseHighlight:Control {

        private Binding _bindings;

        public enum Orientation {
            Horizontal,
            Vertical,
        }

        private Color _highlightColor = Color.Red;
        public Color HighlightColor {
            get => _highlightColor;
            set {
                if (_highlightColor == value) return;

                _highlightColor = value;
                OnPropertyChanged();
            }
        }

        private Orientation _highlightOrientation = Orientation.Horizontal;
        public Orientation HighlightOrientation {
            get => _highlightOrientation;
            set {
                if (_highlightOrientation == value) return;

                _highlightOrientation = value;
                OnPropertyChanged();
            }
        }

        private static Texture2D _spriteHighlight;

        public MouseHighlight() {
            _spriteHighlight = _spriteHighlight ?? Content.GetTexture("scrollbar-track");

            new Library.Adhesive.OneWayBinding<Point, Point>(
                                                             () => this.Size,
                                                             () => GameService.Graphics.SpriteScreen.Size,
                                                             (screenSize) => {
                                                                 if (this.HighlightOrientation == Orientation.Horizontal)
                                                                     return new Point(GameService.Graphics.SpriteScreen.Width, _spriteHighlight.Width);
                                                                 else
                                                                     return new Point(_spriteHighlight.Width, GameService.Graphics.SpriteScreen.Height);
                                                             },
                                                             true
                                                            );

            new Library.Adhesive.OneWayBinding<Point, Orientation>(
                                                             () => this.Size,
                                                             () => this.HighlightOrientation,
                                                             (screenSize) => {
                                                                 if (this.HighlightOrientation == Orientation.Horizontal)
                                                                     return new Point(GameService.Graphics.SpriteScreen.Width, _spriteHighlight.Width);
                                                                 else
                                                                     return new Point(_spriteHighlight.Width, GameService.Graphics.SpriteScreen.Height);
                                                             },
                                                             true
                                                            );
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.None;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            for (var i = 0; i < this.Height / Content.GetTexture("scrollbar-track").Height + 1; i++) {
                spriteBatch.Draw(_spriteHighlight, _spriteHighlight.Bounds.OffsetBy(0, i * _spriteHighlight.Height), this.HighlightColor);
            }

            if (this.HighlightOrientation == Orientation.Horizontal) {
                spriteBatch.Draw(_spriteHighlight, bounds, this.HighlightColor);
            }
        }

    }
}
