﻿using System;
using System.Collections.Generic;

namespace OpenTKGUI
{
    /// <summary>
    /// Contains function related to the creation and display of modal message boxes.
    /// </summary>
    public static class MessageBox
    {
        /// <summary>
        /// Shows a messagebox with the specified options.
        /// </summary>
        public static void Show(LayerContainer LayerContainer, MessageBoxOptions Options)
        {
            MessageBoxStyle style = Options.Style;

            // Message
            Label message = new Label(Options.Message, style.MessageColor, style.MessageLabelStyle);

            ClickHandler anyclick = null;

            // Buttons
            FlowContainer buttonflow = new FlowContainer(style.ButtonSeperation, Axis.Horizontal);
            ButtonStyle bstyle = style.ButtonStyle;
            foreach (MessageBoxOptions._Button b in Options._Buttons)
            {
                string name = b.Name;
                Label label = new Label(name, bstyle.TextColor, bstyle.TextStyle);
                Button button = new Button(bstyle);
                button.Client = label;
                button.Click += b.Click;
                button.Click += delegate { anyclick(); };
                buttonflow.AddChild(button, button.GetFullSize(label.SuggestSize).X);
            }

            // Main flow container
            FlowContainer mainflow = new FlowContainer(style.MessageButtonSeperation, Axis.Vertical);
            mainflow.AddChild(message, message.GetHeight(style.ContentWidth));
            mainflow.AddChild(buttonflow.WithCenterAlign(new Point(buttonflow.SuggestLength, style.ButtonHeight)), style.ButtonHeight);

            // Margin and border
            MarginContainer margin = mainflow.WithMargin(style.Margin);
            Point finalsize = margin.GetSize(new Point(style.ContentWidth, mainflow.SuggestLength));
            Control final = margin;
            if (style.BorderSize > 0.0)
            {
                double bs = style.BorderSize;
                final = final.WithBorder(style.BorderColor, bs, bs, bs, bs);
                finalsize += new Point(bs, bs) * 2.0;
            }

            // Form (finally)
            Form form = new Form(final, Options.Title);
            form.ClientSize = finalsize;
            LayerContainer.AddControl(form, LayerContainer.Size * 0.5 - form.Size * 0.5);

            // Make it modal
            ModalOptions mo = new ModalOptions()
            {
                Lightbox = true,
                LowestModal = form,
                MouseFallthrough = false
            };
            LayerContainer.Modal = mo;

            // Create destruction procedure.
            anyclick = delegate
            {
                LayerContainer.Modal = null;
                form.Dismiss();
            };
        }

        /// <summary>
        /// Shows a messagebox with the OK and cancel options. Action is only performed on OK.
        /// </summary>
        public static void ShowOKCancel(LayerContainer Container, string Title, string Message, ClickHandler OnOKClick, MessageBoxStyle Style)
        {
            MessageBoxOptions mbo = new MessageBoxOptions();
            mbo.AddButton("OK", OnOKClick);
            mbo.AddButton("Cancel", null);
            mbo.Title = Title;
            mbo.Message = Message;
            mbo.Style = Style;
            Show(Container, mbo);
        }

        /// <summary>
        /// Shows a messagebox with the default style with the OK and cancel options. Action is only performed on OK. 
        /// </summary>
        public static void ShowOKCancel(LayerContainer Container, string Title, string Message, ClickHandler OnOKClick)
        {
            ShowOKCancel(Container, Title, Message, OnOKClick, new MessageBoxStyle());
        }
    }

    /// <summary>
    /// Options for showing a message box.
    /// </summary>
    public class MessageBoxOptions
    {
        public MessageBoxOptions()
        {
            this._Buttons = new List<_Button>();
            this._Style = new MessageBoxStyle();
        }

        /// <summary>
        /// Gets or sets the style of the message box.
        /// </summary>
        public MessageBoxStyle Style
        {
            get
            {
                return this._Style;
            }
            set
            {
                this._Style = value;
            }
        }

        /// <summary>
        /// Gets or sets the title of the message box.
        /// </summary>
        public string Title
        {
            get
            {
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }

        /// <summary>
        /// Gets or sets the message of the message box.
        /// </summary>
        public string Message
        {
            get
            {
                return this._Message;
            }
            set
            {
                this._Message = value;
            }
        }

        /// <summary>
        /// Adds a button to the message box.
        /// </summary>
        public void AddButton(string Name, ClickHandler OnClick)
        {
            this._Buttons.Add(new _Button()
            {
                Name = Name,
                Click = OnClick
            });
        }

        internal class _Button
        {
            public string Name;
            public ClickHandler Click;
        }

        private string _Title;
        private string _Message;
        private MessageBoxStyle _Style;
        internal List<_Button> _Buttons;
    }

    /// <summary>
    /// Gives styling options for a message box.
    /// </summary>
    public class MessageBoxStyle
    {
        public ButtonStyle ButtonStyle = new ButtonStyle();
        public FormStyle FormStyle = new FormStyle();
        public LabelStyle MessageLabelStyle = new LabelStyle()
        {
            HorizontalAlign = TextAlign.Center,
            VerticalAlign = TextAlign.Center,
            Wrap = TextWrap.Wrap,
        };
        public Color BorderColor = Color.RGB(0.0, 0.0, 0.0);
        public double BorderSize = 1.0;
        public Color MessageColor = Color.RGB(0.0, 0.0, 0.0);
        public double Margin = 20.0;
        public double MessageButtonSeperation = 10.0;
        public double ButtonSeperation = 5.0;
        public double ButtonHeight = 30.0;
        public double ContentWidth = 400.0;
    }
}