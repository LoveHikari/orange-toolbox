using System.Text;
using System.Windows;
using System.Windows.Input;
using Hikari.Common.IO;
using TeleApp.Models;

namespace TeleApp.ViewModels;

public class ConRepeatCheckViewModel
{
    public ConRepeatCheckModel Model { get; set; }

    public ConRepeatCheckViewModel()
    {
        this.Model = new ConRepeatCheckModel();
        Model.LiveSourceFilePath = "";
    }
    /// <summary>
    /// 浏览文件事件
    /// </summary>
    public ICommand OpenFileCommand
    {
        get
        {
            return new DelegateCommand<object>(delegate (object obj)
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
                var path = this.Model.LiveSourceFilePath;
                var text = FileHelper.Read(path);
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
                var yesDic = new Dictionary<string, List<string>>();
                var noDic = new Dictionary<string, List<string>>();
                foreach (var item in items)
                {

                    HashSet<string> uniqueSet = new HashSet<string>();
                    List<string> duplicatesList = new List<string>();
                    foreach (var vi in item.Value)
                    {
                        if (uniqueSet.Contains(vi)) // 说明重复
                        {
                            duplicatesList.Add(vi);
                        }
                        else
                        {
                            uniqueSet.Add(vi);
                        }
                    }
                    yesDic.Add(item.Key, duplicatesList);
                    noDic.Add(item.Key, uniqueSet.ToList());
                }

                StringBuilder sb = new StringBuilder();
                foreach (var item in yesDic)
                {
                    sb.AppendLine(item.Key);
                    foreach (var iv in item.Value)
                    {
                        sb.AppendLine(iv);
                    }
                    sb.AppendLine();
                }

                var dir = System.IO.Path.GetDirectoryName(path);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                var ext = System.IO.Path.GetExtension(path);
                string path1 = System.IO.Path.Combine(dir, fileName + DateTime.Now.ToString("HHmmss") + "-yes" + ext);
                FileHelper.Write(path1, sb.ToString());
                

                StringBuilder sb2 = new StringBuilder();
                foreach (var item in noDic)
                {
                    sb2.AppendLine(item.Key);
                    foreach (var iv in item.Value)
                    {
                        sb2.AppendLine(iv);
                    }
                    sb2.AppendLine();
                }

                path1 = System.IO.Path.Combine(dir, fileName + DateTime.Now.ToString("HHmmss") + "-no" + ext);
                FileHelper.Write(path1, sb2.ToString());

                var count = items.Sum(x => x.Value.Count);
                var countYes = yesDic.Sum(x => x.Value.Count);
                MessageBox.Show($"共查找了{count}项，发现{countYes}个重复源。");
            });
        }
    }

}