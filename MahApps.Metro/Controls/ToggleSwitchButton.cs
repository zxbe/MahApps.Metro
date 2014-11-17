using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A Button that allows the user to toggle between two states.
    /// </summary>
    [TemplatePart(Name = PART_DraggingThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_SwitchTrack, Type = typeof(Grid))]
    [TemplatePart(Name = PART_ThumbIndicator, Type = typeof(Rectangle))]
    [TemplatePart(Name = PART_ThumbTranslate, Type = typeof(TranslateTransform))]
    public class ToggleSwitchButton : ToggleButton
    {
        private const string PART_DraggingThumb = "PART_DraggingThumb";
        private const string PART_SwitchTrack = "PART_SwitchTrack";
        private const string PART_ThumbIndicator = "PART_ThumbIndicator";
        private const string PART_ThumbTranslate = "PART_ThumbTranslate";

        private Thumb _DraggingThumb;
        private Grid _SwitchTrack;
        private Rectangle _ThumbIndicator;
        private TranslateTransform _ThumbTranslate;

        public static readonly DependencyProperty SwitchForegroundProperty = DependencyProperty.Register("SwitchForeground", typeof(Brush), typeof(ToggleSwitchButton), new PropertyMetadata(null));
        /// <summary>
        /// Gets/sets the brush used for the control's foreground.
        /// </summary>
        public Brush SwitchForeground
        {
            get
            {
                return (Brush)GetValue(SwitchForegroundProperty);
            }
            set
            {
                SetValue(SwitchForegroundProperty, value);
            }
        }

        public ToggleSwitchButton()
        {
            DefaultStyleKey = typeof(ToggleSwitchButton);
            Checked += ToggleSwitchButton_Checked;
            Unchecked += ToggleSwitchButton_Checked;
        }

        void ToggleSwitchButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateThumb();
        }

        private void UpdateThumb()
        {
            if (_ThumbTranslate != null && _SwitchTrack != null && _ThumbIndicator != null)
            {
                double destination = IsChecked.GetValueOrDefault() ? ActualWidth - (_SwitchTrack.Margin.Left + _SwitchTrack.Margin.Right + _ThumbIndicator.ActualWidth) : 0;

                DoubleAnimation animation = new DoubleAnimation();
                animation.To = destination;
                animation.Duration = TimeSpan.FromMilliseconds(500);
                animation.EasingFunction = new ExponentialEase() { Exponent = 9 };
                animation.FillBehavior = FillBehavior.Stop;

                animation.Completed += (sender, e) =>
                    {
                        _ThumbTranslate.X = destination;
                    };
                _ThumbTranslate.BeginAnimation(TranslateTransform.XProperty, animation);
            }
        }

        protected override void OnToggle()
        {
            IsChecked = IsChecked != true;
            UpdateThumb();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _DraggingThumb = GetTemplateChild(PART_DraggingThumb) as Thumb;
            _SwitchTrack = GetTemplateChild(PART_SwitchTrack) as Grid;
            _ThumbIndicator = GetTemplateChild(PART_ThumbIndicator) as Rectangle;
            _ThumbTranslate = GetTemplateChild(PART_ThumbTranslate) as TranslateTransform;

            if (_DraggingThumb != null && _SwitchTrack != null && _ThumbIndicator != null && _ThumbTranslate != null)
            {
                _DraggingThumb.DragStarted += _DraggingThumb_DragStarted;
                _DraggingThumb.DragDelta += _DraggingThumb_DragDelta;
                _DraggingThumb.DragCompleted += _DraggingThumb_DragCompleted;

                _SwitchTrack.SizeChanged += _SwitchTrack_SizeChanged;
            }
        }

        private double? _lastDragPosition;
        private bool _isDragging;
        void _DraggingThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _lastDragPosition = _ThumbTranslate.X;
            _isDragging = false;
        }

        void _DraggingThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_lastDragPosition.HasValue)
            {
                if (Math.Abs(e.HorizontalChange) > 3)
                    _isDragging = true;
                if (_SwitchTrack != null && _ThumbIndicator != null)
                {
                    double lastDragPosition = _lastDragPosition.Value;
                    _ThumbTranslate.X = Math.Min(ActualWidth - (_SwitchTrack.Margin.Left + _SwitchTrack.Margin.Right + _ThumbIndicator.ActualWidth), Math.Max(0, lastDragPosition + e.HorizontalChange));
                }
            }
        }

        void _DraggingThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _lastDragPosition = null;
            if (!_isDragging)
            {
                OnToggle();
            }
            else if (_ThumbTranslate != null && _SwitchTrack != null)
            {
                if (!IsChecked.GetValueOrDefault() && _ThumbTranslate.X + 6.5 >= _SwitchTrack.ActualWidth / 2)
                {
                    OnToggle();
                }
                else if (IsChecked.GetValueOrDefault() && _ThumbTranslate.X + 6.5 <= _SwitchTrack.ActualWidth / 2)
                {
                    OnToggle();
                }
                else UpdateThumb();
            }
        }

        void _SwitchTrack_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_ThumbTranslate != null && _SwitchTrack != null && _ThumbIndicator != null)
            {
                double destination = IsChecked.GetValueOrDefault() ? ActualWidth - (_SwitchTrack.Margin.Left + _SwitchTrack.Margin.Right + _ThumbIndicator.ActualWidth) : 0;
                _ThumbTranslate.X = destination;
            }
        }
    }
}