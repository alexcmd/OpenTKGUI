﻿using System;
using System.Collections.Generic;

using OpenTK.Input;

namespace OpenTKGUI
{
    /// <summary>
    /// Represents an alignment of a text in a direction.
    /// </summary>
    public enum TextAlign
    {
        Left = 0,
        Right = 1,
        Top = 0,
        Bottom = 1,
        Center = 2
    }

    /// <summary>
    /// Represents a method of wrapping text that is too large for its container.
    /// </summary>
    public enum TextWrap
    {
        /// <summary>
        /// Text that does not fit in its line is cut.
        /// </summary>
        Clip,

        /// <summary>
        /// Text that does not fit will be replaced with an ellipsis.
        /// </summary>
        Ellipsis,

        /// <summary>
        /// If text does not fit on a line, it is cut and moved to the next line.
        /// </summary>
        Wrap
    }

    /// <summary>
    /// A control containing text (with no background).
    /// </summary>
    public class Label : Control
    {
        public Label(string Text)
            : this(Text, Color.RGB(0.0, 0.0, 0.0), new LabelStyle())
        {

        }

        public Label(string Text, Color Color)
            : this(Text, Color, new LabelStyle())
        {

        }

        public Label(string Text, Color Color, LabelStyle Style)
        {
            this._Text = Text;
            this._Color = Color;
            this._Style = Style;
        }

        /// <summary>
        /// Gets or sets the text on the label.
        /// </summary>
        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                this._Text = value;
                if (this._Sample != null)
                {
                    this._Sample.Dispose();
                    this._Sample = null;
                }
            }
        }

        /// <summary>
        /// Gets the wrapping mode of this label.
        /// </summary>
        public TextWrap Wrap
        {
            get
            {
                return this._Style.Wrap;
            }
        }

        /// <summary>
        /// Gets the horizontal align mode on the label.
        /// </summary>
        public TextAlign HorizontalAlign
        {
            get
            {
                return this._Style.HorizontalAlign;
            }
        }

        /// <summary>
        /// Gets the vertical align mode on the label.
        /// </summary>
        public TextAlign VerticalAlign
        {
            get
            {
                return this._Style.VerticalAlign;
            }
        }

        /// <summary>
        /// Suggests the size of the label so that no text overflows.
        /// </summary>
        public Point SuggestSize
        {
            get
            {
                TextSample test = this._Style.Font.CreateSample(this.Text);
                Point size = test.Size;
                test.Dispose();
                return size;
            }
        }

        /// <summary>
        /// Gets the height required for the label if the width is known.
        /// </summary>
        public double GetHeight(double Width)
        {
            TextSample test = this._Style.Font.CreateSample(this.Text, new Point(Width, 4096.0), this._Style.HorizontalAlign, this._Style.VerticalAlign, this._Style.Wrap);
            double height = test.Size.Y;
            test.Dispose();
            return Math.Ceiling(height);
        }

        public override void Render(GUIRenderContext Context)
        {
            if (this._Text != null && this._Text != "")
            {
                if (this._Sample == null)
                {
                    this._CreateSample();
                }
                Context.DrawText(this._Color, this._Sample, new Rectangle(this.Size));
            }
        }

        protected override void OnResize(Point Size)
        {
            this._Sample = null;
        }

        private void _CreateSample()
        {
            Font font = this._Style.Font;
            string text = this._Text;
            this._Sample = font.CreateSample(text, this.Size, this._Style.HorizontalAlign, this._Style.VerticalAlign, this._Style.Wrap);
        }

        protected override void OnDispose()
        {
            if (this._Sample != null)
            {
                this._Sample.Dispose();
            }
        }

        private LabelStyle _Style;
        private Color _Color;
        private string _Text;
        private TextSample _Sample;
    }

    /// <summary>
    /// Gives styling options for a label.
    /// </summary>
    public class LabelStyle
    {
        public TextAlign HorizontalAlign = TextAlign.Left;
        public TextAlign VerticalAlign = TextAlign.Top;
        public TextWrap Wrap = TextWrap.Wrap;
        public Font Font = Font.Default;
    }
}