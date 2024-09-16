using System.Reflection;
using AppName;
using Xamarin.Forms.Xaml;
using AppName.Core;

[assembly: AssemblyTitle(AssemblyGlobal.ProductLine + " - " + "Xamarin.Forms ")]
[assembly: AssemblyConfiguration(AssemblyGlobal.Configuration)]
[assembly: AssemblyCompany("COMPANY")]
[assembly: AssemblyProduct(AssemblyGlobal.ProductLine + " - " + "Xamarin.Forms ")]
[assembly: AssemblyCopyright("Mobile App - 2020")]

// Comment the line bewlow to disable XAMLC
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: AppName.Core.GrialVersion("3.0.54.0")]
