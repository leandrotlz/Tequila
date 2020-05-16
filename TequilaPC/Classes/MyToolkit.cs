using System;
using System.IO;

class MyToolkit
{

    public static string[] args;

    public static string AllArgs() { 
        string strArgs = "";

        foreach (string arg in args) {
            strArgs += arg + " ";
        }

        return strArgs.Trim();
    }

    /// <summary>
    /// Static function to validate against path traversal, i.e. files named "..\bad.txt"
    /// Returns true if combining the RootPath and FileName results in an absolute path that's inside of the RootPath.
    /// Returns false if the combined absolute path is not inside of RootPath.
    /// </summary>
    /// <param name="RootPath"></param>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public static bool CheckFileName(string RootPath, string FileName) {
        string combinedPath = Path.Combine(RootPath, FileName);
        string absolutePath = Path.GetFullPath(combinedPath);
        return absolutePath.StartsWith(RootPath);
    }

    /// <summary>
    /// Static function to make sure dashes are appropiate for the specific OS. 
    /// Make sure all paths are absolute, windows paths must contain the drive name at the start for this to work.
    /// </summary>
    /// <param name="Path">File path to validate</param>
    /// <returns></returns>
    public static string ValidPath(string Path) {
        if (Path.Contains(":\\"))
        {
            // Make sure the file name has propper direction slashes for Windows
            Path = Path.Replace("/", "\\");
        } else {
            // Make sure the file name has propper direction slashes for OSX
            Path = Path.Replace("\\", "/");
        }

        return Path;
    }

    public static int MinMax(int Val, int Min, int Max) {
        if (Val > Max) return Max;
        else if (Val < Min) return Min;
        else return Val;
    }

    /// <summary>
    /// Determines if the directorty where path will allow us to read and write. 
    /// </summary>
    /// <param name="path">Install directory path</param>
    /// <returns>False if we cant write to the path.</returns>
    public static bool InstallDirSafe(string path) {
        try {
            File.Move(Path.Combine(path, "Tequila.exe"), Path.Combine(path, "Tequila_rename.exe"));
            if (File.Exists(Path.Combine(path, "Tequila_rename.exe")))
            {
                File.Move(Path.Combine(path, "Tequila_rename.exe"), Path.Combine(path, "Tequila.exe"));
                return true;
            } else {
                return false;
            }
        } catch (Exception ex) {
            return false;
        } 
    }

    public static bool CreateShortcut(string LinkPathName, string TargetPathName)
    {
        return true;
    }

    public static void ErrorReporter(Exception ex, string source) {
        System.Windows.Forms.MessageBox.Show(ex.Message, source);
    }


    public static void ActivityLog(string Line) {
        try
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(Tequila.Settings.GamePath, "TequilaActivityLog.txt"), true))
            {
                writer.WriteLine("[" + DateTime.Now.ToString() + "]\t" + Line);
            }
        }
        catch (Exception ex)
        {

        }
    }


}