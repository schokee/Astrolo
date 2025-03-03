using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix("http://schemas.astrolo.org/", "astrolo")]
[assembly: XmlnsDefinition("http://schemas.astrolo.org/", "Astrolo.Presentation.Controls")]
[assembly: XmlnsDefinition("http://schemas.astrolo.org/", "Astrolo.Presentation.Controls.Astrology")]
[assembly: XmlnsDefinition("http://schemas.astrolo.org/", "Astrolo.Presentation.Controls.GeneKeys")]
[assembly: XmlnsDefinition("http://schemas.astrolo.org/", "Astrolo.Presentation.Controls.Halo")]
[assembly: XmlnsDefinition("http://schemas.astrolo.org/", "Astrolo.Presentation.Controls.HumanDesign")]
[assembly: XmlnsDefinition("http://schemas.astrolo.org/", "Astrolo.Presentation.Controls.YiJing")]
