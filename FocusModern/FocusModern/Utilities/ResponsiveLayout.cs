using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace FocusModern.Utilities
{
    /// <summary>
    /// Utility class to make Windows Forms layouts responsive and modern
    /// </summary>
    public static class ResponsiveLayout
    {
        /// <summary>
        /// Apply responsive layout to a form, making it adapt to window size changes
        /// </summary>
        public static void Apply(Form form)
        {
            // Set up proper anchoring and docking
            ConfigureMainLayout(form);
            
            // Handle window resize events
            form.Resize += (s, e) => HandleResize(form);
            form.Load += (s, e) => HandleResize(form);
        }

        private static void ConfigureMainLayout(Form form)
        {
            foreach (Control control in form.Controls)
            {
                ConfigureControl(control, form);
            }
        }

        private static void ConfigureControl(Control control, Form parentForm)
        {
            // Configure based on control type and name
            switch (control)
            {
                case MenuStrip menu:
                    menu.Dock = DockStyle.Top;
                    break;

                case StatusStrip status:
                    status.Dock = DockStyle.Bottom;
                    break;

                case ToolStrip toolbar:
                    toolbar.Dock = DockStyle.Top;
                    break;

                case Panel panel when panel.Name == "pnlDashboard":
                    panel.Dock = DockStyle.Fill;
                    ConfigureDashboardPanel(panel, parentForm);
                    break;

                case GroupBox gb:
                    ConfigureGroupBox(gb, parentForm);
                    break;

                case Button btn:
                    ConfigureButton(btn, parentForm);
                    break;

                case DataGridView grid:
                    ConfigureDataGrid(grid);
                    break;

                case ListView list:
                    ConfigureListView(list);
                    break;
            }

            // Recursively configure child controls
            foreach (Control child in control.Controls)
            {
                ConfigureControl(child, parentForm);
            }
        }

        private static void ConfigureDashboardPanel(Panel dashboard, Form form)
        {
            dashboard.Padding = new Padding(Theme.SpacingM);

            foreach (Control control in dashboard.Controls)
            {
                if (control is GroupBox gb)
                {
                    ConfigureGroupBox(gb, form);
                }
            }
        }

        private static void ConfigureGroupBox(GroupBox gb, Form form)
        {
            // Make group boxes responsive
            gb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            
            if (gb.Name == "gbSummary")
            {
                // Summary stays at top, full width
                gb.Location = new Point(Theme.SpacingM, Theme.SpacingM);
                gb.Width = Math.Max(form.ClientSize.Width - (Theme.SpacingM * 2), 400);
                gb.Height = 120; // Fixed height for summary
                
                // Configure summary labels for responsive layout
                ConfigureSummaryLabels(gb);
            }
            else if (gb.Name == "gbRecentTransactions")
            {
                // Recent transactions fills remaining space
                var summaryHeight = 120 + Theme.SpacingM; // Summary height + margin
                gb.Location = new Point(Theme.SpacingM, summaryHeight + Theme.SpacingM * 2);
                gb.Width = Math.Max(form.ClientSize.Width - (Theme.SpacingM * 2), 400);
                
                // Calculate available height
                var usedHeight = summaryHeight + Theme.SpacingM * 4; // Summary + margins + button area
                var buttonAreaHeight = 50; // Space for bottom buttons
                var statusBarHeight = 25;
                var availableHeight = form.ClientSize.Height - usedHeight - buttonAreaHeight - statusBarHeight;
                gb.Height = Math.Max(availableHeight, 150);
            }
        }

        private static void ConfigureSummaryLabels(GroupBox summaryGroup)
        {
            var labels = summaryGroup.Controls.OfType<Label>().ToList();
            if (labels.Count == 0) return;

            var groupWidth = summaryGroup.Width - (Theme.SpacingM * 2);
            var columnWidth = groupWidth / 3; // 3 columns for the metrics

            for (int i = 0; i < labels.Count; i += 2) // Process in pairs (label + value)
            {
                if (i + 1 >= labels.Count) break;

                var labelControl = labels[i];
                var valueControl = labels[i + 1];

                var column = i / 2;
                var x = Theme.SpacingM + (column * columnWidth);
                
                // Position label and value vertically
                labelControl.Location = new Point(x, Theme.SpacingL);
                labelControl.Font = Theme.BaseFont;
                labelControl.ForeColor = Theme.TextSecondary;
                labelControl.AutoSize = true;

                valueControl.Location = new Point(x, Theme.SpacingL + 25);
                valueControl.Font = Theme.HeaderFont;
                valueControl.AutoSize = true;
            }
        }

        private static void ConfigureButton(Button btn, Form form)
        {
            // Position buttons at the bottom of the form
            if (btn.Name == "btnSwitchBranch" || btn.Name == "btnBackup" || btn.Name == "btnSettings")
            {
                var bottomMargin = 70; // Space from bottom (above status bar)
                btn.Top = form.ClientSize.Height - bottomMargin;
                btn.Height = 36; // Modern button height
                
                // Position buttons horizontally
                if (btn.Name == "btnSwitchBranch")
                {
                    btn.Left = Theme.SpacingM;
                    btn.Width = 120;
                }
                else if (btn.Name == "btnBackup")
                {
                    btn.Left = Theme.SpacingM + 130;
                    btn.Width = 120;
                }
                else if (btn.Name == "btnSettings")
                {
                    btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                    btn.Left = form.ClientSize.Width - 130;
                    btn.Width = 120;
                }
            }
        }

        private static void ConfigureDataGrid(DataGridView grid)
        {
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AllowUserToResizeRows = false;
        }

        private static void ConfigureListView(ListView list)
        {
            list.Dock = DockStyle.Fill;
            list.View = View.Details;
            list.FullRowSelect = true;
            list.GridLines = true;
        }

        private static void HandleResize(Form form)
        {
            try
            {
                // Reconfigure layout on resize
                foreach (Control control in form.Controls)
                {
                    if (control is Panel dashboard && dashboard.Name == "pnlDashboard")
                    {
                        ConfigureDashboardPanel(dashboard, form);
                    }
                    else if (control is Button btn)
                    {
                        ConfigureButton(btn, form);
                    }
                }

                form.Invalidate();
            }
            catch
            {
                // Ignore resize errors
            }
        }

        /// <summary>
        /// Make a DataGridView fully responsive
        /// </summary>
        public static void MakeDataGridResponsive(DataGridView grid, Form parentForm = null)
        {
            if (grid == null) return;

            // Basic responsive settings
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.Dock = DockStyle.Fill;
            
            // Handle parent form resize
            if (parentForm != null)
            {
                parentForm.Resize += (s, e) => 
                {
                    grid.Invalidate();
                    grid.Update();
                };
            }

            // Adjust column widths based on content type
            foreach (DataGridViewColumn col in grid.Columns)
            {
                switch (col.Name.ToLower())
                {
                    case "id":
                    case "code":
                        col.FillWeight = 60; // Narrower for IDs/codes
                        break;
                    case "date":
                    case "createdat":
                    case "updatedat":
                        col.FillWeight = 80; // Medium width for dates
                        break;
                    case "name":
                    case "description":
                    case "address":
                        col.FillWeight = 150; // Wider for text fields
                        break;
                    case "amount":
                    case "balance":
                    case "payment":
                        col.FillWeight = 90; // Medium width for numbers
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                    default:
                        col.FillWeight = 100; // Default width
                        break;
                }
            }
        }
    }
}