﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Control = Blish_HUD.Controls.Control;

namespace Blish_HUD.Controls {
    public class Image : Control {

        private Texture2D _texture;
        public Texture2D Texture {
            get => _texture;
            set {
                if (_texture == value) return;

                _texture = value;

                OnPropertyChanged();
            }
        }

        private Color _color = Color.White;
        public Color Color {
            get => _color;
            set {
                if (_color == value) return;

                _color = value;
                OnPropertyChanged();
            }
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.None;
        }

        public Image() { /* NOOP */ }

        public Image(Texture2D texture) {
            this.Texture = texture;
            this.Size = texture.Bounds.Size;
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (_texture == null) return;

            spriteBatch.Draw(this.Texture, bounds, this.Color);
        }

    }
}
