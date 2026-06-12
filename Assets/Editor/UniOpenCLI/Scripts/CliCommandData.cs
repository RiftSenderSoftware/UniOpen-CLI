using System;
using System.Collections.Generic;

namespace UniOpenCLI.Editor
{
    [Serializable]
    public class CliCommandData
    {
        public string Name;
        public string Command;
        public bool Enabled;
    }

    [Serializable]
    public class CliCommandDataList
    {
        public List<CliCommandData> Items = new();
    }
}
