using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralPackageVersions.Legacy
{
    public class FileUtilities
    {
        public static string GetPathOfFileAbove(string file, string startingDirectory)
        {
            // This method does not accept a path, only a file name
            if (file.Any(i => i.Equals(Path.DirectorySeparatorChar) || i.Equals(Path.AltDirectorySeparatorChar)))
            {
                throw new ArgumentException("InvalidGetPathOfFileAboveParameter", file);
            }

            // Search for a directory that contains that file
            string directoryName = GetDirectoryNameOfFileAbove(startingDirectory, file);

            return String.IsNullOrEmpty(directoryName) ? String.Empty : Path.Combine(directoryName, file);
        }

        internal static string GetDirectoryNameOfFileAbove(string startingDirectory, string fileName)
        {

            // Canonicalize our starting location
            string lookInDirectory = Path.GetFullPath(startingDirectory);

            do
            {
                // Construct the path that we will use to test against
                string possibleFileDirectory = Path.Combine(lookInDirectory, fileName);

                // If we successfully locate the file in the directory that we're
                // looking in, simply return that location. Otherwise we'll
                // keep moving up the tree.
                if (File.Exists(possibleFileDirectory))
                {
                    // We've found the file, return the directory we found it in
                    return lookInDirectory;
                }
                else
                {
                    // GetDirectoryName will return null when we reach the root
                    // terminating our search
                    lookInDirectory = Path.GetDirectoryName(lookInDirectory);
                }
            }
            while (lookInDirectory != null);

            // When we didn't find the location, then return an empty string
            return String.Empty;
        }


    }
}
