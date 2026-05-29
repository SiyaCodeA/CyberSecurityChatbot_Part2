using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CyberSecurityChatbot_Part2
{
    public partial class MainWindow : Window
    {
        private Chatbot chatbot = new Chatbot();

        private enum AppState { WaitingForName, ShowingMenu, AskingQuestion }
        private AppState currentState = AppState.WaitingForName;

        private MediaPlayer player = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
        }

        // 
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets",
                    "greeting.wav");

                player.Open(new Uri(path, UriKind.Absolute));

                player.Volume = 1.0;

                player.Play();

                await Task.Delay(4000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Audio error: " + ex.Message);
            }

            AddAsciiArt();

            currentState = AppState.WaitingForName;

            await TypeMessage(
                "BOT",
                "Welcome! What is your name?",
                Brushes.Lime);

            txtUserInput.Focus();
        }

        // displaying the ASCII ART
        private void AddAsciiArt()
        {
            string art =
@"=================================================================================
   ____            _                 ____        _   
  / ___| _   _ ___| |_ ___ _ __     | __ )  ___ | |_ 
 | |    | | | / __| __/ _ \ '__|____|  _ \ / _ \| __|
 | |___ | |_| \__ \ ||  __/ | |_____| |_) | (_) | |_ 
  \____| \__, |___/\__\___|_|       |____/ \___/ \__|
         |___/                                       
=================================================================================
        CYBERSECURITY     AWARENESS     CHATBOT
=================================================================================
     Stay Safe Online         |      Protect Your Identity
=================================================================================";

            Paragraph paragraph = new Paragraph();
            Run run = new Run(art)
            {
                Foreground = Brushes.Cyan,
                FontFamily = new FontFamily("Consolas")
            };
            paragraph.Inlines.Add(run);
            rtbChat.Document.Blocks.Add(paragraph);
        }

        // Menu
        private async void ShowMenu()
        {
            currentState = AppState.ShowingMenu;
            pnlInput.Visibility = Visibility.Collapsed;
            pnlMenu.Visibility = Visibility.Visible;

            string memory = chatbot.RecallMemory();
            if (!string.IsNullOrEmpty(memory))
                await TypeMessage("BOT", memory, Brushes.Yellow);

            await TypeMessage("BOT", $"What would you like to do, {chatbot.UserName}?", Brushes.Lime);
        }

       
        private void txtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ProcessInput();
        }

        // Send Button 
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        //  Back to Menu Button
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            btnBack.Visibility = Visibility.Collapsed;
            ShowMenu();
        }

        //  Menu Buttons 
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            pnlMenu.Visibility = Visibility.Collapsed;
            pnlInput.Visibility = Visibility.Visible;
            btnBack.Visibility = Visibility.Visible;
            currentState = AppState.AskingQuestion;
            AddMessage("BOT", "Ask me anything about cybersecurity. Click ◀ Menu to go back.", Brushes.Lime);
            txtUserInput.Focus();
        }

        private async void btn2_Click(object sender, RoutedEventArgs e)
        {
            await TypeMessage("BOT", "You just watered the flower. It's blooming beautifully! ", Brushes.Magenta);
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            AddMessage("BOT", "Goodbye! Stay safe online.", Brushes.Lime);
            Task.Delay(1500).ContinueWith(_ => Dispatcher.Invoke(Application.Current.Shutdown));
        }

        private async void ProcessInput()
        {
            string input = txtUserInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            txtUserInput.Clear();

            if (currentState == AppState.WaitingForName)
            {
                string name = string.IsNullOrWhiteSpace(input) ? "User" : input;
                chatbot.SetUserName(name);
                AddMessage("YOU", name, Brushes.Cyan);
                await TypeMessage("BOT", $"Hello {name}, welcome to the Cybersecurity Awareness Bot!", Brushes.Lime);
                ShowMenu();
            }
            else if (currentState == AppState.AskingQuestion)
            {
                AddMessage("YOU", input, Brushes.Cyan);
                string response = chatbot.GetResponse(input);
                await TypeMessage("BOT", response, Brushes.Lime);
            }
        }

        // Typing effect for bot messages
        private async Task TypeMessage(string sender, string message, Brush color)
        {
            Paragraph paragraph = new Paragraph();
            Run labelRun = new Run(sender + ": ") { Foreground = color };
            Run textRun = new Run("") { Foreground = color };
            paragraph.Inlines.Add(labelRun);
            paragraph.Inlines.Add(textRun);
            rtbChat.Document.Blocks.Add(paragraph);
            rtbChat.ScrollToEnd();

            foreach (char c in message)
            {
                textRun.Text += c;
                rtbChat.ScrollToEnd();
                await Task.Delay(18);
            }
        }

     
        private void AddMessage(string sender, string message, Brush color)
        {
            Paragraph paragraph = new Paragraph();
            Run run = new Run(sender + ": " + message) { Foreground = color };
            paragraph.Inlines.Add(run);
            rtbChat.Document.Blocks.Add(paragraph);
            rtbChat.ScrollToEnd();
        }
    }
}