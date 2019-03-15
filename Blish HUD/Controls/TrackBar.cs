﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Blish_HUD.Controls {
    public class TrackBar:Control {
        
        private const int BUFFER_WIDTH = 4;

        public event EventHandler<EventArgs> ValueChanged;

        private int _maxValue = 100;
        public int MaxValue {
            get => _maxValue;
            set {
                if (_maxValue == value) return;

                _maxValue = value;
                OnPropertyChanged();
            }
        }

        private int _minValue = 0;
        public int MinValue {
            get => _minValue;
            set {
                if (_minValue == value) return;

                _minValue = value;
                OnPropertyChanged();
            }
        }

        private float _value = 50;
        public float Value {
            get => _value;
            set {
                if (_value == value) return;

                _value = MathHelper.Clamp(value, this.MinValue, this.MaxValue);

                OnPropertyChanged();
                OnPropertyChanged(nameof(this.IntValue));

                this.ValueChanged?.Invoke(this, new EventArgs());
            }
        }

        public int IntValue {
            get => (int) Math.Round(_value, 0);
            set => this.Value = value;
        }

        private bool Dragging = false;
        private int DraggingOffset = 0;

        #region "Sprites"

        private static bool _spritesLoaded = false;

        private static TextureRegion2D spriteTrack;
        private static TextureRegion2D spriteNub;

        private static void LoadSprites() {
            if (_spritesLoaded) return;

            spriteTrack = ControlAtlas.GetRegion("trackbar/tb-track");
            spriteNub = ControlAtlas.GetRegion("trackbar/tb-nub");

            _spritesLoaded = true;
        }

        #endregion

        private Rectangle NubBounds;

        public TrackBar() {
            LoadSprites();

            this.Size = new Point(256, 16);

            this.LeftMouseButtonPressed += TrackBar_LeftMouseButtonPressed;
            Input.LeftMouseButtonReleased += Input_LeftMouseButtonReleased;
        }

        private void Input_LeftMouseButtonReleased(object sender, MouseEventArgs e) {
            Dragging = false;
        }

        private void TrackBar_LeftMouseButtonPressed(object sender, MouseEventArgs e) {
            var relMousePos = e.MouseState.Position - this.AbsoluteBounds.Location;

            if (NubBounds.Contains(relMousePos)) {
                Dragging = true;
                DraggingOffset = relMousePos.X - NubBounds.X;
            }
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (Dragging) {
                var relMousePos = Input.MouseState.Position - this.AbsoluteBounds.Location - new Point(DraggingOffset, 0);
                this.Value = ((float)relMousePos.X / (float)(this.Width - BUFFER_WIDTH * 2 - spriteNub.Width)) * (this.MaxValue - this.MinValue);
            }
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Mouse;
        }

        public override void Invalidate() {
            float valueOffset = (((this.Value - this.MinValue) / (this.MaxValue - this.MinValue)) * (spriteTrack.Width - BUFFER_WIDTH * 2 - spriteNub.Width));
            NubBounds = new Rectangle((int)valueOffset + BUFFER_WIDTH, 0, spriteNub.Width, spriteNub.Height);

            base.Invalidate();
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            spriteBatch.Draw(spriteTrack, bounds, Color.White);
            
            spriteBatch.Draw(spriteNub, NubBounds.OffsetBy(bounds.Location), Color.White);
        }

    }
}
