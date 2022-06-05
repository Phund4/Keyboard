using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Text.Json;
namespace Keyboard
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, Button> _buttons;
        private Dictionary<string, SolidColorBrush> _colors;
        private int _index;
        private int _countRightKeys;
        private bool _IsUpper = false;
        private string _str;
        private const string _path = "WriteFromMe.json";

        public MainWindow()
        {
            _buttons = new Dictionary<string, Button>();
            _colors = new Dictionary<string, SolidColorBrush>();
            InitializeComponent();
            InitDictButtons();
            InitDictColors();
        }

        private void InitDictButtons()
        {
            foreach (Button element in KeyBoard.Children)
            {
                _buttons.Add(element.Name, element);
            }
        }

        private void InitDictColors()
        {
            foreach(Button b in KeyBoard.Children)
            {
                _colors.Add(b.Name, (SolidColorBrush)b.Background);
            }
        }

        private void ReadFromFile()
        {
            using (var fs = new FileStream(_path, FileMode.OpenOrCreate))
            {
                var file = JsonSerializer.DeserializeAsync<List<Stroka>>(fs).Result;
                foreach(var s in file)
                    if(s.Id == WindowSettings._diffIndex)
                        textBox.Text = s.Str;
            }
        }

        private void ClickCapsToUpper()
        {
            foreach (Button element in KeyBoard.Children)
                if (element.Content.ToString().Length == 1)
                    if (char.IsLetter(Convert.ToChar(element.Content.ToString())))
                        element.Content = element.Content.ToString().ToUpper();
        }

        private void ClickCapsToLower()
        {
            foreach (Button element in KeyBoard.Children)
                if (element.Content.ToString().Length == 1)
                    if (char.IsLetter(Convert.ToChar(element.Content.ToString())))
                        _buttons[element.Content.ToString()].Content = element.Content.ToString().ToLower();
        }

        private void ClickCaps()
        {
            if (!_IsUpper)
            {
                ClickCapsToUpper();
                _IsUpper = true;
            }
            else
            {
                _IsUpper = false;
                ClickCapsToLower();
            }
        }

        private void KeyDownButton(KeyEventArgs e)
        {
            try
            {
                if ((_buttons[e.Key.ToString()].Content as string).Length == 1)
                {
                    if (_str[_index] == Convert.ToChar(_buttons[e.Key.ToString()].Content))
                        _countRightKeys++;
                    textBoxUser.Text += _buttons[e.Key.ToString()].Content.ToString();
                    _index++;
                }
                _buttons[e.Key.ToString()].Background = new SolidColorBrush(Colors.Black);
            }
            catch (Exception) { }
        }

        private void SupportKeys(KeyEventArgs e)
        {
            if (_buttons[e.Key.ToString()].Name.ToString() == "Capital") ClickCaps();

            if (_buttons[e.Key.ToString()].Name.ToString() == "Space")
            {
                textBoxUser.Text += " ";
                _index++;
            }

            if (_buttons[e.Key.ToString()].Name.ToString() == "Back")
            {
                textBoxUser.Text = textBoxUser.Text.Substring(0, textBoxUser.Text.Length - 1);
                _index--;
            }
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!KeyBoard.IsEnabled) return;

            SupportKeys(e);

            KeyDownButton(e);

            if (_index == _str.Length)
                Stop_Click(sender, e);
        }

        private void UIElement_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (!KeyBoard.IsEnabled) return;
                _buttons[e.Key.ToString()].Background = _colors[e.Key.ToString()];
            }
            catch (Exception) { }
        }

        private void ClearAllKeys()
        {
            foreach(Button b in KeyBoard.Children)
            {
                _buttons[b.Name].Background = _colors[b.Name];
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            ReadFromFile();
            _str = textBox.Text;
            _countRightKeys = 0;
            KeyBoard.IsEnabled = true;
            LabelInfo.Content = null;
            Start.IsEnabled = false;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            ClearAllKeys();
            _index = 0;
            LabelInfo.Content = $"RightTaps: {_countRightKeys}";
            KeyBoard.IsEnabled = false;
            textBox.Text = null;
            textBoxUser.Text = null;
            Start.IsEnabled = true;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var windowSettings = new WindowSettings();
            windowSettings.Show();
        }
    }
}