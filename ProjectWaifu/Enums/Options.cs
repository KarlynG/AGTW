using System.ComponentModel;

namespace ProjectWaifu.Enums;

public enum MenuOptions
{
    [Description("2. Manually publish a tweet")]
    PublishTweet,
    [Description("1. Start Waifu")]
    Auto,
    [Description("Exit")]
    Exit,
    [Description("1. Publish text only")]
    Text,
    [Description("2. Publish tweet with generated media")]
    Media,
    [Description("Go back")]
    Back,
}

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var descriptionAttribute = (DescriptionAttribute)fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
        return descriptionAttribute != null ? descriptionAttribute.Description : value.ToString();
    }
}