using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Rhino.Geometry;

namespace Angelfish
{
    public class AttributePreview : GH_ResizableAttributes<GhpPreview>
    //public class AttributePreview : GH_Attributes<GhpPreview>

    {
        int padding = 6;
        int originalSize = 100; 
        int displaySize;

        public AttributePreview(GhpPreview owner) : base(owner)
        {
            Bounds = new Rectangle(0, 0, (int)(originalSize + (padding * 2)), (int)(originalSize + (padding * 4)));
        }

        protected override Size MinimumSize => new Size(originalSize+ padding+padding, originalSize + padding *4);
        protected override Padding SizingBorders => new Padding(padding);

        public override bool HasOutputGrip => false;
        protected override void Layout()
        {
            base.Layout();

            //Rectangle r = GH_Convert.ToRectangle(this.Bounds);
            //r.Height = (int)(100 + (padding * 4.5));
            //r.Width = (int)(100 + (padding * 2));
            //this.Bounds = r;
        }

        public override void ExpireLayout()
        {
            base.ExpireLayout();
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Wires) RenderIncomingWires(canvas.Painter, Owner.Sources, Owner.WireDisplay);


            if (channel == GH_CanvasChannel.Objects)
            {
                base.Render(canvas, graphics, channel);

                GH_Palette palette = GH_Palette.Normal;

                switch (Owner.RuntimeMessageLevel)
                {
                    case GH_RuntimeMessageLevel.Warning:
                        palette = GH_Palette.Warning;
                        break;

                    case GH_RuntimeMessageLevel.Error:
                        palette = GH_Palette.Error;
                        break;
                }

                GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, palette);
                capsule.AddInputGrip(InputGrip.Y);
                capsule.Render(graphics, Selected, Owner.Locked, true);

                capsule.Dispose();

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;

                RectangleF textRectangle = Bounds;
                textRectangle.Height = 20;

                Random rand = new Random();
                DateTime dT = DateTime.Now;
                //graphics.DrawString(Owner.NickName, GH_FontServer.Standard, Brushes.Black, textRectangle, format);

                string name = dT.ToString("yyyyMMddHHmmss") + rand.Next(0,999).ToString();
                graphics.DrawString(name, GH_FontServer.Standard, Brushes.Black, textRectangle, format);

                int displayWidth = (int)(Bounds.Width - padding * 2);
                int displayHeight = (int)(Bounds.Height - padding * 4);

                System.Drawing.Point position = new System.Drawing.Point((int)(Bounds.X + padding), (int)(Bounds.Y + padding * 3));
                Rectangle PatternRectangle = new Rectangle(position.X, position.Y, displayWidth, displayHeight);
                graphics.DrawRectangle(new Pen(Color.White), PatternRectangle);

                IGH_Structure data = Owner.VolatileData;
                if (data.PathCount != 0)
                {
                    GH_Path path = data.get_Path(0);
                    List<GH_Number> branch = data.get_Branch(path) as List<GH_Number>;

                    int patternSize = branch.Count;
                    float width = (float)Math.Ceiling(Math.Sqrt((double)patternSize));

                    if (displayWidth > displayHeight) displaySize = displayHeight;
                    else displaySize = displayWidth;
                    
                    float rectSize = (float)(displaySize / width);

                    RectangleF[] rectangles = new RectangleF[patternSize];

                    for (int i = 0; i < patternSize; i++)
                    {
                        PointF pos = new PointF((float)(Bounds.X + padding), (float)(Bounds.Y + padding * 3));
                        pos = new PointF(pos.X + rectSize * (float)Math.Floor(i % width), pos.Y + rectSize * (float)Math.Floor(i / width));
                        RectangleF content = new RectangleF(pos.X, pos.Y, rectSize, rectSize);
                        rectangles[i] = content;
                        SolidBrush brush;
                        
                        if((int)branch[i].Value == 0) brush = new SolidBrush(Color.White);
                        else if((int)branch[i].Value == 1) brush = new SolidBrush(Color.Black);
                        else brush = new SolidBrush(Color.Orange);

                        graphics.FillRectangle(brush, rectangles[i]);
                    }
                }

                format.Dispose();
            }
        }
    }
}