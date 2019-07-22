## BeforeAndAfter

A custom control that displays a before-and-after view of something. You usually provide two images and overlap them.

Get it from NuGet: https://www.nuget.org/packages/BeforeAndAfter/

Here's a quick rundown of the control's features:

* Specify the before and after views. These can be whatever Xamarin.Forms views you want, in the sample I'm using two normal Image controls.
* OPTIONAL: provide your own thumb control. Have a look at the sample, I'm using a material design icon.

## Known Limitations

On iOS you might have jittery movement.
Just add thesetwo lines to your App.xaml

             xmlns:ios="clr-namespace:Xamarin.Forms.PlatformConfiguration.iOSSpecific;assembly=Xamarin.Forms.Core"
             ios:Application.HandleControlUpdatesOnMainThread="true"

Here's a gif of the included sample app:

![Sample](media/BeforeAndAfterSample.gif)

Inspired by: https://github.com/xsahil03x/before_after/tree/master/example


As usual you can do everything you want with my code. Have fun!

Have a question?  Tweet at me: https://twitter.com/MunteanMarius
