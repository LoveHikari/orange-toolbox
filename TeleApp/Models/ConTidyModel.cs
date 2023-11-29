namespace TeleApp.Models;

public class ConTidyModel : NotificationObject
{
    private string _liveSourceFilePath;
    /// <summary>
    /// 直播源文件路径
    /// </summary>
    public string LiveSourceFilePath
    {
        get => _liveSourceFilePath;
        set { _liveSourceFilePath = value; NotifyPropertyChanged(); }
    }
}