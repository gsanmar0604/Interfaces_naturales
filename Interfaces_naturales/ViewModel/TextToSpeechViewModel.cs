using Interfaces_naturales.Model;
using Interfaces_naturales.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Interfaces_naturales.ViewModel
{
    public class TextToSpeechViewModel : INotifyPropertyChanged
    {
        private readonly ITextToSpeech _textToSpeech;
        private readonly IDataService _dataService;
        private string _textToSpeak = string.Empty;
        private string _statusText = "Listo";
        private bool _isSpeaking;
        private ObservableCollection<SpeechHistoryItem> _history;

        public event PropertyChangedEventHandler? PropertyChanged;

        public TextToSpeechViewModel(ITextToSpeech textToSpeech, IDataService dataService)
        {
            _textToSpeech = textToSpeech;
            _dataService = dataService;
            _history = new ObservableCollection<SpeechHistoryItem>();

            SpeakCommand = new Command(async () => await SpeakTextAsync(), () => !string.IsNullOrWhiteSpace(TextToSpeak) && !IsSpeaking);
            StopCommand = new Command(async () => await StopSpeakingAsync(), () => IsSpeaking);
            ClearCommand = new Command(ClearText);
            LoadHistoryCommand = new Command(async () => await LoadHistoryAsync());
            ClearHistoryCommand = new Command(async () => await ClearHistoryAsync());
            UseHistoryItemCommand = new Command<SpeechHistoryItem>(UseHistoryItem);

            _ = LoadHistoryAsync();
        }

        public ICommand SpeakCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand LoadHistoryCommand { get; }
        public ICommand ClearHistoryCommand { get; }
        public ICommand UseHistoryItemCommand { get; }

        public string TextToSpeak
        {
            get => _textToSpeak;
            set
            {
                _textToSpeak = value;
                OnPropertyChanged();
                ((Command)SpeakCommand).ChangeCanExecute();
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public bool IsSpeaking
        {
            get => _isSpeaking;
            set
            {
                _isSpeaking = value;
                OnPropertyChanged();
                ((Command)SpeakCommand).ChangeCanExecute();
                ((Command)StopCommand).ChangeCanExecute();
            }
        }

        public ObservableCollection<SpeechHistoryItem> History
        {
            get => _history;
            set
            {
                _history = value;
                OnPropertyChanged();
            }
        }

        private async Task SpeakTextAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TextToSpeak))
                    return;

                IsSpeaking = true;
                StatusText = "Reproduciendo...";

                var locales = await _textToSpeech.GetLocalesAsync();
                var spanishLocale = locales.FirstOrDefault(l => l.Language.StartsWith("es")) ?? locales.First();

                await _textToSpeech.SpeakAsync(TextToSpeak, new SpeechOptions
                {
                    Locale = spanishLocale,
                    Pitch = 1.0f,
                    Volume = 1.0f
                });

                await _dataService.SaveHistoryItemAsync(new SpeechHistoryItem
                {
                    Text = TextToSpeak,
                    Type = SpeechType.TextToSpeech,
                    CreatedAt = DateTime.Now
                });

                await LoadHistoryAsync();
                StatusText = "Completado";
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsSpeaking = false;
            }
        }

        private async Task StopSpeakingAsync()
        {
            try
            {
                await _textToSpeech.SpeakAsync(string.Empty);
                IsSpeaking = false;
                StatusText = "Detenido";
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void ClearText()
        {
            TextToSpeak = string.Empty;
            StatusText = "Listo";
        }

        private async Task LoadHistoryAsync()
        {
            try
            {
                var history = await _dataService.GetHistoryAsync();
                History.Clear();
                foreach (var item in history)
                {
                    History.Add(item);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error cargando historial: {ex.Message}", "OK");
            }
        }

        private async Task ClearHistoryAsync()
        {
            var confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirmar",
                "¿Estás seguro de que quieres borrar todo el historial?",
                "Sí",
                "No");

            if (confirm)
            {
                await _dataService.ClearHistoryAsync();
                History.Clear();
                StatusText = "Historial borrado";
            }
        }

        private void UseHistoryItem(SpeechHistoryItem item)
        {
            if (item != null)
            {
                TextToSpeak = item.Text;
                StatusText = "Texto cargado del historial";
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
