# ByteArrayToImageSourceConverter

<https://github.com/LokiHonoo/development-resources>

<https://learn.microsoft.com/zh-cn/dotnet/communitytoolkit/maui/converters/byte-array-to-image-source-converter>

```xaml

<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
             x:Class="CommunityToolkit.Maui.Sample.Pages.Converters.ByteArrayToImageSourceConverterPage">

    <ContentPage.Resources>
        <ResourceDictionary>
            <toolkit:ByteArrayToImageSourceConverter x:Key="ByteArrayToImageSourceConverter" />
        </ResourceDictionary>
    </ContentPage.Resources>

    <Image Source="{Binding DotNetBotImageByteArray, Mode=OneWay, Converter={StaticResource ByteArrayToImageSourceConverter}}" />

</ContentPage>

```

```c#

class ByteArrayToImageSourceConverterPage : ContentPage
{
    public ByteArrayToImageSourceConverterPage()
    {
        var image = new Image();

    image.SetBinding(
        Image.SourceProperty,
        new Binding(
            static (ViewModel vm) => vm.DotNetBotImageByteArray,
            mode: BindingMode.OneWay,
            converter: new ByteArrayToImageSourceConverter()));

        Content = image;
    }
}

```
