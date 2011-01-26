﻿using System;
using System.Collections.Generic;

namespace OpenTKGUI
{
    /// <summary>
    /// A container that allows hovering controls (such as context menus or forms) to be placed over a
    /// background control.
    /// </summary>
    public class LayerContainer : Control
    {
        public LayerContainer()
        {
            this._LayerControls = new LinkedList<LayerControl>();
        }

        public LayerContainer(Control Background)
            : this()
        {
            this._Background = Background;
        }

        /// <summary>
        /// Gets or sets the style default shadows are rendered with.
        /// </summary>
        public ShadowStyle ShadowStyle
        {
            get
            {
                return this._ShadowStyle;
            }
            set
            {
                this._ShadowStyle = value;
            }
        }

        /// <summary>
        /// Adds a hovering layer control to the top of the layer container (above all other controls). The added control must not
        /// be used by any other layer containers.
        /// </summary>
        public void AddControl(LayerControl Control, Point Position)
        {
            if (Control._Container == null)
            {
                Control._Container = this;
                Control._Position = Position;
                this._LayerControls.AddLast(Control);
            }
        }

        /// <summary>
        /// Removes a hovering layer control from this container.
        /// </summary>
        public void RemoveControl(LayerControl Control)
        {
            if (Control._Container == this)
            {
                Control._Container = null;
                this._LayerControls.Remove(Control);
            }
        }

        /// <summary>
        /// Brings the specified control to the top level of the layer container (above all other controls).
        /// </summary>
        public void BringToTop(LayerControl Control)
        {
            this._LayerControls.Remove(Control);
            this._LayerControls.AddLast(Control);
        }

        public override void Render(GUIRenderContext Context)
        {
            if (this._Background != null)
            {
                this._Background.Render(Context);
            }
            foreach (LayerControl lc in this._LayerControls)
            {
                lc.RenderShadow(lc._Position, Context);
                Context.PushTranslate(lc._Position);
                lc.Render(Context);
                Context.Pop();
            }
        }

        public override void Update(GUIControlContext Context, double Time)
        {
            MouseState ms = Context.MouseState;
            Point? mousepos = ms != null ? (Point?)ms.Position : null;
            LinkedList<LayerControl> oldlayercontrols = this._LayerControls;
            this._LayerControls = new LinkedList<LayerControl>(this._LayerControls);


            // Go through control in reverse order (since the last control is the top-most).
            LinkedListNode<LayerControl> cur = oldlayercontrols.Last;
            while (cur != null) 
            {
                LayerControl lc = cur.Value;
                lc.Update(Context.CreateChildContext(lc, lc._Position, mousepos == null), Time);
                if (mousepos != null)
                {
                    if (new Rectangle(lc._Position, lc.Size).In(mousepos.Value))
                    {
                        mousepos = null;
                    }
                }
                cur = cur.Previous;
            }


            if (this._Background != null)
            {
                this._Background.Update(Context.CreateChildContext(this._Background, new Point(), mousepos == null), Time);
            }
        }

        protected override void OnResize(Point Size)
        {
            if (this._Background != null)
            {
                this.ResizeChild(this._Background, Size);
            }
        }

        /// <summary>
        /// Gets or sets the background control. The background control will always be behind all visible hovering controls.
        /// </summary>
        public Control Background
        {
            get
            {
                return this._Background;
            }
            set
            {
                this._Background = value;
                this.ResizeChild(this._Background, this.Size);
            }
        }

        private ShadowStyle _ShadowStyle = new ShadowStyle();
        private Control _Background;
        private LinkedList<LayerControl> _LayerControls;
    }

    /// <summary>
    /// A hovering control showing in the front layer of a layer container.
    /// </summary>
    public class LayerControl : Control
    {
        /// <summary>
        /// Gets or sets the size of the layer control. This size is independant of any other control.
        /// </summary>
        public new Point Size
        {
            get
            {
                return base.Size;
            }
            protected set
            {
                base.ForceResize(value);
            }
        }

        /// <summary>
        /// Gets the layer container this hovering control is in.
        /// </summary>
        public LayerContainer Container
        {
            get
            {
                return this._Container;
            }
        }

        /// <summary>
        /// Gets or sets the position of the layer control within its container.
        /// </summary>
        public Point Position
        {
            get
            {
                return this._Position;
            }
            set
            {
                this._Position = value;
            }
        }

        /// <summary>
        /// Renders a shadow for the hovering control. The context given will not be translated or clipped from
        /// the layer container.
        /// </summary>
        public virtual void RenderShadow(Point Position, GUIRenderContext Context)
        {
            ShadowStyle ss = this._Container.ShadowStyle;
            double width = ss.Width;
            Context.DrawSurface(ss.Skin.GetSurface(ss.Image, this.Size + new Point(width * 2.0, width * 2.0)), Position - new Point(width, width));
        }

        internal LayerContainer _Container;
        internal Point _Position;
    }

    /// <summary>
    /// Gives styling options for the default shadow on hovering controls.
    /// </summary>
    public class ShadowStyle
    {
        public Skin Skin = Skin.Default;
        public SkinArea Image = new SkinArea(112, 32, 16, 16);
        public double Width = 6.0;
    }
}