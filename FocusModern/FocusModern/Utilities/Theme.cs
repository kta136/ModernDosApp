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
        // Modern color palette inspired by Microsoft Fluent Design
        public static Color Background = Color.FromArgb(248, 249, 250);       // Light neutral background
        public static Color Surface = Color.White;                            // Pure white for cards/panels
        public static Color SurfaceVariant = Color.FromArgb(252, 253, 254);   // Slight variant for alternating rows
        public static Color Border = Color.FromArgb(227, 230, 232);           // Subtle borders
        public static Color BorderLight = Color.FromArgb(241, 243, 244);      // Even lighter borders
        public static Color TextPrimary = Color.FromArgb(32, 33, 36);         // Dark text for high contrast
        public static Color TextSecondary = Color.FromArgb(95, 99, 104);      // Medium gray for secondary text
        public static Color TextTertiary = Color.FromArgb(154, 160, 166);     // Light gray for hints/labels
        
        // Accent colors
        public static Color Primary = Color.FromArgb(26, 115, 232);           // Google Blue - primary actions
        public static Color PrimaryHover = Color.FromArgb(23, 78, 166);       // Darker blue for hover
        public static Color PrimaryLight = Color.FromArgb(232, 240, 254);     // Light blue for selected items
        
        // Semantic colors
        public static Color Success = Color.FromArgb(34, 139, 34);            // Green for success
        public static Color Warning = Color.FromArgb(255, 159, 10);           // Orange for warnings  
        public static Color Error = Color.FromArgb(217, 48, 37);              // Red for errors
        public static Color Info = Color.FromArgb(66, 133, 244);              // Blue for info
        
        // Typography
        public static Font BaseFont = new Font("Segoe UI", 9.75f, FontStyle.Regular, GraphicsUnit.Point);
        public static Font EmphasisFont = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold, GraphicsUnit.Point);
        public static Font LargeFont = new Font("Segoe UI", 11f, FontStyle.Regular, GraphicsUnit.Point);
        public static Font HeaderFont = new Font("Segoe UI Semibold", 12f, FontStyle.Bold, GraphicsUnit.Point);
        public static Font TitleFont = new Font("Segoe UI", 16f, FontStyle.Regular, GraphicsUnit.Point);
        
        // Spacing
        public static int SpacingXS = 4;
        public static int SpacingS = 8;
        public static int SpacingM = 16;
        public static int SpacingL = 24;
        public static int SpacingXL = 32;

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
                    ts.Font = BaseFont;
                }

                // GroupBoxes with modern styling
                foreach (var gb in GetAllChildren(form).OfType<GroupBox>())
                {
                    gb.BackColor = Background;
                    gb.ForeColor = TextSecondary;
                    gb.Font = EmphasisFont;
                    gb.FlatStyle = FlatStyle.Flat;
                }

                // Buttons with modern flat design
                foreach (var btn in GetAllChildren(form).OfType<Button>())
                {
                    ApplyButtonStyle(btn);
                }

                // DataGridViews
                foreach (var grid in GetAllChildren(form).OfType<DataGridView>())
                {
                    ApplyGridStyle(grid);
                }

                // Panels with consistent backgrounds
                foreach (var panel in GetAllChildren(form).OfType<Panel>())
                {
                    if (panel.BackColor == SystemColors.Control || panel.BackColor.Name.StartsWith("Light"))
                        panel.BackColor = Background;
                }

                // Labels with consistent colors
                foreach (var label in GetAllChildren(form).OfType<Label>())
                {
                    if (label.ForeColor == SystemColors.ControlText)
                        label.ForeColor = TextPrimary;
                }

                // TextBoxes and ComboBoxes
                foreach (var tb in GetAllChildren(form).OfType<TextBox>())
                {
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    tb.BackColor = Surface;
                    tb.ForeColor = TextPrimary;
                }

                foreach (var cb in GetAllChildren(form).OfType<ComboBox>())
                {
                    cb.BackColor = Surface;
                    cb.ForeColor = TextPrimary;
                    cb.FlatStyle = FlatStyle.Flat;
                }
            }
            catch { /* never fail UI due to theming */ }
        }

        public static void ApplyButtonStyle(Button btn, bool isPrimary = false)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = BaseFont;
            
            if (isPrimary)
            {
                btn.BackColor = Primary;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = Primary;
                btn.FlatAppearance.MouseOverBackColor = PrimaryHover;
                btn.FlatAppearance.MouseDownBackColor = PrimaryHover;
                btn.FlatAppearance.BorderSize = 1;
            }
            else
            {
                btn.BackColor = Surface;
                btn.ForeColor = TextPrimary;
                btn.FlatAppearance.BorderColor = Border;
                btn.FlatAppearance.MouseOverBackColor = SurfaceVariant;
                btn.FlatAppearance.MouseDownBackColor = BorderLight;
                btn.FlatAppearance.BorderSize = 1;
            }

            // Add some padding
            btn.Padding = new Padding(SpacingS, SpacingXS, SpacingS, SpacingXS);
            btn.Height = Math.Max(btn.Height, 32); // Minimum height for better touch targets
        }

        public static void ApplyGridStyle(DataGridView grid)
        {
            try
            {
                grid.BackgroundColor = Surface;
                grid.BorderStyle = BorderStyle.None;
                grid.EnableHeadersVisualStyles = false;
                grid.Font = BaseFont;

                // Modern header styling
                grid.ColumnHeadersDefaultCellStyle.BackColor = SurfaceVariant;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = TextSecondary;
                grid.ColumnHeadersDefaultCellStyle.Font = EmphasisFont;
                grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                grid.ColumnHeadersHeight = 36; // Taller headers for better readability
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(SpacingS, 0, SpacingS, 0);

                // Grid lines and borders
                grid.GridColor = BorderLight;
                grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

                // Cell styling
                grid.DefaultCellStyle.BackColor = Surface;
                grid.DefaultCellStyle.ForeColor = TextPrimary;
                grid.DefaultCellStyle.SelectionBackColor = PrimaryLight;
                grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
                grid.DefaultCellStyle.Padding = new Padding(SpacingS, SpacingXS, SpacingS, SpacingXS);
                grid.RowTemplate.Height = 32; // Consistent row height

                // Alternating row colors
                grid.AlternatingRowsDefaultCellStyle.BackColor = SurfaceVariant;
                grid.AlternatingRowsDefaultCellStyle.ForeColor = TextPrimary;
                grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = PrimaryLight;
                grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextPrimary;

                // Row headers (if visible)
                grid.RowHeadersVisible = false; // Hide row headers for cleaner look
                
                // Selection mode
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.MultiSelect = false;

                // Scrollbars
                grid.ScrollBars = ScrollBars.Both;
                
                // Auto-sizing
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

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
            public override Color MenuItemSelected => PrimaryLight;
            public override Color MenuItemBorder => Primary;
            public override Color MenuItemPressedGradientBegin => SurfaceVariant;
            public override Color MenuItemPressedGradientEnd => SurfaceVariant;
            public override Color ButtonSelectedHighlight => PrimaryLight;
            public override Color ButtonSelectedBorder => Primary;
            public override Color ButtonPressedHighlight => SurfaceVariant;
            public override Color ButtonPressedBorder => Border;
            public override Color ButtonCheckedHighlight => PrimaryLight;
            public override Color ButtonCheckedGradientBegin => PrimaryLight;
            public override Color ButtonCheckedGradientEnd => PrimaryLight;
            public override Color StatusStripGradientBegin => Surface;
            public override Color StatusStripGradientEnd => Surface;
        }
    }
}

