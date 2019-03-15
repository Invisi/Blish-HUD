using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blish_HUD.Controls {

    public class ColorDropdown : Dropdown {

        private const int COLOR_SIZE = 9;

        public Color SelectedColor {
            get =>
                _colorItemPairings.TryGetValue(this.SelectedItem, out Color selectedColor)
                    ? selectedColor
                    : Color.Transparent;
            set {
                foreach (KeyValuePair<string, Color> ddPairs in _colorItemPairings) {
                    if (ddPairs.Value == value) {
                        this.SelectedItem = ddPairs.Key;
                        break;
                    }
                }
            }
        }

        private readonly Dictionary<string, Color> _colorItemPairings = new Dictionary<string, Color>();

        public void AddColorItem(Color itemColor, string itemText) {
            _colorItemPairings.Add(itemText, itemColor);

            this.Items.Add(itemText);
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            DrawControlBacking(spriteBatch, bounds);

            DrawSelectedItem(
                             spriteBatch,
                             new Rectangle(
                                           bounds.X + COLOR_SIZE * 2,
                                           bounds.Y,
                                           bounds.Width - COLOR_SIZE * 2,
                                           bounds.Height
                                          )
                            );

            if (_colorItemPairings.TryGetValue(this.SelectedItem, out Color selectedColor)) {
                spriteBatch.Draw(
                                 ContentService.Textures.Pixel,
                                 new Rectangle(
                                               8,
                                               this.Height / 2 - COLOR_SIZE / 2,
                                               COLOR_SIZE,
                                               COLOR_SIZE
                                              ),
                                 selectedColor
                                );
            }
        }

        protected override void PaintItemPanelItemText(SpriteBatch spriteBatch, Rectangle bounds, string itemText, Color textColor) {
            base.PaintItemPanelItemText(
                                        spriteBatch,
                                        new Rectangle(
                                                      bounds.X + COLOR_SIZE * 2,
                                                      bounds.Y,
                                                      bounds.Width - COLOR_SIZE * 2,
                                                      bounds.Height
                                                     ),
                                        itemText,
                                        textColor
                                       );

            if (_colorItemPairings.TryGetValue(itemText, out Color selectedColor)) {
                spriteBatch.Draw(
                                 ContentService.Textures.Pixel,
                                 new Rectangle(
                                               8,
                                               bounds.Y + bounds.Height / 2 - COLOR_SIZE / 2,
                                               COLOR_SIZE,
                                               COLOR_SIZE
                                              ),
                                 selectedColor
                                );
            }
        }

        protected override void OnValueChanged(ValueChangedEventArgs e) {
            base.OnValueChanged(e);
            OnPropertyChanged(nameof(this.SelectedColor));
        }

    }

}
