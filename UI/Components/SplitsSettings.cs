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
    public partial class SplitsSettings : UserControl
    {
        private int _VisualSplitCount { get; set; }
        public int VisualSplitCount
        {
            get { return _VisualSplitCount; }
            set
            {
                _VisualSplitCount = value;
                var max = Math.Max(0, _VisualSplitCount - (AlwaysShowLastSplit ? 2 : 1));
                if (dmnUpcomingSegments.Value > max)
                    dmnUpcomingSegments.Value = max;
                dmnUpcomingSegments.Maximum = max;
            }
        }
        public Color CurrentSplitTopColor { get; set; }
        public Color CurrentSplitBottomColor { get; set; }
        public int SplitPreviewCount { get; set; }
        public float SplitWidth { get; set; }
        public float SplitHeight { get; set; }
        public float ScaledSplitHeight { get { return SplitHeight * 10f; } set { SplitHeight = value / 10f; } }
        public float IconSize { get; set; }

        public bool Display2Rows { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }

        public ExtendedGradientType BackgroundGradient { get; set; }
        public String GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (ExtendedGradientType)Enum.Parse(typeof(ExtendedGradientType), value); }
        }

        public String Comparison { get; set; }
        public LiveSplitState CurrentState { get; set; }
        public String TimingMethod { get; set; }

        public bool DisplayIcons { get; set; }
        public bool IconShadows { get; set; }
        public bool ShowThinSeparators { get; set; }
        public bool AlwaysShowLastSplit { get; set; }
        public bool ShowSplitTimes { get; set; }
        public bool ShowBlankSplits { get; set; }
        public bool LockLastSplit { get; set; }
        public bool SeparatorLastSplit { get; set; }

        public bool DropDecimals { get; set; }
        public TimeAccuracy DeltasAccuracy { get; set; }

        public bool OverrideDeltasColor { get; set; }
        public Color DeltasColor { get; set; }

        public Color BeforeNamesColor { get; set; }
        public Color CurrentNamesColor { get; set; }
        public Color AfterNamesColor { get; set; }
        public bool OverrideTextColor { get; set; }
        public Color BeforeTimesColor { get; set; }
        public Color CurrentTimesColor { get; set; }
        public Color AfterTimesColor { get; set; }
        public bool OverrideTimesColor { get; set; }

        public TimeAccuracy SplitTimesAccuracy { get; set; }
        public GradientType CurrentSplitGradient { get; set; }
        public String SplitGradientString { get { return CurrentSplitGradient.ToString(); } 
            set { CurrentSplitGradient = (GradientType)Enum.Parse(typeof(GradientType), value); } }

        public event EventHandler SplitLayoutChanged;

        public LayoutMode Mode { get; set; }

        public SplitsSettings()
        {
            InitializeComponent();
            VisualSplitCount = 8;
            SplitPreviewCount = 1;
            DisplayIcons = true;
            IconShadows = true;
            ShowThinSeparators = true;
            AlwaysShowLastSplit = true;
            ShowSplitTimes = true;
            ShowBlankSplits = true;
            LockLastSplit = true;
            SeparatorLastSplit = true;
            SplitTimesAccuracy = TimeAccuracy.Seconds;
            CurrentSplitTopColor = Color.FromArgb(0x43, 0xa2, 0xd9);
            CurrentSplitBottomColor = Color.FromArgb(0x09, 0x4e, 0x76);
            SplitWidth = 20;
            SplitHeight = 6;
            ScaledSplitHeight = 60;
            IconSize = 24f;
            BeforeNamesColor = Color.FromArgb(255, 255, 255);
            CurrentNamesColor = Color.FromArgb(255, 255, 255);
            AfterNamesColor = Color.FromArgb(255, 255, 255);
            OverrideTextColor = false;
            BeforeTimesColor = Color.FromArgb(255, 255, 255);
            CurrentTimesColor = Color.FromArgb(255, 255, 255);
            AfterTimesColor = Color.FromArgb(255, 255, 255);
            OverrideTimesColor = false;
            CurrentSplitGradient = GradientType.Vertical;
            cmbSplitGradient.SelectedIndexChanged += cmbSplitGradient_SelectedIndexChanged;
            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = ExtendedGradientType.Plain;
            DropDecimals = true;
            DeltasAccuracy = TimeAccuracy.Tenths;
            OverrideDeltasColor = false;
            DeltasColor = Color.FromArgb(255, 255, 255);
            Comparison = "Current Comparison";
            TimingMethod = "Current Timing Method";
            Display2Rows = false;

            dmnTotalSegments.DataBindings.Add("Value", this, "VisualSplitCount", false, DataSourceUpdateMode.OnPropertyChanged);
            dmnUpcomingSegments.DataBindings.Add("Value", this, "SplitPreviewCount", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTopColor.DataBindings.Add("BackColor", this, "CurrentSplitTopColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBottomColor.DataBindings.Add("BackColor", this, "CurrentSplitBottomColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBeforeNamesColor.DataBindings.Add("BackColor", this, "BeforeNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnCurrentNamesColor.DataBindings.Add("BackColor", this, "CurrentNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAfterNamesColor.DataBindings.Add("BackColor", this, "AfterNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBeforeTimesColor.DataBindings.Add("BackColor", this, "BeforeTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnCurrentTimesColor.DataBindings.Add("BackColor", this, "CurrentTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAfterTimesColor.DataBindings.Add("BackColor", this, "AfterTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDisplayIcons.DataBindings.Add("Checked", this, "DisplayIcons", false, DataSourceUpdateMode.OnPropertyChanged);
            chkIconShadows.DataBindings.Add("Checked", this, "IconShadows", false, DataSourceUpdateMode.OnPropertyChanged);
            chkThinSeparators.DataBindings.Add("Checked", this, "ShowThinSeparators", false, DataSourceUpdateMode.OnPropertyChanged);
            chkLastSplit.DataBindings.Add("Checked", this, "AlwaysShowLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowTimes.DataBindings.Add("Checked", this, "ShowSplitTimes", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTextColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTimesColor.DataBindings.Add("Checked", this, "OverrideTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowBlankSplits.DataBindings.Add("Checked", this, "ShowBlankSplits", false, DataSourceUpdateMode.OnPropertyChanged);
            chkLockLastSplit.DataBindings.Add("Checked", this, "LockLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSeparatorLastSplit.DataBindings.Add("Checked", this, "SeparatorLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDropDecimals.DataBindings.Add("Checked", this, "DropDecimals", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideDeltaColor.DataBindings.Add("Checked", this, "OverrideDeltasColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnDeltaColor.DataBindings.Add("BackColor", this, "DeltasColor", false, DataSourceUpdateMode.OnPropertyChanged);
            this.Load += SplitsSettings_Load;
            chkThinSeparators.CheckedChanged += chkThinSeparators_CheckedChanged;
            chkLastSplit.CheckedChanged += chkLastSplit_CheckedChanged;
            chkShowBlankSplits.CheckedChanged += chkShowBlankSplits_CheckedChanged;
            chkLockLastSplit.CheckedChanged += chkLockLastSplit_CheckedChanged;
            chkSeparatorLastSplit.CheckedChanged += chkSeparatorLastSplit_CheckedChanged;
            trkIconSize.DataBindings.Add("Value", this, "IconSize", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbSplitGradient.DataBindings.Add("SelectedItem", this, "SplitGradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbSplitGradient.SelectedIndexChanged += cmbSplitGradient_SelectedIndexChanged;
            cmbComparison.SelectedIndexChanged += cmbComparison_SelectedIndexChanged;
            cmbComparison.DataBindings.Add("SelectedItem", this, "Comparison", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbTimingMethod.DataBindings.Add("SelectedItem", this, "TimingMethod", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbTimingMethod.SelectedIndexChanged += cmbTimingMethod_SelectedIndexChanged;

            cmbGradientType.SelectedIndexChanged += cmbGradientType_SelectedIndexChanged;
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);

            rdoSeconds.CheckedChanged += rdoSeconds_CheckedChanged;
            rdoTenths.CheckedChanged += rdoTenths_CheckedChanged;

            rdoDeltaSeconds.CheckedChanged += rdoDeltaSeconds_CheckedChanged;
            rdoDeltaTenths.CheckedChanged += rdoDeltaTenths_CheckedChanged;

            chkOverrideTextColor.CheckedChanged += chkOverrideTextColor_CheckedChanged;
            chkOverrideDeltaColor.CheckedChanged += chkOverrideDeltaColor_CheckedChanged;
            chkOverrideTimesColor.CheckedChanged += chkOverrideTimesColor_CheckedChanged;
            chkDisplayIcons.CheckedChanged += chkDisplayIcons_CheckedChanged;
        }

        void chkDisplayIcons_CheckedChanged(object sender, EventArgs e)
        {
            trkIconSize.Enabled = label5.Enabled = chkIconShadows.Enabled = chkDisplayIcons.Checked;
        }

        void chkOverrideTimesColor_CheckedChanged(object sender, EventArgs e)
        {
            label6.Enabled = label9.Enabled = label7.Enabled = btnBeforeTimesColor.Enabled
                = btnCurrentTimesColor.Enabled = btnAfterTimesColor.Enabled = chkOverrideTimesColor.Checked;
        }

        void chkOverrideDeltaColor_CheckedChanged(object sender, EventArgs e)
        {
            label8.Enabled = btnDeltaColor.Enabled = chkOverrideDeltaColor.Checked;
        }

        void chkOverrideTextColor_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = label10.Enabled = label13.Enabled = btnBeforeNamesColor.Enabled
            = btnCurrentNamesColor.Enabled = btnAfterNamesColor.Enabled = chkOverrideTextColor.Checked;
        }
        void cmbComparison_SelectedIndexChanged(object sender, EventArgs e)
        {
            Comparison = cmbComparison.SelectedItem.ToString();
        }
        void cmbTimingMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimingMethod = cmbTimingMethod.SelectedItem.ToString();
        }

        void rdoDeltaTenths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDeltaAccuracy();
        }

        void rdoDeltaSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDeltaAccuracy();
        }

        void chkSeparatorLastSplit_CheckedChanged(object sender, EventArgs e)
        {
            SeparatorLastSplit = chkSeparatorLastSplit.Checked;
            SplitLayoutChanged(this, null);
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        void cmbSplitGradient_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnTopColor.Visible = cmbSplitGradient.SelectedItem.ToString() != "Plain";
            btnBottomColor.DataBindings.Clear();
            btnBottomColor.DataBindings.Add("BackColor", this, btnTopColor.Visible ? "CurrentSplitBottomColor" : "CurrentSplitTopCOlor", false, DataSourceUpdateMode.OnPropertyChanged);
            SplitGradientString = cmbSplitGradient.SelectedItem.ToString();
        }

        void chkLockLastSplit_CheckedChanged(object sender, EventArgs e)
        {
            LockLastSplit = chkLockLastSplit.Checked;
            SplitLayoutChanged(this, null);
        }

        void chkShowBlankSplits_CheckedChanged(object sender, EventArgs e)
        {
            ShowBlankSplits = chkLockLastSplit.Enabled = chkShowBlankSplits.Checked;
            SplitLayoutChanged(this, null);
        }

        void rdoTenths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        void rdoSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        void UpdateAccuracy()
        {
            if (rdoSeconds.Checked)
                SplitTimesAccuracy = TimeAccuracy.Seconds;
            else if (rdoTenths.Checked)
                SplitTimesAccuracy = TimeAccuracy.Tenths;
            else
                SplitTimesAccuracy = TimeAccuracy.Hundredths;
        }

        void UpdateDeltaAccuracy()
        {
            if (rdoDeltaSeconds.Checked)
                DeltasAccuracy = TimeAccuracy.Seconds;
            else if (rdoDeltaTenths.Checked)
                DeltasAccuracy = TimeAccuracy.Tenths;
            else
                DeltasAccuracy = TimeAccuracy.Hundredths;
        }

        void chkLastSplit_CheckedChanged(object sender, EventArgs e)
        {
            AlwaysShowLastSplit = chkLastSplit.Checked;
            VisualSplitCount = VisualSplitCount;
            SplitLayoutChanged(this, null);
        }

        void chkThinSeparators_CheckedChanged(object sender, EventArgs e)
        {
            ShowThinSeparators = chkThinSeparators.Checked;
            SplitLayoutChanged(this, null);
        }

        void SplitsSettings_Load(object sender, EventArgs e)
        {
            chkOverrideDeltaColor_CheckedChanged(null, null);
            chkOverrideTextColor_CheckedChanged(null, null);
            chkOverrideTimesColor_CheckedChanged(null, null);
            chkDisplayIcons_CheckedChanged(null, null);
            chkLockLastSplit.Enabled = chkShowBlankSplits.Checked;
            cmbComparison.Items.Clear();
            cmbComparison.Items.Add("Current Comparison");
            cmbComparison.Items.AddRange(CurrentState.Run.Comparisons.Where(x => x != NoneComparisonGenerator.ComparisonName).ToArray());
            if (!cmbComparison.Items.Contains(Comparison))
                cmbComparison.Items.Add(Comparison);

            rdoSeconds.Checked = SplitTimesAccuracy == TimeAccuracy.Seconds;
            rdoTenths.Checked = SplitTimesAccuracy == TimeAccuracy.Tenths;
            rdoHundredths.Checked = SplitTimesAccuracy == TimeAccuracy.Hundredths;

            rdoDeltaSeconds.Checked = DeltasAccuracy == TimeAccuracy.Seconds;
            rdoDeltaTenths.Checked = DeltasAccuracy == TimeAccuracy.Tenths;
            rdoDeltaHundredths.Checked = DeltasAccuracy == TimeAccuracy.Hundredths;

            if (Mode == LayoutMode.Horizontal)
            {
                trkSize.DataBindings.Clear();
                trkSize.Minimum = 5;
                trkSize.Maximum = 120;
                SplitWidth = Math.Min(Math.Max(trkSize.Minimum, SplitWidth), trkSize.Maximum);
                trkSize.DataBindings.Add("Value", this, "SplitWidth", false, DataSourceUpdateMode.OnPropertyChanged);
                lblSplitSize.Text = "Split Width:";
                chkDisplayRows.Enabled = false;
                chkDisplayRows.DataBindings.Clear();
                chkDisplayRows.Checked = true;
            }
            else
            {
                trkSize.DataBindings.Clear();
                trkSize.Minimum = 0;
                trkSize.Maximum = 250;
                ScaledSplitHeight = Math.Min(Math.Max(trkSize.Minimum, ScaledSplitHeight), trkSize.Maximum);
                trkSize.DataBindings.Add("Value", this, "ScaledSplitHeight", false, DataSourceUpdateMode.OnPropertyChanged);
                lblSplitSize.Text = "Split Height:";
                chkDisplayRows.Enabled = true;
                chkDisplayRows.DataBindings.Clear();
                chkDisplayRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
            }
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Version version;
            if (element["Version"] != null)
                version = Version.Parse(element["Version"].InnerText);
            else
                version = new Version(1, 0, 0, 0);
            CurrentSplitTopColor = ParseColor(element["CurrentSplitTopColor"]);
            CurrentSplitBottomColor = ParseColor(element["CurrentSplitBottomColor"]);
            VisualSplitCount = Int32.Parse(element["VisualSplitCount"].InnerText);
            SplitPreviewCount = Int32.Parse(element["SplitPreviewCount"].InnerText);
            DisplayIcons = Boolean.Parse(element["DisplayIcons"].InnerText);
            ShowThinSeparators = Boolean.Parse(element["ShowThinSeparators"].InnerText);
            AlwaysShowLastSplit = Boolean.Parse(element["AlwaysShowLastSplit"].InnerText);
            SplitWidth = Single.Parse(element["SplitWidth"].InnerText.Replace(',', '.'), CultureInfo.InvariantCulture);
            
            if (version >= new Version(1, 5))
            {
                TimingMethod = element["TimingMethod"].InnerText;
            }
            else
            {
                TimingMethod = "Current Timing Method";
            }
            if (version >= new Version(1, 3))
            {
                OverrideTimesColor = Boolean.Parse(element["OverrideTimesColor"].InnerText);
                BeforeTimesColor = ParseColor(element["BeforeTimesColor"]);
                CurrentTimesColor = ParseColor(element["CurrentTimesColor"]);
                AfterTimesColor = ParseColor(element["AfterTimesColor"]);
                BeforeNamesColor = ParseColor(element["BeforeNamesColor"]);
                CurrentNamesColor = ParseColor(element["CurrentNamesColor"]);
                AfterNamesColor = ParseColor(element["AfterNamesColor"]);
                SplitHeight = Single.Parse(element["SplitHeight"].InnerText.Replace(',', '.'), CultureInfo.InvariantCulture);
                SplitGradientString = element["CurrentSplitGradient"].InnerText;
                BackgroundColor = ParseColor(element["BackgroundColor"]);
                BackgroundColor2 = ParseColor(element["BackgroundColor2"]);
                GradientString = element["BackgroundGradient"].InnerText;
                SeparatorLastSplit = Boolean.Parse(element["SeparatorLastSplit"].InnerText);
                DropDecimals = Boolean.Parse(element["DropDecimals"].InnerText);
                DeltasAccuracy = ParseEnum<TimeAccuracy>(element["DeltasAccuracy"]);
                OverrideDeltasColor = Boolean.Parse(element["OverrideDeltasColor"].InnerText);
                DeltasColor = ParseColor(element["DeltasColor"]);
                Comparison = element["Comparison"].InnerText;
                Display2Rows = Boolean.Parse(element["Display2Rows"].InnerText);
            }
            else
            {
                if (version >= new Version(1, 2))
                    BeforeNamesColor = CurrentNamesColor = AfterNamesColor = ParseColor(element["SplitNamesColor"]);
                else
                {
                    BeforeNamesColor = Color.FromArgb(255, 255, 255);
                    CurrentNamesColor = Color.FromArgb(255, 255, 255);
                    AfterNamesColor = Color.FromArgb(255, 255, 255);
                }
                BeforeTimesColor = Color.FromArgb(255, 255, 255);
                CurrentTimesColor = Color.FromArgb(255, 255, 255);
                AfterTimesColor = Color.FromArgb(255, 255, 255);
                OverrideTimesColor = false;
                SplitHeight = 6;
                CurrentSplitGradient = GradientType.Vertical;
                BackgroundColor = Color.Transparent;
                BackgroundColor2 = Color.Transparent;
                BackgroundGradient = ExtendedGradientType.Plain;
                SeparatorLastSplit = true;
                DropDecimals = true;
                DeltasAccuracy = TimeAccuracy.Tenths;
                OverrideDeltasColor = false;
                DeltasColor = Color.FromArgb(255, 255, 255);
                Comparison = "Current Comparison";
                Display2Rows = false;
            }              
            if (version >= new Version(1, 2))
            {
                ShowSplitTimes = Boolean.Parse(element["ShowSplitTimes"].InnerText);
                SplitTimesAccuracy = ParseEnum<TimeAccuracy>(element["SplitTimesAccuracy"]);
                if (version >= new Version(1, 3))
                    OverrideTextColor = Boolean.Parse(element["OverrideTextColor"].InnerText);
                else
                    OverrideTextColor = !Boolean.Parse(element["UseTextColor"].InnerText);
                ShowBlankSplits = Boolean.Parse(element["ShowBlankSplits"].InnerText);
                LockLastSplit = Boolean.Parse(element["LockLastSplit"].InnerText);
                IconSize = Single.Parse(element["IconSize"].InnerText.Replace(',', '.'), CultureInfo.InvariantCulture);
                IconShadows = Boolean.Parse(element["IconShadows"].InnerText);
            }
            else
            {
                ShowSplitTimes = true;
                SplitTimesAccuracy = TimeAccuracy.Seconds;
                OverrideTextColor = false;
                ShowBlankSplits = true;
                LockLastSplit = false;
                IconSize = 24f;
                IconShadows = true;
            }
                
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            parent.AppendChild(ToElement(document, "Version", "1.5"));
            parent.AppendChild(ToElement(document, CurrentSplitTopColor, "CurrentSplitTopColor"));
            parent.AppendChild(ToElement(document, CurrentSplitBottomColor, "CurrentSplitBottomColor"));
            parent.AppendChild(ToElement(document, "VisualSplitCount", VisualSplitCount));
            parent.AppendChild(ToElement(document, "SplitPreviewCount", SplitPreviewCount));
            parent.AppendChild(ToElement(document, "DisplayIcons", DisplayIcons));
            parent.AppendChild(ToElement(document, "ShowThinSeparators", ShowThinSeparators));
            parent.AppendChild(ToElement(document, "AlwaysShowLastSplit", AlwaysShowLastSplit));
            parent.AppendChild(ToElement(document, "SplitWidth", SplitWidth));
            parent.AppendChild(ToElement(document, "ShowSplitTimes", ShowSplitTimes));
            parent.AppendChild(ToElement(document, "SplitTimesAccuracy", SplitTimesAccuracy));
            parent.AppendChild(ToElement(document, BeforeNamesColor, "BeforeNamesColor"));
            parent.AppendChild(ToElement(document, CurrentNamesColor, "CurrentNamesColor"));
            parent.AppendChild(ToElement(document, AfterNamesColor, "AfterNamesColor"));
            parent.AppendChild(ToElement(document, "OverrideTextColor", OverrideTextColor));
            parent.AppendChild(ToElement(document, BeforeTimesColor, "BeforeTimesColor"));
            parent.AppendChild(ToElement(document, CurrentTimesColor, "CurrentTimesColor"));
            parent.AppendChild(ToElement(document, AfterTimesColor, "AfterTimesColor"));
            parent.AppendChild(ToElement(document, "OverrideTimesColor", OverrideTimesColor));
            parent.AppendChild(ToElement(document, "ShowBlankSplits", ShowBlankSplits));
            parent.AppendChild(ToElement(document, "LockLastSplit", LockLastSplit));
            parent.AppendChild(ToElement(document, "IconSize", IconSize));
            parent.AppendChild(ToElement(document, "IconShadows", IconShadows));
            parent.AppendChild(ToElement(document, "SplitHeight", SplitHeight));
            parent.AppendChild(ToElement(document, "CurrentSplitGradient", CurrentSplitGradient));
            parent.AppendChild(ToElement(document, BackgroundColor, "BackgroundColor"));
            parent.AppendChild(ToElement(document, BackgroundColor2, "BackgroundColor2"));
            parent.AppendChild(ToElement(document, "BackgroundGradient", BackgroundGradient));
            parent.AppendChild(ToElement(document, "SeparatorLastSplit", SeparatorLastSplit));
            parent.AppendChild(ToElement(document, "DeltasAccuracy", DeltasAccuracy));
            parent.AppendChild(ToElement(document, "DropDecimals", DropDecimals));
            parent.AppendChild(ToElement(document, "OverrideDeltasColor", OverrideDeltasColor));
            parent.AppendChild(ToElement(document, DeltasColor, "DeltasColor"));
            parent.AppendChild(ToElement(document, "Comparison", Comparison));
            parent.AppendChild(ToElement(document, "TimingMethod", TimingMethod));
            parent.AppendChild(ToElement(document, "Display2Rows", Display2Rows));
            return parent;
        }

        private Color ParseColor(XmlElement colorElement)
        {
            return Color.FromArgb(Int32.Parse(colorElement.InnerText, NumberStyles.HexNumber));
        }

        private XmlElement ToElement(XmlDocument document, Color color, string name)
        {
            var element = document.CreateElement(name);
            element.InnerText = color.ToArgb().ToString("X8");
            return element;
        }

        private T ParseEnum<T>(XmlElement element)
        {
            return (T)Enum.Parse(typeof(T), element.InnerText);
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var picker = new ColorPickerDialog();
            picker.SelectedColor = picker.OldColor = button.BackColor;
            picker.SelectedColorChanged += (s, x) => button.BackColor = picker.SelectedColor;
            picker.ShowDialog(this);
            button.BackColor = picker.SelectedColor;
        }

        private XmlElement ToElement<T>(XmlDocument document, String name, T value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString();
            return element;
        }

        private XmlElement ToElement(XmlDocument document, String name, float value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString(CultureInfo.InvariantCulture);
            return element;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbComparison_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

    }
}
