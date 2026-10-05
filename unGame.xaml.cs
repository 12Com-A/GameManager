using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace GameManager
{
    /// <summary>
    /// unGame.xaml 的交互逻辑
    /// </summary>
    public partial class unGame : Window
    {
        public unGame()
        {
            InitializeComponent();
            bool isflash = flash();
            if (!isflash)
            {
                MessageBox.Show("读取游戏列表失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }


        bool flash()
        {
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
            catch (Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "Json解析错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return is_flash;
            }
            foreach (var item in gameJsons)
            {
                GameToDelete.Items.Add(item.GameName);
            }
            is_flash = true;
            return is_flash;
        }

        private void DeleteGameButton_Click(object sender, RoutedEventArgs e)
        {
            string? selectedGame = GameToDelete.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedGame))
            {
                MessageBox.Show("你没有选择游戏喵", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                List<GameJson> gameJsons = JsonSerializer.Deserialize<List<GameJson>>(File.ReadAllText(AppConfig.GameJsonPath));
                
                foreach (var item in gameJsons)
                {
                    if (item.GameName == selectedGame)
                    {
                        try
                        {
                            item.GamePath = System.IO.Path.GetDirectoryName(item.GamePath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("获取游戏文件夹路径失败喵，可能是权限问题或者文件夹不存在\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        try
                        {
                            Directory.Delete(item.GamePath, true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("删除游戏文件夹失败喵，可能是权限问题或者文件夹不存在\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        gameJsons.Remove(item);
                        
                        GameToDelete.Items.Remove(selectedGame);
                        File.WriteAllText(AppConfig.GameJsonPath, "");
                        File.WriteAllText(AppConfig.GameJsonPath, JsonSerializer.Serialize(gameJsons));
                        break;
                    }
                    flash();
                }

            }
        }

        private void DeleteGameConfigButton_Click(object sender, RoutedEventArgs e)
        {
            string? selectedGame = GameToDelete.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedGame))
            {
                MessageBox.Show("你没有选择游戏喵", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                List<GameJson> gameJsons = JsonSerializer.Deserialize<List<GameJson>>(File.ReadAllText(AppConfig.GameJsonPath));
                List<int> xiegangweizhi = new List<int>();
                foreach (var item in gameJsons)
                {
                    if (item.GameName == selectedGame)
                    {
                        gameJsons.Remove(item);
                        break;
                    }
                }
                File.WriteAllText(AppConfig.GameJsonPath,"");
                File.WriteAllText(AppConfig.GameJsonPath, JsonSerializer.Serialize(gameJsons));
            }
            GameToDelete.Items.Remove(selectedGame);
            MessageBox.Show("游戏已删除喵", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            flash();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
