# LogoGen

LogoGen is a tool designed to help quickly convert an SVG logo to the myriad of resolutions required for modern mobile apps, especially when targeting multiple platforms (e.g. when using frameworks like Cordova and Xamarin).

LogoGen consists of a core .Net class library that can be used in your code directly, a console app on top to help automation, and a wpf GUI for making it simpler to use.

## The Class Library

To create and individual image, use `LogoGenerator.Generate()`:

```Csharp
var bitmap = new LogoGenerator().Generate(new LogoSettings {
   SvgPath = "C:\Temp\logo.svg",     // the source SVG
   Width = 1280,                     // target PNG width
   Height = 720,                     // target PNG height
   Scale = 0.6,                      // scale down factor for the source
   BackgroundColor = Color.White     // background color (can be transparent)
   SaveOutputFile = true             // Set to true if the result should be exported to file
   OutputPath = "C:\Temp\target.png" // Only used if SaveOutputFile is true
});
```

To create a batch of images, use `LogoGenerator.GenerateBatch()`:

```Csharp
var itemSettings = new [] {
    new BatchItemSettings(1024, 768, "C:\Temp\splash1.png"),
    new BatchItemSettings(1920, 1080, "C:\Temp\splash2.png"),
    new BatchItemSettings(100, 100, "C:\Temp\icon1.png", 1.0, Color.Transparent),
    new BatchItemSettings(192, 192, "C:\Temp\icon2.png", 1.0, Color.Transparent)
};

var batchSettings = new BatchSettings(
    "C:\Temp\logo.svg",
    0.6,
    Color.White,
    itemSettings);

var batchResults = new LogoGenerator.GenerateBatch(batchSettings);
```

The batch uses a single SVG and has default `Scale` and `Background Color` for the entire batch. For each item in the batch you need to specify the target PNG path and width/height, however you can optionally specify an override for the scale and color if you want.

The result is a collection of objects with a `Succeeded` and an `Exception` (if any occurred) per item.

## The Console Application

The console is a very thin wrapper for the batch generator above. You pass it a settings file which needs to be a JSON represenation of the `BatchSettings` object:

```cmd
C:\LogoGen\bin>LogoGen.Console.exe "C:\Temp\BatchSettings.json"
```

## The GUI

The UI is still very much work in progress, but is essentially a user friendly way of generating the setting JSON. The diagram below helps clarify the purpose of various controls:

![LogoGen GUI](/gui-help.png)
