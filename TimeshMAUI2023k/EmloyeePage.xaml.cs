using System.Collections.ObjectModel;
using TimeshMAUI2023k.Models;
using Newtonsoft.Json;

namespace TimeshMAUI2023k;

public partial class EmployeePage : ContentPage
{
    // Muuttujan alustaminen
    ObservableCollection<Employee> dataa = new ObservableCollection<Employee>();

#if DEBUG
    private static readonly string Base = "http://10.0.2.2";
    private static readonly string ApiBaseUrl = $"{Base}:5126/";
#else
    private static readonly string Base = "https://YOUR_APP_SERVICE.azurewebsites.net";
    private static readonly string ApiBaseUrl = "$"{Base}:5001/";
#endif

    public EmployeePage()
    {
        InitializeComponent();

        LoadDataFromRestAPI();


        //Annetaan latausilmoitus
        emp_lataus.Text = "Ladataan työntekijöitä...";

    }

        async void LoadDataFromRestAPI()
        {
            try
            {

               
#if DEBUG
                HttpsClientHandlerService handler = new HttpsClientHandlerService();
                HttpClient client = new HttpClient(handler.GetPlatformMessageHandler());
#else
                    client = new HttpClient();
#endif

                client.BaseAddress = new Uri(ApiBaseUrl);
                string json = await client.GetStringAsync("api/employees");

                IEnumerable<Employee> employees = JsonConvert.DeserializeObject<Employee[]>(json);
                // dataa -niminen observableCollection on alustettukin jo ylhäällä päätasolla että hakutoiminto,
                // pääsee siihen käsiksi.
                // asetetaan sen sisältö ensi kerran tässä pienellä kepulikonstilla:
               dataa = new ObservableCollection<Employee>(employees);
            
                // Asetetaan datat näkyviin xaml tiedostossa olevalle listalle
                employeeList.ItemsSource = dataa;

                // Tyhjennetään latausilmoitus label
                emp_lataus.Text = "";

            }

            catch (Exception e)
            {
                await DisplayAlert("Virhe", e.Message.ToString(), "SELVÄ!");

            }
        }
    

    private void paivitys_nappi_Clicked(object sender, EventArgs e)
    {
        LoadDataFromRestAPI();
    }


    // Hakutoiminto
    private void OnSearchBarTextChanged(object sender, EventArgs args)
    {
       // SearchBar searchBar = (SearchBar)sender; alla sama asia eri syntksilla
        SearchBar searchBar = sender as SearchBar;

        string searchText = searchBar.Text;

        // Työntekijälistaukseen valitaan nyt vain ne joiden etu- tai sukunimeen sisältyy annettu hakutermi
        // "var dataa" on tiedoston päätasolla alustettu muuttuja, johon sijoitettiin alussa koko lista työntekijöistä.
        // Nyt siihen sijoitetaan vain hakuehdon täyttävät työntekijät
        employeeList.ItemsSource = dataa.Where(x => x.LastName.ToLower().Contains(searchText.ToLower())
        || x.FirstName.ToLower().Contains(searchText.ToLower()));

    }

    async void navibutton_Clicked(object sender, EventArgs e)
    {
        Employee emp = (Employee)employeeList.SelectedItem;

        if (emp == null)
        {
            await DisplayAlert("Valinta puuttuu", "Valitse työntekijä.", "OK"); // (otsikko, teksti, kuittausnapin teksti)
            //return;
        }
        else
        {

            int id = emp.IdEmployee;
            await Navigation.PushAsync(new WorkAssignmentPage(id)); // Navigoidaan uudelle sivulle
        }
    }
}


