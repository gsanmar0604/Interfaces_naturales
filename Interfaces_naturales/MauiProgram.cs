using CommunityToolkit.Maui.Media;
using Interfaces_naturales.Services;
using Interfaces_naturales.View;
using Interfaces_naturales.ViewModel;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace Interfaces_naturales
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
            builder.Services.AddSingleton<ISpeechToText>(SpeechToText.Default);
            builder.Services.AddSingleton<ITextToSpeech>(TextToSpeech.Default);
            builder.Services.AddSingleton<IDataService, JsonDataService>();
            builder.Services.AddTransient<SpeechToTextViewModel>();
            builder.Services.AddTransient<TextToSpeechViewModel>();
            builder.Services.AddTransient<SpeechToTextPage>();
            builder.Services.AddTransient<TextToSpeechPage>();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}