using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace spritesheet
{
   
    public class Buttons
    {
        private readonly Texture2D _atlas;

        private readonly int _frameWidth;
        private readonly int _frameHeight;
        private readonly int _columns;

        // Optional per-frame source rectangles (overrides uniform grid calculation).
        private readonly Dictionary<int, Rectangle>? _customSourceRects;

        private readonly List<Button> _buttons = new();

        public Buttons(Texture2D atlas, int frameWidth, int frameHeight, int columns, Dictionary<int, Rectangle>? customSourceRects = null)
        {
            _atlas = atlas ?? throw new ArgumentNullException(nameof(atlas));
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _columns = columns;
            _customSourceRects = customSourceRects;
        }

        
        public Buttons(Texture2D atlas, int rows, int columns)
        {
            _atlas = atlas ?? throw new ArgumentNullException(nameof(atlas));
            _columns = Math.Max(1, columns);
            int r = Math.Max(1, rows);
            _frameWidth = _atlas.Width / _columns;
            _frameHeight = _atlas.Height / r;
        }

       
        public Buttons(ContentManager content, string assetName = "customButtons", int rows = 10, int columns = 4)
        {
            _atlas = content.Load<Texture2D>(assetName) ?? throw new ArgumentNullException(nameof(assetName));
            _columns = Math.Max(1, columns);
            int r = Math.Max(1, rows);
            _frameWidth = _atlas.Width / _columns;
            _frameHeight = _atlas.Height / r;
        }

        public Button Create(Rectangle bounds, int frameDefault, int frameHovered, int framePressed, Action onClick = null, string text = null)
        {
            var b = new Button(bounds, frameDefault, frameHovered, framePressed, onClick, text);
            _buttons.Add(b);
            return b;
        }

        public void Remove(Button b) => _buttons.Remove(b);

        public void Update(GameTime gameTime, MouseState currentMouse, MouseState previousMouse)
        {
            foreach (var b in _buttons)
                b.Update(gameTime, currentMouse, previousMouse);
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font = null)
        {
            foreach (var b in _buttons)
                b.Draw(spriteBatch, _atlas, _frameWidth, _frameHeight, _columns, font, _customSourceRects);
        }
    }

    // Single button instance
    public class Button
    {
        private enum ButtonState
        {
            IsHovered,
            IsPressed,
            Transition,
            Default
        }

        public Rectangle Bounds;
        public string Text;
        public Action OnClick;
        public bool Enabled = true;

        // Frame indices into the sprite-sheet atlas
        // These should be set so that they correspond to the frame positions in your sheet
        public int FrameDefault = 4;
        public int FrameHovered = 3;
        public int FramePressed = 2;

        // Optional: transition (animated) frames range and duration
        public int TransitionStart = -1; // set to -1 to disable
        public int TransitionEnd = -1;
        public float TransitionDuration = 0.5f; // seconds

        private ButtonState _state = ButtonState.Default;
        private bool _isPressed;
        private float _transitionTimer;

        public Button(Rectangle bounds, int frameDefault, int frameHovered, int framePressed, Action onClick = null, string text = null)
        {
            Bounds = bounds;
            FrameDefault = frameDefault;
            FrameHovered = frameHovered;
            FramePressed = framePressed;
            OnClick = onClick;
            Text = text;
        }

        // Call every frame with the current and previous mouse states
        public void Update(GameTime gameTime, MouseState currentMouse, MouseState previousMouse)
        {
            if (!Enabled)
            {
                _isPressed = false;
                _state = ButtonState.Default;
                return;
            }

            var mousePos = new Point(currentMouse.X, currentMouse.Y);
            bool contains = Bounds.Contains(mousePos);

            // Press detection
            if (contains && currentMouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                _isPressed = true;
                _state = ButtonState.IsPressed;
            }

            // Release -> click
            if (_isPressed && previousMouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && currentMouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                _isPressed = false;
                if (contains)
                {
                    OnClick?.Invoke();
                    // if you want a transition animation on click, set state to Transition and reset timer
                    if (TransitionStart >= 0) { _state = ButtonState.Transition; _transitionTimer = 0f; }
                    else _state = contains ? ButtonState.IsHovered : ButtonState.Default;
                }
            }

            // Hover state when not pressed
            if (!_isPressed)
            {
                _state = contains ? ButtonState.IsHovered : ButtonState.Default;
            }

            // Advance transition timer if needed
            if (_state == ButtonState.Transition && TransitionStart >= 0 && TransitionEnd >= TransitionStart)
            {
                _transitionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_transitionTimer >= TransitionDuration)
                {
                    // End of transition; return to default/hover state
                    _transitionTimer = 0f;
                    _state = Bounds.Contains(new Point(currentMouse.X, currentMouse.Y)) ? ButtonState.IsHovered : ButtonState.Default;
                }
            }
        }

        // Draw the button using the atlas. Provide frame size and atlas columns to compute source rect.
        public void Draw(SpriteBatch spriteBatch, Texture2D atlas, int frameWidth, int frameHeight, int columns, SpriteFont font = null, Dictionary<int, Rectangle>? customSourceRects = null)
        {
            if (atlas == null) throw new ArgumentNullException(nameof(atlas));

            int frameIndex = FrameDefault;
            switch (_state)
            {
                case ButtonState.IsHovered:
                    frameIndex = FrameHovered;
                    break;
                case ButtonState.IsPressed:
                    frameIndex = FramePressed;
                    break;
                case ButtonState.Transition:
                    if (TransitionStart >= 0 && TransitionEnd >= TransitionStart)
                    {
                        int frameCount = TransitionEnd - TransitionStart + 1;
                        if (frameCount <= 0) frameIndex = TransitionStart;
                        else
                        {
                            float t = Math.Clamp(_transitionTimer / Math.Max(0.0001f, TransitionDuration), 0f, 1f);
                            int step = (int)(t * (frameCount - 1));
                            frameIndex = TransitionStart + step;
                        }
                    }
                    else frameIndex = FrameDefault;
                    break;
                default:
                    frameIndex = FrameDefault;
                    break;
            }

            Rectangle src;
            if (customSourceRects != null && customSourceRects.TryGetValue(frameIndex, out var r))
            {
                src = r;
            }
            else
            {
                src = GetSourceRect(frameIndex, frameWidth, frameHeight, columns);
            }
            spriteBatch.Draw(atlas, Bounds, src, Color.White);

            if (font != null && !string.IsNullOrEmpty(Text))
            {
                var textSize = font.MeasureString(Text);
                var textPos = new Vector2(Bounds.Center.X, Bounds.Center.Y) - textSize / 2f;
                spriteBatch.DrawString(font, Text, textPos, Color.Black);
            }
        }

        private static Rectangle GetSourceRect(int frameIndex, int frameWidth, int frameHeight, int columns)
        {
            if (frameIndex < 0) frameIndex = 0;
            int col = frameIndex % Math.Max(1, columns);
            int row = frameIndex / Math.Max(1, columns);
            return new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);
        }
    }
}
