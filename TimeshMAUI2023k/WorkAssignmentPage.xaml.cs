using Newtonsoft.Json;
using System.Collections.ObjectModel;
using TimeshMAUI2023k.Models;
using Microsoft.Maui.Devices.Sensors;
using System.Text;
using System.Text.Json;

namespace TimeshMAUI2023k;

public partial class WorkAssignmentPage : ContentPage
{
    int eId;
    string lat;
    string lon;
    // MAUI Geolocation dokumentaation ohjeen mukaan laitettu:
    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;

    
#if DEBUG
    private static readonly string Base = "http://10.0.2.2";
    private static readonly string ApiBaseUrl = $"{Base}:5126/";
#else
    private static readonly string Base = "https://YOUR_APP_SERVICE.azurewebsites.net";
    private static readonly string ApiBaseUrl = "$"{Base}:5001/";
#endif


    public WorkAssignmentPage(int id)
	{
		InitializeComponent();
        LoadDataFromRestAPI();
        GetCurrentLocation();

        //Annetaan latausilmoitus
        wa_lataus.Text = "Ladataan ty�teht�vi�...";
        lon_label.Text = "Sijaintia haetaan";
        eId = id;

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
            string json = await client.GetStringAsync("api/workassignments");

            IEnumerable<WorkAssignment> wa = JsonConvert.DeserializeObject<WorkAssignment[]>(json);
           
            ObservableCollection<WorkAssignment> dataa = new ObservableCollection<WorkAssignment>(wa);

            // Asetetaan datat n�kyviin xaml tiedostossa olevalle listalle
            waList.ItemsSource = dataa;

            // Tyhjennet��n latausilmoitus label
            wa_lataus.Text = "";

        }

        catch (Exception e)
        {
            await DisplayAlert("Virhe", e.Message.ToString(), "SELV�!");

        }
    }

    // Sijainnin haku
    public async Task GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
            {

                lat = location.Latitude.ToString();
                lon = location.Longitude.ToString();

                lat_label.Text = $"Latitude: {location.Latitude}";
                lon_label.Text = $"Longitude: {location.Longitude}";
            }
        }
        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (Exception ex)
        {
            // Unable to get location
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }

    public void CancelRequest()
    {
        if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            _cancelTokenSource.Cancel();
    }


    // ---------------------START----------------------

    async void startbutton_Clicked(object sender, EventArgs e)
    {
        WorkAssignment wa = (WorkAssignment)waList.SelectedItem;

        if (wa == null)
        {
            await DisplayAlert("Valinta puuttuu", "Valitse ty�teht�v�.", "OK");
            return;
        }


        try
        {
            // Luodaan operation luokan instanssi eli objekti joka sis�lt�� start metodissa v�litett�v�n datan
            var op = new Operation();

            op.EmployeeID = eId;
            op.WorkAssignmentID = wa.IdWorkAssignment;
            op.CustomerID = wa.IdCustomer;
            op.OperationType = "start";
            op.Comment = "Aloitettu";
            op.Latitude = lat;
            op.Longitude = lon;


#if DEBUG
            HttpsClientHandlerService handler = new HttpsClientHandlerService();
            HttpClient client = new HttpClient(handler.GetPlatformMessageHandler());
#else
                    client = new HttpClient();
#endif
            client.BaseAddress = new Uri(ApiBaseUrl);

            // Muutetaan em. data objekti Jsoniksi
            var input = JsonConvert.SerializeObject(op);

            HttpContent content = new StringContent(input, Encoding.UTF8, "application/json");


            // L�hetet��n serialisoitu objekti back-endiin Post pyynt�n�
            HttpResponseMessage message = await client.PostAsync("/api/workassignments", content);


            // Otetaan vastaan palvelimen vastaus
            string reply = await message.Content.ReadAsStringAsync();

            //Asetetaan vastaus de-serialisoituna success muuttujaan
            bool success = JsonConvert.DeserializeObject<bool>(reply);

            if (success == false)
            {
                await DisplayAlert("Ei voida aloittaa", "Ty� on jo k�ynniss�", "OK");
            }
            else if (success == true)
            {
                await DisplayAlert("Ty� aloitettu", "Ty� on aloitettu", "OK");
            }
        }


        catch (Exception ex)
        {
            await DisplayAlert(ex.GetType().Name, ex.Message, "OK");
        }
        

    }


    //---------------------- STOP ----------------------------

    async void stopbutton_Clicked(object sender, EventArgs e)
    {
        WorkAssignment wa = (WorkAssignment)waList.SelectedItem;

        if (wa == null)
        {
            await DisplayAlert("Valinta puuttuu", "Valitse ty�teht�v�.", "OK");
            return;
        }

        // Pyydet��n k�ytt�j�lt� kommentti
        string answer = await DisplayPromptAsync("Palaute", "Voit j�tt�� nyt kommentin halutessasi", "Valmis");
        if (answer == null)
        {
            answer = "-";
        }

        try
        {
            // Luodaan operation luokan instanssi eli objekti joka sis�lt�� start metodissa v�litett�v�n datan
            var op = new Operation();

            op.EmployeeID = eId;
            op.WorkAssignmentID = wa.IdWorkAssignment;
            op.CustomerID = wa.IdCustomer;
            op.OperationType = "stop";
            op.Comment = answer;
            op.Latitude = lat;
            op.Longitude = lon;


#if DEBUG
            HttpsClientHandlerService handler = new HttpsClientHandlerService();
            HttpClient client = new HttpClient(handler.GetPlatformMessageHandler());
#else
                    client = new HttpClient();
#endif
            client.BaseAddress = new Uri(ApiBaseUrl);

            // Muutetaan em. data objekti Jsoniksi
            var input = JsonConvert.SerializeObject(op);

            HttpContent content = new StringContent(input, Encoding.UTF8, "application/json");


            // L�hetet��n serialisoitu objekti back-endiin Post pyynt�n�
            HttpResponseMessage message = await client.PostAsync("/api/workassignments", content);


            // Otetaan vastaan palvelimen vastaus
            string reply = await message.Content.ReadAsStringAsync();

            //Asetetaan vastaus de-serialisoituna success muuttujaan
            bool success = JsonConvert.DeserializeObject<bool>(reply);

            if (success == false)
            {
                await DisplayAlert("Ei voida lopettaa", "Ty�t� ei ole aloitettu", "OK");
            }
            else if (success == true)
            {
                await DisplayAlert("Ty� lopetettu", "Ty� on lopetettu", "OK");
                await Navigation.PushAsync(new WorkAssignmentPage(eId));
            }
        }


        catch (Exception ex)
        {
            await DisplayAlert(ex.GetType().Name, ex.Message, "OK");
        }


    }
}
