using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System;
using Microsoft.Win32;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Net;
using System.IO.Compression;

namespace GameManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            Update_Version_Click(null,null);
            InitializeComponent();
            Version_Info.Text = "当前版本：" + AppConfig.appVersion;
            if (!File.Exists(AppConfig.GameJsonPath))
            {
                try { using (File.Create(AppConfig.GameJsonPath)) { } }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR:" + ex.Message, "初始化错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                string content = File.ReadAllText(AppConfig.GameJsonPath);
                if (content != "")
                {
                    bool ruslt = flash();
                    if (!ruslt)
                    {
                        MessageBox.Show("读取游戏列表失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        public string JsonBulider(List<GameJson> content) {
            string JsonString = null;
            try
            {
                JsonString = JsonSerializer.Serialize(content);
            }
            catch (Exception ex) {
                MessageBox.Show("ERROR:" + ex.Message, "Json构造错误", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            return JsonString;
        }

        void Choice_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                string? selectedGame = comboBox.SelectedItem?.ToString();
                if (selectedGame != null)
                {
                    The_Game_Info.Content = selectedGame;
                    Start_the_Fucking_Game.IsEnabled = true;
                }
            }

        }
        bool flash() {
            bool is_flash = false;
            string content = File.ReadAllText(AppConfig.GameJsonPath);
            if (string.IsNullOrWhiteSpace(content))
            {
                return is_flash;
            }
            List<GameJson> gameJsons;
            try
            {
                gameJsons = JsonSerializer.Deserialize<List<GameJson>>(content);
            }
            catch (Exception ex) { 
                MessageBox.Show("ERROR:" + ex.Message, "Json解析错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return is_flash;
            }
            foreach (var item in gameJsons) {
                Choice_The_Fucking_Game.Items.Add(item.GameName);
            }
            is_flash = true;
            return is_flash;
        }

        private void add_Game(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Title = "请选择游戏主文件";
            Dialog.Filter = "应用程序 (*.exe)|*.exe";
            if (Dialog.ShowDialog() == true) {
                string path = Dialog.FileName;
                string GameName = Game_Name.Text;
                if (GameName == "输入你的游戏名称")
                {
                    MessageBox.Show("输入游戏名称呢 喵", "警告");
                    return;

                }
                List<GameJson> gameJsons = new List<GameJson> {
                        new GameJson { GameName = GameName, GamePath = path }

                    };
                
                string content = File.ReadAllText(AppConfig.GameJsonPath);
                if (string.IsNullOrWhiteSpace(content))
                {
                    content = JsonBulider(gameJsons);
                    if (content == null)
                    {
                        MessageBox.Show("你报错了wow！", "content = null");
                    }
                    else
                    {
                        File.WriteAllText(AppConfig.GameJsonPath, content);
                    }
                }
                else {
                    try {
                        
                        List<GameJson> gameJsons1 = JsonSerializer.Deserialize<List<GameJson>>(content);
                        foreach (var item in gameJsons1)
                        {
                            if (item.GameName == GameName)
                            {
                                MessageBox.Show("游戏名称重复了喵", "警告");
                                return;
                            }
                        }
                        gameJsons.AddRange(gameJsons1);
                        string jsonContent = JsonBulider(gameJsons);
                        File.WriteAllText(AppConfig.GameJsonPath, jsonContent);
                        MessageBox.Show("已完成添加！！！","提示",MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch(Exception ex) {
                        MessageBox.Show(ex.Message, "读取错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            if (!flash()) {
                MessageBox.Show("刷新游戏列表失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Start_the_Fucking_Game_Click(object sender, RoutedEventArgs e)
        {
            string GameName = The_Game_Info.Content.ToString();
            string Json = File.ReadAllText(AppConfig.GameJsonPath);
            if (Json == null)
            {
                MessageBox.Show("Json文件为空！", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            List<GameJson> gameJsons = new List<GameJson>();
            try
            {
                gameJsons = JsonSerializer.Deserialize<List<GameJson>>(Json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Json解析出现错误！" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            foreach (var item in gameJsons)
            {
                if (item.GameName == GameName)
                {
                    try
                    {
                        Process.Start(item.GamePath);
                        Close_the_Fucking_Game.IsEnabled = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("启动游戏失败！" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Close_the_Fucking_Game_Click(object sender, RoutedEventArgs e)
        {
            List<int> weizhi = new List<int>();
            string gameexe;
            string GameName = The_Game_Info.Content.ToString();
            string Json = File.ReadAllText(AppConfig.GameJsonPath);
            if (Json == null)
            {
                MessageBox.Show("Json文件为空！", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            List<GameJson> gameJsons = new List<GameJson>();
            try
            {
                gameJsons = JsonSerializer.Deserialize<List<GameJson>>(Json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Json解析出现错误！" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            foreach (var item in gameJsons)
            {
                if (item.GameName == GameName)
                {

                    
                    for (int i = 0; i < item.GamePath.Length; i++) {
                        if (item.GamePath[i] == '\\') {
                            weizhi.Add(i);
                        }
                    }
                    gameexe = item.GamePath.Substring(weizhi[weizhi.Count - 1] + 1);
                    
                    int index = gameexe.IndexOf(".");
                    string gamenoexe = gameexe.Substring(0, index);
                    Process[] processes = Process.GetProcessesByName(gamenoexe);

                    foreach (var process in processes) {
                        process.Kill();
                    }
                }
            }
            
        }

        private void Delete_Game_btn_Click(object sender, RoutedEventArgs e)
        {
            unGame unGameWindow = new unGame();
            unGameWindow.ShowDialog();
            Choice_The_Fucking_Game.Items.Clear();
            bool flash_result = flash();
            if (!flash_result) {
                MessageBox.Show("刷新游戏列表失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            The_Game_Info.Content = "请到游戏库选择一款游戏";
        }

        private void Update_Version_Click(object sender, RoutedEventArgs e)
        {
            using (WebClient client = new WebClient()) {
                string url = "http://update.kesug.com/bulidingversion.txt";
                byte[] data = client.DownloadData(url);
                int ver = (int)data[0];
                if (ver > AppConfig.bulidingversion)
                {
                    MessageBoxResult res = MessageBox.Show("有新版本可用,是否立即更新?",
                        "更新提示", MessageBoxButton.YesNo,
                        MessageBoxImage.Information,
                        MessageBoxResult.No);
                    if (res == MessageBoxResult.Yes) {
                        client.DownloadFile("http://update.kesug.com/GameManager.zip", "GameManager.zip");
                        MessageBox.Show("下载完成,请手动解压覆盖原文件", "更新提示", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }
                else {
                    MessageBox.Show("当前已是最新版本", "更新提示", MessageBoxButton.OK, MessageBoxImage.Information);

                }



            }
        }
    }
}