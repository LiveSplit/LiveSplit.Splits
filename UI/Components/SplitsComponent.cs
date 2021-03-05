using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class SplitsComponent : IComponent
    {
        public ComponentRendererComponent InternalComponent { get; protected set; }

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;

        protected IList<IComponent> Components { get; set; }
        protected IList<SplitComponent> SplitComponents { get; set; }

        protected SplitsSettings Settings { get; set; }

        private Dictionary<Image, Image> ShadowImages { get; set; }

        private int visualSplitCount;
        private int settingsSplitCount;

        protected bool PreviousShowLabels { get; set; }

        protected int ScrollOffset { get; set; }
        protected int LastSplitSeparatorIndex { get; set; }

        protected LiveSplitState CurrentState { get; set; }
        protected LiveSplitState OldState { get; set; }
        protected LayoutMode OldLayoutMode { get; set; }
        protected Color OldShadowsColor { get; set; }

        protected IEnumerable<ColumnData> ColumnsList => Settings.ColumnsList.Select(x => x.Data);

        public string ComponentName => "Splits";

        public float VerticalHeight => InternalComponent.VerticalHeight;

        public float MinimumWidth => InternalComponent.MinimumWidth;

        public float HorizontalWidth => InternalComponent.HorizontalWidth;

        public float MinimumHeight => InternalComponent.MinimumHeight;

        public IDictionary<string, Action> ContextMenuControls => null;

        public SplitsComponent(LiveSplitState state)
        {
            CurrentState = state;
            Settings = new SplitsSettings(state);
            InternalComponent = new ComponentRendererComponent();
            ShadowImages = new Dictionary<Image, Image>();
            visualSplitCount = Settings.VisualSplitCount;
            settingsSplitCount = Settings.VisualSplitCount;
            Settings.SplitLayoutChanged += Settings_SplitLayoutChanged;
            ScrollOffset = 0;
            RebuildVisualSplits();
            state.ComparisonRenamed += state_ComparisonRenamed;
        }

        void state_ComparisonRenamed(object sender, EventArgs e)
        {
            var args = (RenameEventArgs)e;
            foreach (var column in ColumnsList)
            {
                if (column.Comparison == args.OldName)
                {
                    column.Comparison = args.NewName;
                    ((LiveSplitState)sender).Layout.HasChanged = true;
                }
            }
        }

        void Settings_SplitLayoutChanged(object sender, EventArgs e)
        {
            RebuildVisualSplits();
        }

        private void RebuildVisualSplits()
        {
            Components = new List<IComponent>();
            SplitComponents = new List<SplitComponent>();
            InternalComponent.VisibleComponents = Components;

            var totalSplits = Settings.ShowBlankSplits ? Math.Max(Settings.VisualSplitCount, visualSplitCount) : visualSplitCount;

            if (Settings.ShowColumnLabels && CurrentState.Layout?.Mode == LayoutMode.Vertical)
            {
                Components.Add(new LabelsComponent(Settings, ColumnsList));
                Components.Add(new SeparatorComponent());
            }

            for (var i = 0; i < totalSplits; ++i)
            {
                if (i == totalSplits - 1 && i > 0)
                {
                    LastSplitSeparatorIndex = Components.Count;
                    if (Settings.AlwaysShowLastSplit && Settings.SeparatorLastSplit)
                        Components.Add(new SeparatorComponent());
                    else if (Settings.ShowThinSeparators)
                        Components.Add(new ThinSeparatorComponent());
                }

                var splitComponent = new SplitComponent(Settings, ColumnsList);
                Components.Add(splitComponent);
                if (i < visualSplitCount - 1 || i == (Settings.LockLastSplit ? totalSplits - 1 : visualSplitCount - 1))
                    SplitComponents.Add(splitComponent);                   

                if (Settings.ShowThinSeparators && i < totalSplits - 2)
                    Components.Add(new ThinSeparatorComponent());
            }
        }

        private void Prepare(LiveSplitState state)
        {
            if (state != OldState)
            {
                state.OnScrollDown += state_OnScrollDown;
                state.OnScrollUp += state_OnScrollUp;
                state.OnStart += state_OnStart;
                state.OnReset += state_OnReset;
                state.OnSplit += state_OnSplit;
                state.OnSkipSplit += state_OnSkipSplit;
                state.OnUndoSplit += state_OnUndoSplit;
                OldState = state;
            }

            var previousSplitCount = visualSplitCount;
            visualSplitCount = Math.Min(state.Run.Count, Settings.VisualSplitCount);
            if (previousSplitCount != visualSplitCount 
                || (Settings.ShowBlankSplits && settingsSplitCount != Settings.VisualSplitCount)
                || Settings.ShowColumnLabels != PreviousShowLabels
                || (Settings.ShowColumnLabels && state.Layout.Mode != OldLayoutMode))
            {
                PreviousShowLabels = Settings.ShowColumnLabels;
                OldLayoutMode = state.Layout.Mode;
                RebuildVisualSplits();
            }
            settingsSplitCount = Settings.VisualSplitCount;

            var skipCount = Math.Min(
                Math.Max(
                    0,
                    state.CurrentSplitIndex - (visualSplitCount - 2 - Settings.SplitPreviewCount + (Settings.AlwaysShowLastSplit ? 0 : 1))),
                state.Run.Count - visualSplitCount);
            ScrollOffset = Math.Min(Math.Max(ScrollOffset, -skipCount), state.Run.Count - skipCount - visualSplitCount);
            skipCount += ScrollOffset;

            if (OldShadowsColor != state.LayoutSettings.ShadowsColor)
                ShadowImages.Clear();

            foreach (var split in state.Run)
            {
                if (split.Icon != null && (!ShadowImages.ContainsKey(split.Icon) || OldShadowsColor != state.LayoutSettings.ShadowsColor))
                {
                    ShadowImages.Add(split.Icon, IconShadow.Generate(split.Icon, state.LayoutSettings.ShadowsColor));
                }
            }

            var iconsNotBlank = state.Run.Where(x => x.Icon != null).Count() > 0;
            foreach (var split in SplitComponents)
            {
                split.DisplayIcon = iconsNotBlank && Settings.DisplayIcons;

                if (split.Split != null && split.Split.Icon != null)
                    split.ShadowImage = ShadowImages[split.Split.Icon];
                else
                    split.ShadowImage = null;
            }
            OldShadowsColor = state.LayoutSettings.ShadowsColor;

            foreach (var component in Components)
            {
                if (component is SeparatorComponent)
                {
                    var separator = (SeparatorComponent)component;
                    var index = Components.IndexOf(separator);
                    if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
                    {
                        if (((SplitComponent)Components[index + 1]).Split == state.CurrentSplit)
                            separator.LockToBottom = true;
                        else if (Components[index - 1] is SplitComponent && ((SplitComponent)Components[index - 1]).Split == state.CurrentSplit)
                            separator.LockToBottom = false;
                    }
                    if (Settings.AlwaysShowLastSplit && Settings.SeparatorLastSplit && index == LastSplitSeparatorIndex)
                    {
                        if (skipCount >= state.Run.Count - visualSplitCount)
                        {
                            if (Settings.ShowThinSeparators)
                                separator.DisplayedSize = 1f;
                            else
                                separator.DisplayedSize = 0f;

                            separator.UseSeparatorColor = false;
                        }
                        else
                        {
                            separator.DisplayedSize = 2f;
                            separator.UseSeparatorColor = true;
                        }
                    }
                }
                else if (component is ThinSeparatorComponent)
                {
                    var separator = (ThinSeparatorComponent)component;
                    var index = Components.IndexOf(separator);
                    if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
                    {
                        if (((SplitComponent)Components[index + 1]).Split == state.CurrentSplit)
                            separator.LockToBottom = true;
                        else if (((SplitComponent)Components[index - 1]).Split == state.CurrentSplit)
                            separator.LockToBottom = false;
                    }
                }
            }
        }

        void state_OnUndoSplit(object sender, EventArgs e)
        {
            ScrollOffset = 0;
        }

        void state_OnSkipSplit(object sender, EventArgs e)
        {
            ScrollOffset = 0;
        }

        void state_OnSplit(object sender, EventArgs e)
        {
            ScrollOffset = 0;
        }

        void state_OnReset(object sender, TimerPhase e)
        {
            ScrollOffset = 0;
        }

        void state_OnStart(object sender, EventArgs e)
        {
            ScrollOffset = 0;
        }

        void state_OnScrollUp(object sender, EventArgs e)
        {
            ScrollOffset--;
        }

        void state_OnScrollDown(object sender, EventArgs e)
        {
            ScrollOffset++;
        }

        void DrawBackground(Graphics g, float width, float height)
        {
            if (Settings.BackgroundGradient != ExtendedGradientType.Alternating
                && (Settings.BackgroundColor.A > 0
                || Settings.BackgroundGradient != ExtendedGradientType.Plain
                && Settings.BackgroundColor2.A > 0))
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            Settings.BackgroundGradient == ExtendedGradientType.Horizontal
                            ? new PointF(width, 0)
                            : new PointF(0, height),
                            Settings.BackgroundColor,
                            Settings.BackgroundGradient == ExtendedGradientType.Plain
                            ? Settings.BackgroundColor
                            : Settings.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            Prepare(state);
            DrawBackground(g, width, VerticalHeight);
            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            Prepare(state);
            DrawBackground(g, HorizontalWidth, height);
            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
            RebuildVisualSplits();
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            var skipCount = Math.Min(
                Math.Max(
                    0,
                    state.CurrentSplitIndex - (visualSplitCount - 2 - Settings.SplitPreviewCount + (Settings.AlwaysShowLastSplit ? 0 : 1))),
                state.Run.Count - visualSplitCount);
            ScrollOffset = Math.Min(Math.Max(ScrollOffset, -skipCount), state.Run.Count - skipCount - visualSplitCount);
            skipCount += ScrollOffset;

            var i = 0;
            if (SplitComponents.Count >= visualSplitCount)
            {
                foreach (var split in state.Run.Skip(skipCount).Take(visualSplitCount - 1 + (Settings.AlwaysShowLastSplit ? 0 : 1)))
                {
                    SplitComponents[i].Split = split;
                    i++;
                }
                if (Settings.AlwaysShowLastSplit)
                    SplitComponents[i].Split = state.Run.Last();
            }

            if (invalidator != null)
                InternalComponent.Update(invalidator, state, width, height, mode);
        }

        public void Dispose()
        {
            foreach (IComponent component in Components)
            {
                component.Dispose();
            }

            Components.Clear();

            foreach (SplitComponent splitComponent in SplitComponents)
            {
                splitComponent.Dispose();
            }

            SplitComponents.Clear();
            Settings.SplitLayoutChanged -= Settings_SplitLayoutChanged;
            Settings.Dispose();
        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();
    }
}
