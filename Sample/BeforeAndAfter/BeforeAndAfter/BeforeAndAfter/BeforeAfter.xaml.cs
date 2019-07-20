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


        double previousTranslateX = 0.0d;

        private void PanGestureRecognizer_OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    previousTranslateX = e.TotalX;
                    break;

                case GestureStatus.Running:
                    AddOffsetDelta(e.TotalX - previousTranslateX);
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
            this.Resources[Offset] = ((double) this.Resources[Offset]) + offsetDelta;
            this.Resources[NegativeOffset] = -((double) this.Resources[Offset]);
        }
    }
}