using System;

namespace NetApp.Common.Cache
{
    public class FileCacheDependency
    {
        public FileCacheDependency(string filename)
        {
            FileName = filename;
        }

        public string FileName { get; }
    }
}
