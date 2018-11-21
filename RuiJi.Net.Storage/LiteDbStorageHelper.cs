using System.Runtime.InteropServices;

namespace RuiJi.Net.Storage
{
    public class LiteDbStorageHelper
    {
        
        /**
       * System.TypeInitializationException: The type initializer for 'RuiJi.Net.Node.Feed.Db.FeedLiteDb' threw an exception. --->
       * System.InvalidOperationException:
       * Your platform does not support FileStream.Lock. Please set mode=Exclusive in your connnection string to avoid this error. ---> System.PlatformNotSupportedException: Locking/unlocking file regions is not supported on this platform. Use FileShare on the entire file instead.
       *
       * fix:
       * OS X do not support file lock, so you can't use LiteDB with lock support (no multi process access). But you can still use LiteDB in multi thread model, using: "Mode=Exclusive" in connection string (exclusive mode do not lock file because open file in exclusive mode
       * https://github.com/mbdavid/LiteDB/issues/787
       * 
       */
        static readonly bool isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static string GetConnectionString(string dbPath)
        {
            return isOSX
                ? $@"Filename={dbPath};Mode=Exclusive"
                : dbPath;
        }
        
    }
}