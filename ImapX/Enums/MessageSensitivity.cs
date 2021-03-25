using System.ComponentModel;

namespace ImapX.Enums
{
    [DefaultValue(None)]
    public enum MessageSensitivity
    {
        None,
        Personal,
        Private,
        CompanyConfidential
    }
}
