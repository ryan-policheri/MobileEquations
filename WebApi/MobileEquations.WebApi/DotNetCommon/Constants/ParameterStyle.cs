using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DotNetCommon.Constants
{
    public enum ParameterStyle
    {
        [Description("--")]
        DoubleDash,
        [Description("-")]
        SingleDash
    }
}
