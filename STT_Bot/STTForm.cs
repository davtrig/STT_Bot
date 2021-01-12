using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using STT_Bot.Models;

namespace STT_Bot
{
    public partial class STTForm : Form
    {
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine();
        SpeechSynthesizer Bot = new SpeechSynthesizer();
        SpeechRecognitionEngine startlistening = new SpeechRecognitionEngine();
        SpeechRecognitionEngine _answersRecognizer = new SpeechRecognitionEngine();
        Random rnd = new Random();
        int RecTimeOut = 0;


        public STTForm()
        {
            InitializeComponent();
            showCommands();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void STTForm_Load(object sender, EventArgs e)
        {
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines(@"DefaultCommands.txt")))));
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Default_SpeechRecognized);
            _recognizer.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(_recognizer_SpeechRecognized);
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);

            startlistening.SetInputToDefaultAudioDevice();
            startlistening.LoadGrammarAsync(new Grammar(new GrammarBuilder(new Choices(File.ReadAllLines(@"DefaultCommands.txt")))));
            startlistening.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(startlistening_SpeechRecognized);

            _answersRecognizer.SetInputToDefaultAudioDevice();
            DictationGrammar defaultDictationGrammar = new DictationGrammar();
            defaultDictationGrammar.Name = "default dictation";
            defaultDictationGrammar.Enabled = true;
            _answersRecognizer.LoadGrammarAsync(defaultDictationGrammar);
            _answersRecognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_answersRecognizer_SpeechRecognized);
        }

        private void Default_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            int ranNum;
            string speech = e.Result.Text;
            if (e.Result == null) return;

            if (speech == "Hello")
            {
                Bot.SpeakAsync("Hello, I am here");
            }
            if (speech == "How are you")
            {
                Bot.SpeakAsync("I am working normally");
            }
            if (speech == "What time is it")
            {
                Bot.SpeakAsync(DateTime.Now.ToString("h mm tt"));
            }
            if(speech == "Ask questions")
            {
                Bot.SpeakAsync("For which of your pets do you want to schedule an appointment?");
                _recognizer.RecognizeAsyncCancel();
                _answersRecognizer.RecognizeAsync(RecognizeMode.Multiple);
                
            }
            if (speech == "Stop talking")
            {
                Bot.SpeakAsyncCancelAll();
                ranNum = rnd.Next(1, 2);
                if (ranNum == 1)
                {
                    Bot.SpeakAsync("Yes sir");
                }
                else if (ranNum == 2)
                {
                    Bot.SpeakAsync("I will be quiet");
                }
            }
            if (speech == "Stop listening")
            {
                Bot.SpeakAsync("if you need me just ask");
                _recognizer.RecognizeAsyncCancel();
                startlistening.RecognizeAsync(RecognizeMode.Multiple);
            }
            if (speech == "Show commands")
            {
                showCommands();
            }
            if (speech == "Hide commands")
            {
                LstCommands.Visible = false;
            }
        }

        private void showCommands()
        {
            string[] commands = (File.ReadAllLines(@"DefaultCommands.txt"));
            LstCommands.Items.Clear();
            LstCommands.SelectionMode = SelectionMode.None;
            LstCommands.Visible = true;
            foreach (string command in commands)
            {
                LstCommands.Items.Add(command);
            }
        }

        private void _answersRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;
            if (e.Result == null) return;
            var question = new Question();
            question.Id = 1;
            question.Description = speech;
            string jsonObject = JsonConvert.SerializeObject(question);
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");

            HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = true
            };

            using (HttpClient client = new HttpClient(handler))
            {
                client.BaseAddress = new Uri("https://localhost:44398");
                var result = client.PostAsync("/api/Questions/", content).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;

                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                Payload match = json_serializer.Deserialize<Payload>(resultContent);
                Bot.SpeakAsync($"The word matched was {match.Word}");
            }
        }

        private void _recognizer_SpeechRecognized(object sender, SpeechDetectedEventArgs e)
        {
            RecTimeOut = 0;
        }

        private void startlistening_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string speech = e.Result.Text;
            
            if(speech == "Wake up")
            {
                startlistening.RecognizeAsyncCancel();
                Bot.SpeakAsync("Yes, I am here");
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void TimerSpeaking_Tick(object sender, EventArgs e)
        {
            if (RecTimeOut == 10)
            {
                _recognizer.RecognizeAsyncCancel();
            }
            else if (RecTimeOut == 11)
            {
                TimerSpeaking.Stop();
                startlistening.RecognizeAsync(RecognizeMode.Multiple);
                RecTimeOut = 0;
            }
        }
    }
}
