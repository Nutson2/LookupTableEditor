using System;
using System.Diagnostics;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LookupTableEditor.ViewModels;

public abstract partial class BaseViewModel : ErrorsViewModel
{
    [ObservableProperty]
    private Page? _dialogPage;
    public event Action<Page?>? OnPageLoaded;

    partial void OnDialogPageChanged(Page? value) => OnPageLoaded?.Invoke(value);

    [RelayCommand]
    private void GotoTelegram() => Process.Start(Settings.Default.TelegramUrl);

    [RelayCommand]
    private void GoToYouTube() => Process.Start(Settings.Default.YouTubeChannelUrl);
}
