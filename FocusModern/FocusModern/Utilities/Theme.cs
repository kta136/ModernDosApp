using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FocusModern.Utilities
{
    /// <summary>
    /// Lightweight theming utilities to give WinForms a more modern look
    /// without introducing external dependencies. Applies fonts, colors,
    /// ToolStrip renderer, and common grid/button styles.
    /// </summary>
    public static class Theme
    {
        // Color tokens (tweak as desired)
        public static Color Background = Color.FromArgb(245, 247, 250);
        public static Color Surface = Color.White;
        public static Color Border = Color.FromArgb(220, 225, 230);
        public static Color TextPrimary = Color.FromArgb(25, 28, 31);
        public static Color TextSecondary = Color.FromArgb(90, 98, 106);
        public static Color Accent = Color.FromArgb(0, 120, 215); // Windows accent blue
        public static Color AccentHover = Color.FromArgb(0, 99, 177);

        public static Font BaseFont = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);
        public static Font EmphasisFont = new Font("Segoe UI Semibold", 10f, FontStyle.Bold, GraphicsUnit.Point);

        public static void Apply(Form form)
        {
            try
            {
                form.Font = BaseFont;
                if (form.BackColor == SystemColors.Control)
                    form.BackColor = Background;

                // ToolStrips / MenuStrips / StatusStrips
                foreach (var ts in form.Controls.OfType<ToolStrip>().Concat(GetAllChildren(form).OfType<ToolStrip>()))
                {
                    ts.Renderer = new ModernToolStripRenderer();
                    ts.BackColor = Surface;
                    ts.ForeColor = TextPrimary;
                }

                // GroupBoxes & Labels subtitle color
                foreach (var gb in GetAllChildren(form).OfType<GroupBox>())
                {
                    gb.BackColor = Surface;
                    gb.ForeColor = TextSecondary;
                }

                // Buttons
                foreach (var btn in GetAllChildren(form).OfType<Button>())
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Border;
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 240, 245);
                    btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(225, 230, 235);
                    btn.BackColor = Surface;
                    btn.ForeColor = TextPrimary;
                }

                // DataGridViews
                foreach (var grid in GetAllChildren(form).OfType<DataGridView>())
                {
                    ApplyGridStyle(grid);
                }

                // Panels default background
                foreach (var panel in GetAllChildren(form).OfType<Panel>())
                {
                    if (panel.BackColor == SystemColors.Control)
                        panel.BackColor = Surface;
                }
            }
            catch { /* never fail UI due to theming */ }
        }

        public static void ApplyGridStyle(DataGridView grid)
        {
            try
            {
                grid.BackgroundColor = Surface;
                grid.BorderStyle = BorderStyle.None;
                grid.EnableHeadersVisualStyles = false;

                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
                grid.ColumnHeadersDefaultCellStyle.Font = EmphasisFont;
                grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                grid.GridColor = Border;

                grid.DefaultCellStyle.BackColor = Surface;
                grid.DefaultCellStyle.ForeColor = TextPrimary;
                grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(229, 241, 251);
                grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 253);

                // reduce flicker
                SetDoubleBuffered(grid);
            }
            catch { }
        }

        private static void SetDoubleBuffered(Control c)
        {
            try
            {
                var prop = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                prop?.SetValue(c, true, null);
            }
            catch { }
        }

        private static System.Collections.Generic.IEnumerable<Control> GetAllChildren(Control c)
        {
            foreach (Control child in c.Controls)
            {
                yield return child;
                foreach (var grand in GetAllChildren(child))
                    yield return grand;
            }
        }

        private class ModernToolStripRenderer : ToolStripProfessionalRenderer
        {
            public ModernToolStripRenderer() : base(new ModernColorTable()) { }
        }

        private class ModernColorTable : ProfessionalColorTable
        {
            public override Color ToolStripGradientBegin => Surface;
            public override Color ToolStripGradientMiddle => Surface;
            public override Color ToolStripGradientEnd => Surface;
            public override Color MenuStripGradientBegin => Surface;
            public override Color MenuStripGradientEnd => Surface;
            public override Color ImageMarginGradientBegin => Surface;
            public override Color ImageMarginGradientMiddle => Surface;
            public override Color ImageMarginGradientEnd => Surface;
            public override Color MenuItemSelected => Color.FromArgb(229, 241, 251);
            public override Color MenuItemBorder => Accent;
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(240, 244, 248);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(240, 244, 248);
            public override Color ButtonSelectedHighlight => Color.FromArgb(229, 241, 251);
            public override Color ButtonSelectedBorder => Accent;
        }
    }
}

