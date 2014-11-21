using Fetze.WinFormsColor;
using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class ColumnSettings : UserControl
    {
        public String ColumnName { get { return Data.Name; } set { Data.Name = value; } }
        public String Type { get { return GetColumnType(Data.Type); } set { Data.Type = ParseColumnType(value); } }
        public String Comparison { get { return Data.Comparison; } set { Data.Comparison = value; } }
        public String TimingMethod { get { return Data.TimingMethod; } set { Data.TimingMethod = value; } }

        public ColumnData Data { get; set; }
        public LiveSplitState CurrentState { get; set; }
        public IList<ColumnSettings> ColumnsList { get; set; }

        public int ColumnIndex { get { return ColumnsList.IndexOf(this); } }
        public int TotalColumns { get { return ColumnsList.Count; } }

        public event EventHandler ColumnRemoved;
        public event EventHandler MovedUp;
        public event EventHandler MovedDown;

        public ColumnSettings(LiveSplitState state, String columnName, IList<ColumnSettings> columnsList)
        {
            InitializeComponent();

            Data = new ColumnData(columnName, ColumnType.Delta, "Current Comparison", "Current Timing Method");

            CurrentState = state;
            ColumnsList = columnsList;

            txtName.DataBindings.Add("Text", this, "ColumnName", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbColumnType.DataBindings.Add("SelectedItem", this, "Type", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbComparison.DataBindings.Add("SelectedItem", this, "Comparison", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbTimingMethod.DataBindings.Add("SelectedItem", this, "TimingMethod", false, DataSourceUpdateMode.OnPropertyChanged);

            txtName.TextChanged += txtName_TextChanged;
            this.Load += ColumnSettings_Load;
        }

        void ColumnSettings_Load(object sender, EventArgs e)
        {
            cmbComparison.Items.Add("Current Comparison");
            cmbComparison.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x != BestSplitTimesComparisonGenerator.ComparisonName && x != NoneComparisonGenerator.ComparisonName).ToArray());
            if (!cmbComparison.Items.Contains(Comparison))
                cmbComparison.Items.Add(Comparison);
        }

        public void UpdateEnabledButtons()
        {
            btnMoveDown.Enabled = ColumnIndex < TotalColumns - 1;
            btnMoveUp.Enabled = ColumnIndex > 0;
        }

        void txtName_TextChanged(object sender, EventArgs e)
        {
            groupColumn.Text = txtName.Text;
        }

        private String GetColumnType(ColumnType type)
        {
            if (type == ColumnType.Delta)
                return "Delta";
            else if (type == ColumnType.SegmentDelta)
                return "Segment Delta";
            else if (type == ColumnType.SegmentTime)
                return "Segment Time";
            return "Split Time";
        }

        private ColumnType ParseColumnType(String columnType)
        {
            return (ColumnType)Enum.Parse(typeof(ColumnType), columnType.Replace(" ", String.Empty));
        }

        private void btnRemoveColumn_Click(object sender, EventArgs e)
        {
            if (ColumnRemoved != null)
                ColumnRemoved(this, null);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (MovedUp != null)
                MovedUp(this, null);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (MovedDown != null)
                MovedDown(this, null);
        }

        public void SelectControl()
        {
            btnRemoveColumn.Select();
        }
    }
}
