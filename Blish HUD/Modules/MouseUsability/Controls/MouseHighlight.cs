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

        private int _highlightThickness = 1;
        public int HighlightThickness {
            get => _highlightThickness;
            set {
                if (_highlightThickness == value) return;

                _highlightThickness = value;
                OnPropertyChanged();

                if (_highlightOrientation == Orientation.Horizontal) {
                    this.Height = Content.GetTexture("scrollbar-track").Width;
                } else {
                    this.Width = Content.GetTexture("scrollbar-track").Width;
                }
            }
        }

        private Orientation _highlightOrientation = Orientation.Horizontal;
        public Orientation HighlightOrientation {
            get => _highlightOrientation;
            set {
                if (_highlightOrientation == value) return;

                _highlightOrientation = value;

                if (_highlightOrientation == Orientation.Horizontal) {
                    this.Left = 0;
                } else {
                    this.Top = 0;
                }

                OnPropertyChanged();
            }
        }

        private static Texture2D _spriteHighlight;

        public MouseHighlight() {
            _spriteHighlight = _spriteHighlight ?? Content.GetTexture("scrollbar-track");

            GameService.Graphics.SpriteScreen.Resized += (object sender, ResizedEventArgs e) => {
                if (this.HighlightOrientation == Orientation.Horizontal) {
                    this.Width = e.CurrentSize.X;
                } else {
                    this.Height = e.CurrentSize.Y;
                }
            };
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.None;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            for (var i = 0; i < this.Height / Content.GetTexture("scrollbar-track").Height + 1; i++) {
                spriteBatch.Draw(Content.GetTexture("scrollbar-track"), Content.GetTexture("scrollbar-track").Bounds.OffsetBy(0, i * Content.GetTexture("scrollbar-track").Height), this.HighlightColor);
            }
            
        }

    }
}
