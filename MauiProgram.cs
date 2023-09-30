
using Lab3.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lab3;
/**
Name: Gerald Vinyeta, Michael Meisenburg
Description: Lab 3
Date: 9/29/2023
Bugs: None known
Reflection: We didn't have too many problems setting up Cockroach Labs or 
            updating our methods with the example format given in class
            so the lab went by pretty smoothly.
**/
public static class MauiProgram
{
    public static readonly IBusinessLogic bl = new BusinessLogic();
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
