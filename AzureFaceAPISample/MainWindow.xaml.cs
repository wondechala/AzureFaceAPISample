//|---------------------------------------------------------------|
//|                   AZURE FACE API WPF CLIENT                   |
//|---------------------------------------------------------------|
//|                     Developed by Wonde Tadesse                |
//|                        Copyright ©2018 - Present              |
//|---------------------------------------------------------------|
//|                   AZURE FACE API WPF CLIENT                   |
//|---------------------------------------------------------------|

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Web;


using AzureFaceAPISample.Entities;
using Common;

namespace AzureFaceAPIWPFClient
{
    public partial class MainWindow : Window
    {
        #region Properties 

        List<Face> faces;
        string[] faceDescriptions;
        double resizeFactor;
        BitmapImage bitmapSource = new BitmapImage();
        AzureFaceAPIProcessor AzureFaceAPIProcessor = null;

        #endregion

        #region Constructor 

        public MainWindow()
        {
            InitializeComponent();
            AzureFaceAPIProcessor = new AzureFaceAPIProcessor(ConfigurationManager.AppSettings["AzureFaceAPISubscritionKey"], ConfigurationManager.AppSettings["AzureFaceAPISubscritionURL"]);
        }

        #endregion

        #region WPF Control Events 

        private void FacePhoto_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (faces == null)
                return;

            Point mouseXY = e.GetPosition(FacePhoto);

            ImageSource imageSource = FacePhoto.Source;
            BitmapSource bitmapSource = (BitmapSource)imageSource;

            var scale = FacePhoto.ActualWidth / (bitmapSource.PixelWidth / resizeFactor);

            bool mouseOverFace = false;

            for (int i = 0; i < faces.Count; ++i)
            {
                FaceRectangle fr = faces[i].FaceRectangle;
                double left = fr.Left * scale;
                double top = fr.Top * scale;
                double width = fr.Width * scale;
                double height = fr.Height * scale;

                if (mouseXY.X >= left && mouseXY.X <= left + width && mouseXY.Y >= top && mouseXY.Y <= top + height)
                {
                    faceDescriptionStatusBar.Text = faceDescriptions[i];
                    mouseOverFace = true;
                    break;
                }
            }

            if (!mouseOverFace)
                faceDescriptionStatusBar.Text = "Place the mouse pointer over a face to see the face description.";
        }

        private async void TrainFacesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openDlg = new FolderBrowserDialog();

                var result = openDlg.ShowDialog();

                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                Uri fileUri = new Uri(openDlg.SelectedPath);
                TrainFace trainFace = new TrainFace()
                {
                    ImageDirectoryPath = openDlg.SelectedPath,
                    PersonGroupID = ConfigurationManager.AppSettings["PersonGroupID"],
                    PersonGroupName = ConfigurationManager.AppSettings["PersonGroupName"],
                    PersonName = ConfigurationManager.AppSettings["PersonName"],
                };

                bool trainCompleted = await AzureFaceAPIProcessor.TrainFaces(trainFace);

                if (trainCompleted)
                {
                    faceDescriptionStatusBar.Text = "Face Training is completed !";
                }
                openDlg.Dispose();

            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void GenderChartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GenderChart genderChart = AzureFaceAPIProcessor.GetGenderChart(faces);
                await PublishSignalREvent(genderChart, EventName.ON_CHART_PRODUCED);
            }
            catch (Exception exception)
            {
                await PublishExceptionEvent(exception);
            }
        }

        private async void AgeGroupChartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AgeGroupChart ageGroupChart = AzureFaceAPIProcessor.AgeGroupChart(faces);
                await PublishSignalREvent(ageGroupChart, EventName.ON_CHART_PRODUCED);
            }
            catch (Exception exception)
            {
                await PublishExceptionEvent(exception);
            }
        }

        private async void DetectFacesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = BrowseAndLoadImage();
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return;
                }
                Stream stream = File.OpenRead(filePath);
                faceDescriptionStatusBar.Text = "Detecting...";
                faces = await AzureFaceAPIProcessor.DetectFaces(stream);
                faceDescriptionStatusBar.Text = String.Format("Detection Finished. {0} face(s) detected", faces.Count);

                if (faces.Count > 0)
                {
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext drawingContext = visual.RenderOpen();
                    drawingContext.DrawImage(bitmapSource,
                        new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                    double dpi = bitmapSource.DpiX;
                    resizeFactor = 96 / dpi;
                    faceDescriptions = new string[faces.Count];

                    for (int index = 0; index < faces.Count; ++index)
                    {
                        Face face = faces[index];
                        string json = JsonConvert.SerializeObject(face);
                        drawingContext.DrawRectangle(
                            Brushes.Transparent,
                            new Pen(Brushes.GreenYellow, 3),
                            new Rect(
                                face.FaceRectangle.Left * resizeFactor,
                                face.FaceRectangle.Top * resizeFactor,
                                face.FaceRectangle.Width * resizeFactor,
                                face.FaceRectangle.Height * resizeFactor));

                        faceDescriptions[index] = FaceDescription(face);
                    }

                    drawingContext.Close();

                    RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                        (int)(bitmapSource.PixelWidth * resizeFactor),
                        (int)(bitmapSource.PixelHeight * resizeFactor), 96, 96,
                        PixelFormats.Pbgra32);

                    faceWithRectBitmap.Render(visual);
                    FacePhoto.Source = faceWithRectBitmap;

                    faceDescriptionStatusBar.Text = "Hover mouse over a face to see the face description.";
                }
            }
            catch (Exception exception)
            {
                await PublishExceptionEvent(exception);
            }
        }

        private async void IdentifyFace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = BrowseAndLoadImage();
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return;
                }
                Stream stream = File.OpenRead(filePath);
                IdentifyFace identifyFace = await AzureFaceAPIProcessor.IdentifyFace(ConfigurationManager.AppSettings["PersonGroupID"], stream);
                if (identifyFace.IsFound)
                {
                    stream = File.OpenRead(filePath);
                    BinaryReader binaryReader = new BinaryReader(stream);
                    byte[] bytes = binaryReader.ReadBytes((Int32)stream.Length);
                    string base64ImageString = Convert.ToBase64String(bytes, 0, bytes.Length);
                    int startIndex = 0;
                    int amount = 1500; // Publish 1.5K of base64Image string
                    do
                    {
                        if (base64ImageString.Length - startIndex < amount)
                        {
                            identifyFace.Base64ImageString = base64ImageString.Substring(startIndex).Replace("+", "_").Replace(" ", string.Empty);
                            identifyFace.IsCompleted = true;
                        }
                        else
                        {
                            identifyFace.Base64ImageString = base64ImageString.Substring(startIndex, amount).Replace("+", "_").Replace(" ", string.Empty);
                            startIndex = startIndex + amount;
                        }
                        await PublishSignalREvent(identifyFace, EventName.ON_PHOTO_DETECTED);
                    } while (identifyFace.Base64ImageString.Length == amount);

                }
            }
            catch (Exception exception)
            {
                await PublishExceptionEvent(exception);
            }
        }

        #endregion

        #region Private Methods 

        private string BrowseAndLoadImage()
        {
            try
            {
                // Get the image file to scan from the user.
                var openDlg = new Microsoft.Win32.OpenFileDialog();

                openDlg.Filter = "JPEG Image(*.jpg)|*.jpg";
                bool? result = openDlg.ShowDialog(this);

                // Return if canceled.
                if (!(bool)result)
                {
                    return string.Empty;
                }

                bitmapSource = new BitmapImage();
                bitmapSource.BeginInit();
                bitmapSource.CacheOption = BitmapCacheOption.None;
                bitmapSource.UriSource = new Uri(openDlg.FileName);
                bitmapSource.EndInit();

                FacePhoto.Source = bitmapSource;
                faceDescriptionStatusBar.Text = "Loading image completed !";
                return openDlg.FileName;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private string FaceDescription(Face face)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Face: ");

            stringBuilder.Append(face.FaceAttributes.Gender);
            stringBuilder.Append(", ");
            stringBuilder.Append(face.FaceAttributes.Age);
            stringBuilder.Append(", ");
            stringBuilder.Append(String.Format("Smile {0:F1}%, ", face.FaceAttributes.Smile * 100));

            stringBuilder.Append("Emotion: ");
            EmotionScores emotionScores = face.FaceAttributes.Emotion;
            if (emotionScores.Anger >= 0.1f) stringBuilder.Append(string.Format("Anger {0:F1}%, ", emotionScores.Anger * 100));
            if (emotionScores.Contempt >= 0.1f) stringBuilder.Append(string.Format("Contempt {0:F1}%, ", emotionScores.Contempt * 100));
            if (emotionScores.Disgust >= 0.1f) stringBuilder.Append(string.Format("Disgust {0:F1}%, ", emotionScores.Disgust * 100));
            if (emotionScores.Fear >= 0.1f) stringBuilder.Append(string.Format("Fear {0:F1}%, ", emotionScores.Fear * 100));
            if (emotionScores.Happiness >= 0.1f) stringBuilder.Append(string.Format("Happiness {0:F1}%, ", emotionScores.Happiness * 100));
            if (emotionScores.Neutral >= 0.1f) stringBuilder.Append(string.Format("Neutral {0:F1}%, ", emotionScores.Neutral * 100));
            if (emotionScores.Sadness >= 0.1f) stringBuilder.Append(string.Format("Sadness {0:F1}%, ", emotionScores.Sadness * 100));
            if (emotionScores.Surprise >= 0.1f) stringBuilder.Append(string.Format("Surprise {0:F1}%, ", emotionScores.Surprise * 100));

            stringBuilder.Append(face.FaceAttributes.Glasses);
            stringBuilder.Append(", ");

            stringBuilder.Append("Hair: ");

            if (face.FaceAttributes.Hair.Bald >= 0.01f)
                stringBuilder.Append(String.Format("Bald {0:F1}% ", face.FaceAttributes.Hair.Bald * 100));

            HairColor[] hairColors = face.FaceAttributes.Hair.HairColor;
            foreach (HairColor hairColor in hairColors)
            {
                if (hairColor.Confidence >= 0.1f)
                {
                    stringBuilder.Append(hairColor.Color.ToString());
                    stringBuilder.Append(String.Format(" {0:F1}% ", hairColor.Confidence * 100));
                }
            }
            return stringBuilder.ToString();
        }

        private async Task<bool> PublishSignalREvent<T>(T type, EventName EventName) where T :
            class
        {
            HttpClient httpClient = new HttpClient();
            try
            {

                string JSONify = JsonConvert.SerializeObject(type);
                string url = string.Concat(ConfigurationManager.AppSettings["RESTfulSignalRServiceURL"], "?message=", JSONify, "&eventName=", EventName.EnumDescription());
                var responseEntity = await httpClient.PostAsJsonAsync(url, JSONify);
                if (!responseEntity.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("Http response Status Code : {0}, ReasonPhrase : {1}", (double)responseEntity.StatusCode, responseEntity.ReasonPhrase));
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return true;
        }

        private async Task PublishExceptionEvent(Exception exception)
        {
            ErrorResponse errorResponse = new ErrorResponse()
            {
                ErrorMessage = string.Concat("Error occurred while processing Azure Face API. Message : ", exception.Message)
            };
            try
            {
                await PublishSignalREvent(errorResponse, EventName.ON_EXCEPTION);
            }
            catch (Exception publishException)
            {
                System.Windows.MessageBox.Show(publishException.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

    }
}