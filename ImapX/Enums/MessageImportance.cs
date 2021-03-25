using System.ComponentModel;

namespace ImapX.Enums
{
    [DefaultValue(Normal)]
    public enum MessageImportance
    {
        Normal,
        High,
        Medium,
        Low
    }
}
