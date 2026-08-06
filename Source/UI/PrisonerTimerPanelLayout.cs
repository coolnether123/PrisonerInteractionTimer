using System;

namespace PrisonerInteractionTimer.UI
{
    /// <summary>
    /// Carries measured row geometry so drawing and vanilla height reservation
    /// use the same layout decision.
    /// </summary>
    internal readonly struct PrisonerTimerPanelLayout
    {
        public PrisonerTimerPanelLayout(
            float contentWidth,
            bool stackHeader,
            float headerLabelWidth,
            float statusWidth,
            float headerLabelRowHeight,
            float statusRowHeight,
            float headerHeight,
            float modeRowHeight,
            float nextRowHeight,
            float watchRowHeight,
            float outerHeight)
        {
            ContentWidth = contentWidth;
            StackHeader = stackHeader;
            HeaderLabelWidth = headerLabelWidth;
            StatusWidth = statusWidth;
            HeaderLabelRowHeight = headerLabelRowHeight;
            StatusRowHeight = statusRowHeight;
            HeaderHeight = headerHeight;
            ModeRowHeight = modeRowHeight;
            NextRowHeight = nextRowHeight;
            WatchRowHeight = watchRowHeight;
            OuterHeight = outerHeight;
        }

        public float ContentWidth { get; }

        public bool StackHeader { get; }

        public float HeaderLabelWidth { get; }

        public float StatusWidth { get; }

        public float HeaderLabelRowHeight { get; }

        public float StatusRowHeight { get; }

        public float HeaderHeight { get; }

        public float ModeRowHeight { get; }

        public float NextRowHeight { get; }

        public float WatchRowHeight { get; }

        public float OuterHeight { get; }
    }

    /// <summary>
    /// Measures localized, wrapping panel text so narrow tabs and descenders
    /// remain visible without repositioning vanilla rows.
    /// </summary>
    internal static class PrisonerTimerPanelLayoutCalculator
    {
        internal const float PanelGap = 8f;
        internal const float HorizontalPadding = 10f;
        internal const float TopPadding = 8f;
        internal const float BottomPadding = 10f;
        internal const float RowGap = 2f;
        internal const float HeaderGap = 6f;
        internal const float MinimumRowHeight = 24f;
        internal const float TextBottomPadding = 4f;

        internal static PrisonerTimerPanelLayout Measure(
            float panelWidth,
            float headerTextWidth,
            float statusTextWidth,
            float headerTextHeight,
            float statusTextHeight,
            float modeTextHeight,
            float nextTextHeight,
            float watchTextHeight,
            bool showWatch)
        {
            float contentWidth = Math.Max(
                1f,
                panelWidth - 2f * HorizontalPadding);
            bool stackHeader =
                headerTextWidth + HeaderGap + statusTextWidth >
                contentWidth;

            float headerLabelRowHeight = RowHeight(headerTextHeight);
            float statusRowHeight = RowHeight(statusTextHeight);
            float headerHeight = stackHeader
                ? headerLabelRowHeight + RowGap + statusRowHeight
                : Math.Max(headerLabelRowHeight, statusRowHeight);
            float modeRowHeight = RowHeight(modeTextHeight);
            float nextRowHeight = RowHeight(nextTextHeight);
            float watchRowHeight = showWatch
                ? RowHeight(watchTextHeight)
                : 0f;
            float outerHeight = TopPadding + headerHeight + RowGap +
                modeRowHeight + RowGap + nextRowHeight + BottomPadding;

            if (showWatch)
            {
                outerHeight += RowGap + watchRowHeight;
            }

            float statusWidth = stackHeader
                ? contentWidth
                : Math.Min(statusTextWidth, contentWidth);
            float headerLabelWidth = stackHeader
                ? contentWidth
                : Math.Max(1f, contentWidth - HeaderGap - statusWidth);

            return new PrisonerTimerPanelLayout(
                contentWidth,
                stackHeader,
                headerLabelWidth,
                statusWidth,
                headerLabelRowHeight,
                statusRowHeight,
                headerHeight,
                modeRowHeight,
                nextRowHeight,
                watchRowHeight,
                outerHeight);
        }

        internal static float AdditionalTabHeight(
            PrisonerTimerPanelLayout layout)
        {
            return PanelGap + layout.OuterHeight;
        }

        private static float RowHeight(float textHeight)
        {
            return Math.Max(
                MinimumRowHeight,
                Math.Max(0f, textHeight) + TextBottomPadding);
        }
    }
}
