namespace MauiApp_TARgv24_
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        string text = "";
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                if (count > 10) {text= "1.kordsed";}
                else if (count > 20) {text="2.kordsed"; }
                else if (count > 30) { text = "3.kordsed"; }
                else if (count > 40) { text = "4.kordsed"; }
                else if (count > 50) { text = "5.kordsed"; }
                else if (count > 60) { text = "6.kordsed"; }
                else if (count > 70) { text = "7.kordsed"; }
                else if (count > 80) { text = "8.kordsed"; }
                else if (count > 90) { text = "9.kordsed"; }
                else if (count > 100) { text = "10.kordsed"; }
                CounterBtn.Text = $"Clicked {count} times \n {text}";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
