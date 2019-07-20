using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeforeAndAfter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BeforeAfter : ContentView
    {
        private const string Offset = "Offset";
        private const string NegativeOffset = "NegativeOffset";

        public BeforeAfter()
        {
            InitializeComponent();

            // Apparently there's a bug in the PanGestureRecognizer such that on Android the PAnUpdatedEventArgs.TotalX actually contains the X-delta since the last handler invocation
            if (Device.RuntimePlatform == Device.iOS)
            {
                _getOffset = (panUpdatedEventArgs, previousPanX) => panUpdatedEventArgs.TotalX - previousPanX;
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                _getOffset = (panUpdatedEventArgs, previousPanX) => panUpdatedEventArgs.TotalX;
            }
        }

        public static readonly BindableProperty BeforeViewProperty = BindableProperty.Create(
            nameof(BeforeView),
            typeof(View),
            typeof(BeforeAfter),
            new BoxView {BackgroundColor = Color.FromHex("#FFFD700")},
            propertyChanged: OnBeforeViewChanged
        );

        private static void OnBeforeViewChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is BeforeAfter beforeAfter)
            {
                // Remove any previous BeforeView
                if (oldvalue is View oldView && beforeAfter.LayoutRoot.Children.Contains(oldView))
                {
                    beforeAfter.LayoutRoot.Children.Remove(oldView);
                }

                // Add the new BeforeView
                if (newvalue is View newView)
                {
                    beforeAfter.LayoutRoot.Children.Insert(0, newView);
                }
            }
        }

        public View BeforeView
        {
            get => GetValue(BeforeViewProperty) as View;
            set => SetValue(BeforeViewProperty, value);
        }

        public static readonly BindableProperty AfterViewProperty = BindableProperty.Create(
            nameof(AfterView),
            typeof(View),
            typeof(BeforeAfter),
            new BoxView {BackgroundColor = Color.FromHex("#FFFD700")},
            propertyChanged: OnAfterViewChanged
        );

        private static void OnAfterViewChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is BeforeAfter beforeAfter)
            {
                // Remove any previous AfterView
                beforeAfter.AfterViewParentLayout.Children.Clear();

                // Add the new AfterView
                if (newvalue is View newView)
                {
                    beforeAfter.AfterViewParentLayout.Children.Add(newView);
                    //newView.TranslationX = (double) beforeAfter.Resources["NegativeOffset"];

                    newView.SetDynamicResource(TranslationXProperty, NegativeOffset);
                }
            }
        }

        public View AfterView
        {
            get => GetValue(AfterViewProperty) as View;
            set => SetValue(AfterViewProperty, value);
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

        private void AddOffsetDelta(double offsetDelta)
        {
            if (Math.Abs(offsetDelta) < 0.01)
            {
                return;
            }


            var delta = ((double) Resources[Offset]) + offsetDelta;
            Resources[NegativeOffset] = -delta;
            Resources[Offset] = delta;


            //var translationX = AfterViewParentLayout.TranslationX + offsetDelta;
            //AfterViewParentLayout.TranslateTo(translationX, 0, 1);
            //AfterView.TranslateTo(-translationX, 0, 1);
        }
    }
}