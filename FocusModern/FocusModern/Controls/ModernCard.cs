using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FocusModern.Utilities;

namespace FocusModern.Controls
{
    /// <summary>
    /// Modern card-style panel with rounded corners and subtle shadows
    /// </summary>
    public class ModernCard : Panel
    {
        private int cornerRadius = 8;
        private Color shadowColor = Color.FromArgb(20, 0, 0, 0);
        private bool hasShadow = true;
        private string cardTitle = "";

        [Category("Appearance")]
        [Description("The radius of the card corners")]
        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Whether to show a shadow behind the card")]
        public bool HasShadow
        {
            get => hasShadow;
            set
            {
                hasShadow = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("The title text displayed at the top of the card")]
        public string CardTitle
        {
            get => cardTitle;
            set
            {
                cardTitle = value ?? "";
                Invalidate();
            }
        }

        public ModernCard()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            
            BackColor = Theme.Surface;
            Padding = new Padding(Theme.SpacingM);
            Margin = new Padding(Theme.SpacingS);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            var cardRect = new Rectangle(0, 0, Width - 1, Height - 1);
            
            // Draw shadow if enabled
            if (hasShadow)
            {
                var shadowRect = new Rectangle(2, 2, Width - 2, Height - 2);
                DrawRoundedRectangle(graphics, shadowRect, cornerRadius, shadowColor);
            }
            
            // Draw card background
            DrawRoundedRectangle(graphics, cardRect, cornerRadius, BackColor);
            
            // Draw border
            using (var borderPen = new Pen(Theme.Border, 1))
            {
                DrawRoundedRectangleBorder(graphics, cardRect, cornerRadius, borderPen);
            }
            
            // Draw title if present
            if (!string.IsNullOrEmpty(cardTitle))
            {
                var titleRect = new Rectangle(Theme.SpacingM, Theme.SpacingS, Width - Theme.SpacingM * 2, 24);
                using (var titleBrush = new SolidBrush(Theme.TextSecondary))
                {
                    graphics.DrawString(cardTitle, Theme.EmphasisFont, titleBrush, titleRect);
                }
                
                // Draw separator line
                var lineY = titleRect.Bottom + Theme.SpacingXS;
                using (var linePen = new Pen(Theme.BorderLight, 1))
                {
                    graphics.DrawLine(linePen, Theme.SpacingM, lineY, Width - Theme.SpacingM, lineY);
                }
            }
            
            // Don't call base.OnPaint as we're handling all the drawing
        }

        private void DrawRoundedRectangle(Graphics graphics, Rectangle rect, int radius, Color color)
        {
            using (var brush = new SolidBrush(color))
            {
                using (var path = CreateRoundedRectPath(rect, radius))
                {
                    graphics.FillPath(brush, path);
                }
            }
        }

        private void DrawRoundedRectangleBorder(Graphics graphics, Rectangle rect, int radius, Pen pen)
        {
            using (var path = CreateRoundedRectPath(rect, radius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));
            
            // Top left
            path.AddArc(arc, 180, 90);
            
            // Top right
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            
            // Bottom right
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            
            // Bottom left
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            Invalidate();
        }
    }
    
    /// <summary>
    /// Modern button with rounded corners and hover effects
    /// </summary>
    public class ModernButton : Button
    {
        private int cornerRadius = 6;
        private bool isHovered = false;
        private bool isPrimary = false;

        [Category("Appearance")]
        [Description("The radius of the button corners")]
        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                cornerRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Whether this is a primary action button")]
        public bool IsPrimary
        {
            get => isPrimary;
            set
            {
                isPrimary = value;
                UpdateColors();
                Invalidate();
            }
        }

        public ModernButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.OptimizedDoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = Theme.BaseFont;
            Height = 36;
            Cursor = Cursors.Hand;
            
            UpdateColors();
        }

        private void UpdateColors()
        {
            if (isPrimary)
            {
                BackColor = Theme.Primary;
                ForeColor = Color.White;
            }
            else
            {
                BackColor = Theme.Surface;
                ForeColor = Theme.TextPrimary;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            if (isPrimary)
                BackColor = Theme.PrimaryHover;
            else
                BackColor = Theme.SurfaceVariant;
            
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            UpdateColors();
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            var buttonRect = new Rectangle(0, 0, Width - 1, Height - 1);
            
            // Draw background
            using (var backgroundBrush = new SolidBrush(BackColor))
            {
                using (var path = CreateRoundedRectPath(buttonRect, cornerRadius))
                {
                    graphics.FillPath(backgroundBrush, path);
                }
            }
            
            // Draw border
            var borderColor = isPrimary ? Theme.Primary : Theme.Border;
            using (var borderPen = new Pen(borderColor, 1))
            {
                using (var path = CreateRoundedRectPath(buttonRect, cornerRadius))
                {
                    graphics.DrawPath(borderPen, path);
                }
            }
            
            // Draw text
            var textFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
            TextRenderer.DrawText(graphics, Text, Font, buttonRect, ForeColor, textFlags);
        }

        private GraphicsPath CreateRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));
            
            // Top left
            path.AddArc(arc, 180, 90);
            
            // Top right
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            
            // Bottom right
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            
            // Bottom left
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            
            path.CloseFigure();
            return path;
        }
    }
}