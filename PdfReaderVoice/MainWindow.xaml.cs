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
using System.Speech.Recognition;
using System.Windows.Shapes;
using System.Speech.Synthesis;
using System.Threading;
using System.Globalization;
using Microsoft.Win32;
using System.Security.Policy;

namespace PdfReaderVoice
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpeechRecognitionEngine recognizer;
        private Boolean recogOn;

        public MainWindow()
        {
            InitializeComponent();
            recogOn = false;
            InitializeRecognizer();
        }
        // DONT FORGET TO ENABLE MIC WITH KEYBOARD F4 key
        private void InitializeRecognizer()
        {
            var selectedRecognizer = (from e in SpeechRecognitionEngine.InstalledRecognizers()
                                      where e.Culture.Equals(Thread.CurrentThread.CurrentCulture)
                                      select e).FirstOrDefault();
            string[] words = { "monte", "milieu", "descend", "suivant", "précédent" };

            Grammar grammar = new Grammar(new GrammarBuilder(new Choices(words)));

            recognizer = new SpeechRecognitionEngine(selectedRecognizer);
            recognizer.LoadGrammar(grammar);
            recognizer.SetInputToDefaultAudioDevice();

            recognizer.AudioStateChanged += new EventHandler<AudioStateChangedEventArgs>(recognizer_AudioStateChanged);
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
        }

        private void recognizer_AudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            switch (e.AudioState)
            {
                case AudioState.Speech:
                    LabelStatus.Content = "Listening";
                    break;
                case AudioState.Silence:
                    LabelStatus.Content = "Idle";
                    break;
                case AudioState.Stopped:
                    LabelStatus.Content = "Stopped";
                    break;
            }
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (recogOn == false)
                return;
            //float accuracy = (float)e.Result.Confidence;
            string phrase = e.Result.Text;
            System.Console.WriteLine(phrase);

        }
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (!recogOn)
            {
                recogOn = true;
                ButtonStart.Content = "Stop";
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                recogOn = false;
                ButtonStart.Content = "Start";
                recognizer.RecognizeAsyncStop();
            }
        }

        private void ButtonOpenPdf_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() == true)
            {
                // do something with the filename
                System.Console.WriteLine($"{openFileDialog.FileName}");
                browser.Source =new System.Uri("file:///" + openFileDialog.FileName);
            }
        }
    }
}

