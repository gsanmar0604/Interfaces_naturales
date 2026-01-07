using CommunityToolkit.Maui.Media;
using Interfaces_naturales.Model;
using Interfaces_naturales.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Interfaces_naturales.ViewModel
{
    public class SpeechToTextViewModel : INotifyPropertyChanged
    {
        private readonly ISpeechToText _speechToText;
        private readonly IDataService _dataService;
        private bool _isListening;
        private string _recognizedText = "El texto reconocido aparecerá aquí...";
        private string _statusText = "Listo";
        private string _buttonText = "Iniciar Reconocimiento";

        public event PropertyChangedEventHandler? PropertyChanged;

        public SpeechToTextViewModel(ISpeechToText speechToText, IDataService dataService)
        {
            _speechToText = speechToText;
            _dataService = dataService;
            StartListeningCommand = new Command(async () => await StartListeningAsync());
            ClearCommand = new Command(ClearText);
            NavigateToTextToSpeechCommand = new Command(async () => await NavigateToTextToSpeechAsync());
        }

        public ICommand StartListeningCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand NavigateToTextToSpeechCommand { get; }

        public string RecognizedText
        {
            get => _recognizedText;
            set
            {
                _recognizedText = value;
                OnPropertyChanged();
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

        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                OnPropertyChanged();
            }
        }

        public bool IsListening
        {
            get => _isListening;
            set
            {
                _isListening = value;
                OnPropertyChanged();
            }
        }

        private async Task StartListeningAsync()
        {
            try
            {
                if (IsListening)
                {
                    await StopListeningAsync();
                    return;
                }

                var permissionStatus = await Permissions.RequestAsync<Permissions.Microphone>();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Permiso Denegado",
                        "Se necesita acceso al micrófono para usar esta función",
                        "OK");
                    return;
                }

                await StartRecognitionAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task StartRecognitionAsync()
        {
            RecognizedText = "Escuchando...";
            StatusText = "Grabando...";
            ButtonText = "Detener";
            IsListening = true;

            var recognitionResult = await _speechToText.ListenAsync(
                CultureInfo.GetCultureInfo("es-ES"),
                new Progress<string>(partialText =>
                {
                    RecognizedText = partialText;
                }),
                CancellationToken.None);

            await ProcessRecognitionResultAsync(recognitionResult);
        }

        private async Task ProcessRecognitionResultAsync(SpeechToTextResult result)
        {
            if (result.IsSuccessful)
            {
                RecognizedText = result.Text;
                StatusText = "Completado";

                await _dataService.SaveHistoryItemAsync(new SpeechHistoryItem
                {
                    Text = result.Text,
                    Type = SpeechType.SpeechToText,
                    CreatedAt = DateTime.Now
                });
            }
            else
            {
                RecognizedText = $"Error: {result.Exception?.Message ?? "Desconocido"}";
                StatusText = "Error";
            }

            IsListening = false;
            ButtonText = "Iniciar Reconocimiento";
        }

        private async Task StopListeningAsync()
        {
            await _speechToText.StopListenAsync(CancellationToken.None);
            IsListening = false;
            ButtonText = "Iniciar Reconocimiento";
            StatusText = "Detenido";
        }

        private void ClearText()
        {
            RecognizedText = "El texto reconocido aparecerá aquí...";
            StatusText = "Listo";
        }

        private async Task NavigateToTextToSpeechAsync()
        {
            await Shell.Current.GoToAsync("//TextToSpeechPage");
        }

        private async Task HandleErrorAsync(Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            IsListening = false;
            ButtonText = "Iniciar Reconocimiento";
            StatusText = "Error";
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
