using System.Windows;
using MotoBotCore;
using MotoBotCore.Irc;

namespace MotoBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bot _bot;
        public MainWindow()
        {
            InitializeComponent();
            JoinButton.IsEnabled = false;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            _bot = new Bot();
            _bot.SystemMessageReceived += (s, e2) =>
            {
                StatusBox.Dispatcher.Invoke(() => StatusBox.Text += e2.Message + "\n");
            };

            _bot.MessageReceived += (s, e3) =>
            {
                if (e3.QueryContext.IsPrivateMessage)
                {
                    var text = "PM:{0} -> {1}\n".F(e3.QueryContext.User.Name, e3.RawMessage);
                    MessageBox.Dispatcher.Invoke(() => MessageBox.Text += text);
                }
                else
                {
                    var text = "{0}:{1} -> {2}\n".F(e3.QueryContext.Channel.Name, e3.QueryContext.User.Name, e3.RawMessage);
                    MessageBox.Dispatcher.Invoke(() => MessageBox.Text += text);
                }
            };

            var network = new Network
            {
                Address = "irc.quakenet.org",
                Name = "Quakenet",
                Nickname = "MotoBot",
                NicknameAlt = "MotoBot_",
                Port = 6667,
            };
            _bot.Connect(network);
            _bot.Connected += (s, e1) =>
            {
                JoinButton.Dispatcher.Invoke(() => JoinButton.IsEnabled = true);
            };
        }

        private void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            _bot.Join(ChannelBox.Text);
        }
    }
}
