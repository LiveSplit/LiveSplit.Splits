using LiveSplit.Model;
using LiveSplit.Properties;
using LiveSplit.TimeFormatters;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class SplitComponent : IComponent
    {
        public ISegment Split { get; set; }

        protected SimpleLabel NameLabel { get; set; }
        protected SimpleLabel TimeLabel { get; set; }
        protected SimpleLabel MeasureTimeLabel { get; set; }
        protected SimpleLabel MeasureDeltaLabel { get; set; }
        protected SimpleLabel DeltaLabel { get; set; }
        public SplitsSettings Settings { get; set; }

        protected int FrameCount { get; set; }

        public GraphicsCache Cache { get; set; }
        protected bool NeedUpdateAll { get; set; }
        protected bool IsActive { get; set; }

        protected TimeAccuracy CurrentAccuracy { get; set; }
        protected TimeAccuracy CurrentDeltaAccuracy { get; set; }
        protected bool CurrentDropDecimals { get; set; }

        protected ITimeFormatter TimeFormatter { get; set; }
        protected ITimeFormatter DeltaTimeFormatter { get; set; }

        protected int IconWidth { get { return DisplayIcon ? (int)(Settings.IconSize+7.5f) : 0; } }

        public bool DisplayIcon { get; set; }

        public Image ShadowImage { get; set; }

        protected Image OldImage { get; set; }

        public Image NoIconImage = Resources.DefaultSplitIcon.ToBitmap();
        public Image NoIconShadow = IconShadow.Generate(Resources.DefaultSplitIcon.ToBitmap(), Color.Black);

        public float PaddingTop { get { return 0f; } }
        public float PaddingLeft { get { return 0f; } }
        public float PaddingBottom { get { return 0f; } }
        public float PaddingRight { get { return 0f; } }

        public float VerticalHeight
        {
            get { return 25 + Settings.SplitHeight; }
        }

        public float MinimumWidth { get; set; }

        public float HorizontalWidth
        {
            get { return Settings.SplitWidth + (Settings.ShowSplitTimes ? MeasureDeltaLabel.ActualWidth : 0) + MeasureTimeLabel.ActualWidth + IconWidth; }
        }

        public float MinimumHeight { get; set; }

        public IDictionary<string, Action> ContextMenuControls
        {
            get { return null; }
        }

        public SplitComponent(SplitsSettings settings)
        {
            NoIconShadow = IconShadow.Generate(NoIconImage, Color.Black);
            NameLabel = new SimpleLabel()
            {
                HorizontalAlignment = StringAlignment.Near,
                X = 8,
                Y = 3,
                Text = ""
            };
            TimeLabel = new SimpleLabel()
            {
                HorizontalAlignment = StringAlignment.Far,
                Y = 3,
                Text = ""
            };
            MeasureTimeLabel = new SimpleLabel();
            DeltaLabel = new SimpleLabel()
            {
                HorizontalAlignment = StringAlignment.Far,
                Y = 3,
                Text = ""
            };
            MeasureDeltaLabel = new SimpleLabel();
            Settings = settings;
            TimeFormatter = new RegularSplitTimeFormatter(Settings.SplitTimesAccuracy);
            DeltaTimeFormatter = new DeltaSplitTimeFormatter(Settings.DeltasAccuracy, Settings.DropDecimals);
            MinimumHeight = 31;

            MeasureTimeLabel.Text = TimeFormatter.Format(new TimeSpan(9, 0, 0));
            NeedUpdateAll = true;
            IsActive = false;

            Cache = new GraphicsCache();
        }

        private void DrawGeneral(Graphics g, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (NeedUpdateAll)
                UpdateAll(state);

            if (Settings.BackgroundGradient == ExtendedGradientType.Alternating)
                g.FillRectangle(new SolidBrush(
                    state.Run.IndexOf(Split) % 2 == 1 
                    ? Settings.BackgroundColor2 
                    : Settings.BackgroundColor
                    ), 0, 0, width, height);

            MeasureTimeLabel.Text = TimeFormatter.Format(new TimeSpan(24, 0, 0));
            MeasureDeltaLabel.Text = DeltaTimeFormatter.Format(new TimeSpan(0, 24, 0, 0));

            MeasureTimeLabel.Font = state.LayoutSettings.TimesFont;
            MeasureTimeLabel.IsMonospaced = true;
            MeasureDeltaLabel.Font = state.LayoutSettings.TimesFont;
            MeasureDeltaLabel.IsMonospaced = true;

            MeasureTimeLabel.SetActualWidth(g);
            MeasureDeltaLabel.SetActualWidth(g);
            TimeLabel.SetActualWidth(g);
            DeltaLabel.SetActualWidth(g);

            NameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            TimeLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            DeltaLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
            MinimumWidth = (Settings.ShowSplitTimes ? MeasureDeltaLabel.ActualWidth : 10) + MeasureTimeLabel.ActualWidth + IconWidth + 10;
            MinimumHeight = 0.85f * (g.MeasureString("A", state.LayoutSettings.TimesFont).Height + g.MeasureString("A", state.LayoutSettings.TextFont).Height);

            if (Settings.SplitTimesAccuracy != CurrentAccuracy)
            {
                TimeFormatter = new RegularSplitTimeFormatter(Settings.SplitTimesAccuracy);
                CurrentAccuracy = Settings.SplitTimesAccuracy;
            }
            if (Settings.DeltasAccuracy != CurrentDeltaAccuracy || Settings.DropDecimals != CurrentDropDecimals)
            {
                DeltaTimeFormatter = new DeltaSplitTimeFormatter(Settings.DeltasAccuracy, Settings.DropDecimals);
                CurrentDeltaAccuracy = Settings.DeltasAccuracy;
                CurrentDropDecimals = Settings.DropDecimals;
            }

            

            if (Split != null)
            {

                if (mode == LayoutMode.Vertical)
                {
                    NameLabel.VerticalAlignment = StringAlignment.Center;
                    DeltaLabel.VerticalAlignment = StringAlignment.Center;
                    TimeLabel.VerticalAlignment = StringAlignment.Center;
                    NameLabel.Y = 0;
                    DeltaLabel.Y = 0;
                    TimeLabel.Y = 0;
                    NameLabel.Height = height;
                    DeltaLabel.Height = height;
                    TimeLabel.Height = height;
                }
                else
                {
                    NameLabel.VerticalAlignment = StringAlignment.Near;
                    DeltaLabel.VerticalAlignment = StringAlignment.Far;
                    TimeLabel.VerticalAlignment = StringAlignment.Far;
                    NameLabel.Y = 0;
                    DeltaLabel.Y = height - 50;
                    TimeLabel.Y = height - 50;
                    NameLabel.Height = 50;
                    DeltaLabel.Height = 50;
                    TimeLabel.Height = 50;
                }

                if (IsActive)
                {
                    var currentSplitBrush = new LinearGradientBrush(
                        new PointF(0, 0),
                        Settings.CurrentSplitGradient == GradientType.Horizontal
                        ? new PointF(width, 0)
                        : new PointF(0, height),
                        Settings.CurrentSplitTopColor,
                        Settings.CurrentSplitGradient == GradientType.Plain
                        ? Settings.CurrentSplitTopColor
                        : Settings.CurrentSplitBottomColor);
                    g.FillRectangle(currentSplitBrush, 0, 0, width, height);
                }

                if (DisplayIcon)
                {
                    var icon = Split.Icon ?? NoIconImage;
                    var shadow = (Split.Icon != null) ? ShadowImage : NoIconShadow;

                    /*if (DateTime.Now.Date.Month == 4 && DateTime.Now.Date.Day == 1)
                    {
                        icon = LiveSplit.Web.Share.TwitchEmoteResolver.Resolve("Kappa", true, false, false);
                        shadow = null;
                    }*/

                    if (OldImage != icon)
                    {
                        ImageAnimator.Animate(icon, (s, o) => { });
                        ImageAnimator.Animate(shadow, (s, o) => { });
                        OldImage = icon;
                    }

                    var drawWidth = Settings.IconSize;
                    var drawHeight = Settings.IconSize;
                    var shadowWidth = Settings.IconSize * (5 / 4f);
                    var shadowHeight = Settings.IconSize * (5 / 4f);
                    if (icon.Width > icon.Height)
                    {
                        var ratio = icon.Height / (float)icon.Width;
                        drawHeight *= ratio;
                        shadowHeight *= ratio;
                    }
                    else
                    {
                        var ratio = icon.Width / (float)icon.Height;
                        drawWidth *= ratio;
                        shadowWidth *= ratio;
                    }

                    ImageAnimator.UpdateFrames(shadow);
                    if (Settings.IconShadows && shadow != null)
                    {
                        g.DrawImage(
                            shadow,
                            7 + (Settings.IconSize * (5 / 4f) - shadowWidth) / 2 - 0.7f,
                            (height - Settings.IconSize) / 2.0f + (Settings.IconSize * (5 / 4f) - shadowHeight) / 2 - 0.7f,
                            shadowWidth,
                            shadowHeight);
                    }

                    ImageAnimator.UpdateFrames(icon);

                    g.DrawImage(
                        icon,
                        7 + (Settings.IconSize - drawWidth) / 2,
                        (height - Settings.IconSize) / 2.0f + (Settings.IconSize - drawHeight) / 2,
                        drawWidth,
                        drawHeight);
                }

                NameLabel.Font = state.LayoutSettings.TextFont;
                //NameLabel.Text = Split.Name;
                NameLabel.X = 5 + IconWidth;
                NameLabel.HasShadow = state.LayoutSettings.DropShadows;

                TimeLabel.Font = state.LayoutSettings.TimesFont;

                if (Settings.ShowSplitTimes)
                {
                    TimeLabel.Width = MeasureTimeLabel.ActualWidth + 20;
                    TimeLabel.X = width - MeasureTimeLabel.ActualWidth - 27;
                }
                else
                {
                    TimeLabel.Width = Math.Max(MeasureDeltaLabel.ActualWidth, MeasureTimeLabel.ActualWidth) + 20;
                    TimeLabel.X = width - Math.Max(MeasureDeltaLabel.ActualWidth, MeasureTimeLabel.ActualWidth) - 27;
                }
                TimeLabel.HasShadow = state.LayoutSettings.DropShadows;
                TimeLabel.IsMonospaced = true;

                DeltaLabel.Font = state.LayoutSettings.TimesFont;
                DeltaLabel.X = width - MeasureTimeLabel.ActualWidth - MeasureDeltaLabel.ActualWidth - 32;
                DeltaLabel.Width = MeasureDeltaLabel.ActualWidth + 20;
                DeltaLabel.HasShadow = state.LayoutSettings.DropShadows;
                DeltaLabel.IsMonospaced = true;

                NameLabel.Width = width - IconWidth - (mode == LayoutMode.Vertical ? DeltaLabel.ActualWidth + (String.IsNullOrEmpty(DeltaLabel.Text) ? TimeLabel.ActualWidth : MeasureTimeLabel.ActualWidth + 5) + 10 : 10);

                NameLabel.Draw(g);
                TimeLabel.Draw(g);
                DeltaLabel.Draw(g);
            }
            else DisplayIcon = Settings.DisplayIcons;
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            if (Settings.Display2Rows)
                DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Horizontal);
            else
                DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
            //DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Vertical);
        }

        public string ComponentName
        {
            get { return "Split"; }
        }


        public Control GetSettingsControl(LayoutMode mode)
        {
            throw new NotImplementedException();
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            throw new NotImplementedException();
        }


        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            throw new NotImplementedException();
        }

        public string UpdateName
        {
            get { throw new NotSupportedException(); }
        }

        public string XMLURL
        {
            get { throw new NotSupportedException(); }
        }

        public string UpdateURL
        {
            get { throw new NotSupportedException(); }
        }

        public Version Version
        {
            get { throw new NotSupportedException(); }
        }

        protected void UpdateAll(LiveSplitState state)
        {
            if (Split != null)
            {
                NameLabel.Text = Split.Name;

                var comparison = Settings.Comparison == "Current Comparison" ? state.CurrentComparison : Settings.Comparison;
                if (!state.Run.Comparisons.Contains(comparison))
                    comparison = state.CurrentComparison;

                var timingMethod = state.CurrentTimingMethod;
                if (Settings.TimingMethod == "Real Time")
                    timingMethod = TimingMethod.RealTime;
                else if (Settings.TimingMethod == "Game Time")
                    timingMethod = TimingMethod.GameTime;

                var splitIndex = state.Run.IndexOf(Split);
                if (splitIndex < state.CurrentSplitIndex)
                {
                    TimeLabel.ForeColor = Settings.OverrideTimesColor ? Settings.BeforeTimesColor : state.LayoutSettings.TextColor;
                    NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.BeforeNamesColor : state.LayoutSettings.TextColor;
                    var deltaTime = Split.SplitTime[timingMethod] - Split.Comparisons[comparison][timingMethod];
                    if (!Settings.ShowSplitTimes)
                    {
                        var color = LiveSplitStateHelper.GetSplitColor(state, deltaTime, 0, splitIndex, comparison, timingMethod);
                        if (color == null)
                            color = Settings.OverrideTimesColor ? Settings.BeforeTimesColor : state.LayoutSettings.TextColor;
                        TimeLabel.ForeColor = color.Value;
                        if (deltaTime != null)
                            TimeLabel.Text = DeltaTimeFormatter.Format(deltaTime);
                        else
                            TimeLabel.Text = TimeFormatter.Format(Split.SplitTime[timingMethod]);
                        DeltaLabel.Text = "";
                    }
                    else
                    {
                        var color = LiveSplitStateHelper.GetSplitColor(state, deltaTime, 0, splitIndex, comparison, timingMethod);
                        if (color == null)
                            color = Settings.OverrideTimesColor ? Settings.BeforeTimesColor : state.LayoutSettings.TextColor;
                        DeltaLabel.ForeColor = color.Value;
                        TimeLabel.Text = TimeFormatter.Format(Split.SplitTime[timingMethod]);
                        DeltaLabel.Text = DeltaTimeFormatter.Format(deltaTime);
                    }

                }
                else
                {
                    if (Split == state.CurrentSplit)
                    {
                        TimeLabel.ForeColor = Settings.OverrideTimesColor ? Settings.CurrentTimesColor : state.LayoutSettings.TextColor;
                        NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.CurrentNamesColor : state.LayoutSettings.TextColor;
                    }
                    else
                    {
                        TimeLabel.ForeColor = Settings.OverrideTimesColor ? Settings.AfterTimesColor : state.LayoutSettings.TextColor;
                        NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.AfterNamesColor : state.LayoutSettings.TextColor;
                    }
                    TimeLabel.Text = TimeFormatter.Format(Split.Comparisons[comparison][timingMethod]);
                    //Live Delta
                    var bestDelta = LiveSplitStateHelper.CheckLiveDelta(state, false, comparison, timingMethod);
                    if (bestDelta != null && Split == state.CurrentSplit)
                    {
                        if (!Settings.ShowSplitTimes)
                        {
                            TimeLabel.Text = DeltaTimeFormatter.Format(bestDelta);
                            TimeLabel.ForeColor = Settings.OverrideDeltasColor ? Settings.DeltasColor : state.LayoutSettings.TextColor;
                            DeltaLabel.Text = "";
                        }
                        else
                        {
                            DeltaLabel.Text = DeltaTimeFormatter.Format(bestDelta);
                            DeltaLabel.ForeColor = Settings.OverrideDeltasColor ? Settings.DeltasColor : state.LayoutSettings.TextColor;
                        }
                    }
                    else
                    {
                        DeltaLabel.Text = "";
                    }
                }
            }
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (Split != null)
            {
                UpdateAll(state);
                NeedUpdateAll = false;

                IsActive = (state.CurrentPhase == TimerPhase.Running
                            || state.CurrentPhase == TimerPhase.Paused) &&
                                                    state.CurrentSplit == Split;

                Cache.Restart();
                Cache["Icon"] = Split.Icon;
                if (Cache.HasChanged)
                {
                    if (Split.Icon == null)
                        FrameCount = 0;
                    else
                        FrameCount = Split.Icon.GetFrameCount(new FrameDimension(Split.Icon.FrameDimensionsList[0]));
                }
                Cache["DisplayIcon"] = DisplayIcon;
                Cache["SplitName"] = NameLabel.Text;
                Cache["DeltaLabel"] = DeltaLabel.Text;
                Cache["TimeLabel"] = TimeLabel.Text;
                Cache["IsActive"] = IsActive;
                Cache["NameColor"] = NameLabel.ForeColor.ToArgb();
                Cache["TimeColor"] = TimeLabel.ForeColor.ToArgb();
                Cache["DeltaColor"] = DeltaLabel.ForeColor.ToArgb();

                if (invalidator != null && Cache.HasChanged || FrameCount > 1)
                {
                    invalidator.Invalidate(0, 0, width, height);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
