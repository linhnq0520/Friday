using System.Globalization;
using System.Resources;
using Microsoft.Extensions.Localization;

namespace Friday.MCHair.Web.Localization;

public interface IUiLocalizer
{
    LocalizedString this[string name] { get; }

    LocalizedString this[string name, params object[] arguments] { get; }

    LocalizedString GetString(string name);
}

public sealed class UiLocalizer : IUiLocalizer
{
    private const string ResourceBaseName = "Friday.MCHair.Web.Resources.SharedResources";

    private static readonly ResourceManager ResourceManager = new(
        ResourceBaseName,
        typeof(UiLocalizer).Assembly
    );

    public LocalizedString this[string name] => GetString(name);

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            LocalizedString localized = GetString(name);
            if (localized.ResourceNotFound || arguments.Length == 0)
            {
                return localized;
            }

            return new LocalizedString(
                name,
                string.Format(CultureInfo.CurrentCulture, localized.Value, arguments),
                localized.ResourceNotFound,
                localized.SearchedLocation
            );
        }
    }

    public LocalizedString GetString(string name)
    {
        string? value = ResourceManager.GetString(name, CultureInfo.CurrentUICulture);
        bool notFound = string.IsNullOrEmpty(value);
        return new LocalizedString(
            name,
            notFound ? name : value!,
            notFound,
            notFound ? null : ResourceBaseName
        );
    }
}
