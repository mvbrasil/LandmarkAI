using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using LandmarkAI.Classes;
using Newtonsoft.Json;

namespace LandmarkAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png; *.jpg)|*.png; *.jpg; *.jpeg|All files (*.*)|*.*";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(fileName));

                MakePredictionAsync(fileName);
            }
        }

        private async Task MakePredictionAsync(string fileName)
        {
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/2ebbdc78-f4e9-4717-8a0a-8fc830eb1583/classify/iterations/Iteration1/image";
            string predictionKey = "ca2d92581df5497591263a1470030839";
            string contentType = "application/octet-stream";

            var file = File.ReadAllBytes(fileName);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);
                
                using(var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    var response = await client.PostAsync(url, content);

                    var responseString =await response.Content.ReadAsStringAsync();

                    List<Prediction> predictions = (JsonConvert.DeserializeObject<CustomVision>(responseString)).predictions;
                    predictionsListView.ItemsSource = predictions;

                }
            }
        }
    }
}
