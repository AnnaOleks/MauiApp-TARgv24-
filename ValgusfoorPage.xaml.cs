using System;

namespace MauiApp_TARgv24_;

public partial class ValgusfoorPage : ContentPage
{
    public List<string> tekstnupp = new List<string>() { "ON", "OFF" };
    public List<string> tekstring = new List<string>() { "punane", "kollane", "roheline" };
    BoxView BoxView;
    Grid grid;
    Label label;
    VerticalStackLayout vsl;
    HorizontalStackLayout hsl;
    VerticalStackLayout root;
    ScrollView sv;

    public ValgusfoorPage()
	{
        Title = "Valgusfoor";
        vsl = new VerticalStackLayout{HorizontalOptions = LayoutOptions.Center};
        for (int i = 0; i < tekstring.Count; i++)
        {
            grid = new Grid
            {
                WidthRequest = 160,
                HeightRequest = 160,
                Margin = 5
           
            };
            BoxView = new BoxView
            {
                Color = Color.FromRgb(155, 166, 166),
                WidthRequest = 160,
                HeightRequest = 160,
                CornerRadius = 90,
                BackgroundColor = Color.FromRgba(0, 0, 0, 0),
                Margin = 5
          
            };
            label = new Label
            {
                Text = tekstring[i],
                FontSize = 20,
                FontFamily = "StoryScript-Regular",
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            grid.Add(BoxView); 
            grid.Add(label); 
            vsl.Add(grid);
        }
        hsl = new HorizontalStackLayout { HorizontalOptions = LayoutOptions.Center };
        for (int i = 0; i < tekstnupp.Count; i++)
        {
            Button nupp = new Button
            {
                Text = tekstnupp[i],
                FontSize = 20,
                BackgroundColor = Colors.White,
                TextColor = Color.FromRgb(4, 48, 61),
                CornerRadius = 5,
                BorderColor = Color.FromRgb(37, 186, 199),
                BorderWidth = 2,
                Margin = 10,
                FontFamily = "StoryScript-Regular",
                ZIndex = i,
            };
            nupp.WidthRequest = (int)(DeviceDisplay.MainDisplayInfo.Width / 8);
            hsl.Add(nupp);
           
        }
        root = new VerticalStackLayout
        {
            Children = { vsl, hsl },
    
        };

        sv = new ScrollView { Content = root, HorizontalOptions = LayoutOptions.Center };
        Content = sv;
    }
    
}