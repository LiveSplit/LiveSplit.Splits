using Fetze.WinFormsColor;
using LiveSplit.Model;
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
        public String Name { get { return Data.Name; } set { Data.Name = value; } }
        public String Type { get { return GetColumnType(Data.Type); } set { Data.Type = ParseColumnType(value); } }
        public String Comparison { get { return Data.Comparison; } set { Data.Comparison = value; } }
        public String TimingMethod { get { return Data.TimingMethod; } set { Data.TimingMethod = value; } }

        public ColumnData Data { get; set; }
        public LiveSplitState CurrentState { get; set; }

        public EventHandler ColumnRemoved;

        public ColumnSettings(LiveSplitState state, String columnName)
        {
            InitializeComponent();

            Data = new ColumnData(columnName, ColumnType.Delta, "Current Comparison", "Current Timing Method");

            CurrentState = state;

            txtName.DataBindings.Add("Text", this, "Name", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbColumnType.DataBindings.Add("Value", this, "Type", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbComparison.DataBindings.Add("Value", this, "Comparison", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbTimingMethod.DataBindings.Add("Value", this, "TimingMethod", false, DataSourceUpdateMode.OnPropertyChanged);

            txtName.TextChanged += txtName_TextChanged;
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
            ColumnRemoved(this, null);
        }
    }
}
