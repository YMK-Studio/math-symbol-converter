using MathSymbolConverter.Services;
using MathSymbolConverter.Views;
using Prism.Ioc;
using System.Windows;
using MathSymbolConverter.ViewModels;

namespace MathSymbolConverter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<SettingsDialog, SettingsDialogViewModel>();
            containerRegistry.RegisterSingleton<IMessageBoxService, MessageBoxService>();
            containerRegistry.RegisterSingleton<IWindowService, WindowService>();
            containerRegistry.RegisterSingleton<ISymbolManagerService, SymbolManagerService>();
            containerRegistry.RegisterSingleton<IKeyboardListenerService, KeyboardListenerService>();
            containerRegistry.RegisterSingleton<IDatabaseService, DatabaseService>();
        }
    }

}
