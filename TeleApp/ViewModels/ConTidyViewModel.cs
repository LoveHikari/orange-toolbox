using System.Text;
using System.Windows;
using System.Windows.Input;
using Hikari.Common.IO;
using TeleApp.Models;

namespace TeleApp.ViewModels;

public class ConTidyViewModel
{
    public ConTidyModel Model {get; set; }

    public ConTidyViewModel()
    {
        this.Model = new ConTidyModel();
        Model.LiveSourceFilePath = "";
    }
    /// <summary>
    /// 浏览文件事件
    /// </summary>
    public ICommand OpenFileCommand
    {
        get
        {
            return new DelegateCommand<object>(delegate(object obj)
            {
                Ookii.Dialogs.Wpf.VistaFileDialog dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog()
                {
                    Title = "打开直播源文件",
                    Filter = "文本文件(*.txt)|*.txt",
                    RestoreDirectory = true,
                };
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    this.Model.LiveSourceFilePath = dialog.FileName;
                }
            });
        }
    }
    /// <summary>
    /// 检查事件
    /// </summary>
    public ICommand ExamineCommand
    {
        get
        {
            return new DelegateCommand<object>(delegate (object obj)
            {
                var text = FileHelper.Read(this.Model.LiveSourceFilePath);
                var line = text.Split(System.Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                var items = new Dictionary<string, List<string>>();
                var currentKey = "";
                foreach (var row in line)
                {
                    if (row.Contains(",#genre#"))
                    {
                        currentKey = row;
                        items.Add(row, new List<string>());
                    }
                    else
                    {
                        items[currentKey].Add(row);
                    }

                }

                foreach (var item in items)
                {
                    items[item.Key] = GroupSameItems(item.Value);
                }

                StringBuilder sb = new StringBuilder();
                foreach (var item in items)
                {
                    sb.AppendLine(item.Key);
                    foreach (var iv in item.Value)
                    {
                        sb.AppendLine(iv);
                    }
                    sb.AppendLine();
                }

                var dir = System.IO.Path.GetDirectoryName(this.Model.LiveSourceFilePath);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(this.Model.LiveSourceFilePath);
                var ext = System.IO.Path.GetExtension(this.Model.LiveSourceFilePath);
                string path = System.IO.Path.Combine(dir, fileName + DateTime.Now.ToString("HHmmss") + "-new"  + ext);
                FileHelper.Write(path, sb.ToString());
                var count = items.Sum(x => x.Value.Count);
                MessageBox.Show($"共查找了{count}项");
            });
        }
    }

    private List<string> GroupSameItems(List<string> array)
    {
        Dictionary<string, List<string>> itemGroups = new Dictionary<string, List<string>>();
        List<string> result = new List<string>();

        foreach (string item in array)
        {
            string key = item.Split(',')[0].Trim();

            if (itemGroups.ContainsKey(key))
            {
                itemGroups[key].Add(item);
            }
            else
            {
                itemGroups[key] = new List<string> { item };
            }
        }

        foreach (string item in array)
        {
            string key = item.Split(',')[0].Trim();

            if (itemGroups.ContainsKey(key))
            {
                result.AddRange(itemGroups[key]);
                itemGroups.Remove(key);
            }
        }

        return result;
    }
}