using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeforeAndAfter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BeforeAfter : ContentView
    {
        public BeforeAfter()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty BeforeViewProperty = BindableProperty.Create(
            nameof(BeforeView),
            typeof(View),
            typeof(BeforeAfter),
            new BoxView { BackgroundColor = Color.FromHex("#FFFD700") },
            propertyChanged: OnBeforeViewChanged
        );

        private static void OnBeforeViewChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is BeforeAfter beforeAfter && newvalue is View newView)
            {
                // Remove any previous BeforeView
                var childrenToBeRemoved = beforeAfter.LayoutRoot.Children.Where(view => view != beforeAfter.AfterViewParentLayout).ToList();
                if (childrenToBeRemoved.Any())
                {
                    childrenToBeRemoved.ForEach(view => beforeAfter.LayoutRoot.Children.Remove(view));
                }

                // Add the new child to the before view
                beforeAfter.LayoutRoot.Children.Insert(0, newView);
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
            new BoxView { BackgroundColor = Color.FromHex("#FFFD700") },
            propertyChanged: OnAfterViewChanged
        );

        private static void OnAfterViewChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is BeforeAfter beforeAfter && newvalue is View newView)
            {
                // Remove any previous AfterView
                beforeAfter.AfterViewParentLayout.Children.Clear();

                // Add the new child to the after view
                beforeAfter.AfterViewParentLayout.Children.Add(newView);

                newView.TranslationX = (double) beforeAfter.Resources["NegativeOffset"];
            }
        }

        public View AfterView
        {
            get => GetValue(AfterViewProperty) as View;
            set => SetValue(AfterViewProperty, value);
        }
    }
}