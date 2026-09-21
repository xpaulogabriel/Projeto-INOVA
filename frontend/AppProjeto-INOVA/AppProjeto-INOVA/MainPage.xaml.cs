namespace AppProjeto_INOVA
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnEntrarClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Atenção", "Login ainda não conectado a uma tela seguinte.", "OK");
            // quando você criar a tela de Dashboard, troque a linha acima por:
            // await Navigation.PushAsync(new DashboardPage());
        }

        private async void OnGoogleClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Google", "Login com Google ainda não implementado.", "OK");
        }

        private async void OnEsqueciSenhaTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Recuperar senha", "Fluxo de recuperação de senha aqui.", "OK");
        }
    }
}
