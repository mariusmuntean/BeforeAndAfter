using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeforeAndAfter
{
    [DesignTimeVisible(true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BeforeAfter : ContentView
    {
        private const string Offset = "Offset";
        private const string NegativeOffset = "NegativeOffset";
        private const string ThumbControlMarginOffset = "ThumbControlMarginOffset";
        private const string ThumbControlOffset = "ThumbControlOffset";

        public BeforeAfter()
        {
            InitializeComponent();

            // Apparently there's a bug in the PanGestureRecognizer such that on Android the PanUpdatedEventArgs.TotalX actually contains the X-delta since the last handler invocation
            if (Device.RuntimePlatform == Device.Android)
            {
                _getOffset = (panUpdatedEventArgs, previousPanX) => panUpdatedEventArgs.TotalX;
            }
            else
            {
                _getOffset = (panUpdatedEventArgs, previousPanX) => panUpdatedEventArgs.TotalX - previousPanX;
            }

            if (ThumbControl == null)
            {
                ThumbControl = new DefaultThumbControl();
            }
        }

        public static readonly BindableProperty BeforeViewProperty = BindableProperty.Create(
            nameof(BeforeView),
            typeof(View),
            typeof(BeforeAfter),
            new BoxView { BackgroundColor = Color.FromHex("#FFFFD700") }
        );

        public View BeforeView
        {
            get => GetValue(BeforeViewProperty) as View;
            set => SetValue(BeforeViewProperty, value);
        }

        public static readonly BindableProperty AfterViewProperty = BindableProperty.Create(
            nameof(AfterView),
            typeof(View),
            typeof(BeforeAfter),
            new BoxView { BackgroundColor = Color.FromHex("#FFFFD788") });

        public View AfterView
        {
            get => GetValue(AfterViewProperty) as View;
            set => SetValue(AfterViewProperty, value);
        }

        public static readonly BindableProperty ThumbControlProperty = BindableProperty.Create(
            nameof(ThumbControl),
            typeof(View),
            typeof(BeforeAfter),
            propertyChanged: OnThumbControlChanged
        );

        private static void OnThumbControlChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is BeforeAfter beforeAfter && newvalue is View newThumbControl)
            {
                newThumbControl.SizeChanged += (sender, args) =>
                {
                    beforeAfter.Resources[ThumbControlMarginOffset] = new Thickness(-newThumbControl.Width / 2.0d, 0, 0, 0);
                };
            }
        }

        public View ThumbControl
        {
            get => (View)GetValue(ThumbControlProperty);
            set => SetValue(ThumbControlProperty, value);
        }

        private double previousTranslateX = 0.0d;
        private readonly Func<PanUpdatedEventArgs, double, double> _getOffset;

        private void PanGestureRecognizer_OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    previousTranslateX = e.TotalX;
                    break;

                case GestureStatus.Running:
                    var offset = _getOffset(e, previousTranslateX);
                    AddOffsetDelta(offset);

                    previousTranslateX = e.TotalX;
                    break;

                case GestureStatus.Completed:
                    break;

                case GestureStatus.Canceled:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private double _showRatio = 0.0d;

        private void AddOffsetDelta(double offsetDelta)
        {
            if (Math.Abs(offsetDelta) < 0.01)
            {
                return;
            }

            var delta = ((double)Resources[Offset]) + offsetDelta;
            Resources[NegativeOffset] = -delta;
            Resources[Offset] = delta;

            // Compute how much of the Before is shown as the ratio between the delta and the total width
            _showRatio = delta / this.Width;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            // Handle screen rotation
            var newDelta = _showRatio * width;
            Resources[NegativeOffset] = -newDelta;
            Resources[Offset] = newDelta;

            base.OnSizeAllocated(width, height);
        }
    }
}