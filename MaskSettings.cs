using Autodesk.AutoCAD.Colors;

namespace MaskMText
{
    /// <summary>
    /// Static class to store mask settings across command invocations
    /// </summary>
    public static class MaskSettings
    {
        private static double _maskOffset = 1.5;
        private static Color _maskColor = Color.FromColorIndex(ColorMethod.ByAci, 7); // White

        /// <summary>
        /// Mask offset factor (background scale factor)
        /// Default: 1.5 (150% of text height)
        /// </summary>
        public static double MaskOffset
        {
            get { return _maskOffset; }
            set
            {
                if (value > 0)
                    _maskOffset = value;
            }
        }

        /// <summary>
        /// Mask background color
        /// Default: Color Index 7 (White/Black depending on background)
        /// </summary>
        public static Color MaskColor
        {
            get { return _maskColor; }
            set { _maskColor = value; }
        }

        /// <summary>
        /// Reset settings to defaults
        /// </summary>
        public static void ResetToDefaults()
        {
            _maskOffset = 1.5;
            _maskColor = Color.FromColorIndex(ColorMethod.ByAci, 7);
        }
    }
}
