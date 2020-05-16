using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public class DownloadURL
{
    public DownloadURL(string URL)
    {
        m_URL = URL.Trim();
        m_PullCount = 0;
    }
    
    public string URL { get { return URL;} }
    public int PullCount { get { return m_PullCount; } }
    
    // This is different from the URL property in that it will keep a count. 
    // Sometimes we will want to remove URLs that have been pulled too many times.
    public string PullURL()
    {
        m_PullCount++;
        return m_URL; 
    }
    
    protected string m_URL = "";
    protected int m_PullCount = 0; 
}

/************************************************************************
*  This class exists for two purposes: either give it a path and it     *
*  will automatically load a file size and md5 checsum, or give         *
*  it all the size and checksum data in the constructor to package      *
*  pre-defined checksums, loaded possibly from a file.                  *
************************************************************************/
public class Fingerprint
{
    private string m_FileName;
    private string m_RootPath;
    private long m_Size;
    private string m_Checksum;
    private bool m_mismatch = false;
    //private ArrayList m_DownloadURLs = new ArrayList();
    private List<DownloadURL> m_DownloadURLs = new List<DownloadURL>();
    private bool m_warn = true;
    private static Random rand = new Random();

    /**********************************************************************/
    public string FileName { get { return m_FileName; } }
    public string RootPath { get { return m_RootPath; } }
    public string FullName { get { return MyToolkit.ValidPath(Path.Combine(m_RootPath, m_FileName)); } }
    public string AbsolutePath { get { return Path.GetFullPath(FullName); } }
    public bool PathIsSafe { get { return MyToolkit.CheckFileName(m_RootPath, m_FileName); } }
    public long Size { get { return m_Size; } }
    public string Checksum { get { return m_Checksum; } }
    public bool Mismatch { get { return m_mismatch; } set { m_mismatch = value; } }
    public bool Warn { get { return m_warn; } set { m_warn = Warn; } }

    public string DownloadURL { 
        get {
            if (m_DownloadURLs.Count >= 1)
            {
                int randomIndex = rand.Next(0, m_DownloadURLs.Count);

                // Pull the random URL
                string returnURL = m_DownloadURLs[randomIndex].PullURL();
                
                // If this URL has been pulled 5 times or more, remove it from the list
                if (m_DownloadURLs[randomIndex].PullCount >= 5) {
                    m_DownloadURLs.RemoveAt(randomIndex);
                }

                return returnURL;
            } else {
                return "";
            }
        }
    }

    /**********************************************************************/

    public Fingerprint(string RootPath, string FileName) {
        try {
            m_RootPath = RootPath;
            m_FileName = FileName.Replace(".EXE", ".exe");
            if (File.Exists(FullName)) m_Size = (new FileInfo(FullName)).Length;
            else m_Size = 0;
            m_Checksum = GenerateHash();
        } catch (Exception ex) {
            MyToolkit.ErrorReporter(ex, "Fingerprint.Constructor1");
        }
    }

    public Fingerprint(string RootPath, string FileName, string Checksum)
    {
        try{
            m_RootPath = RootPath;
            m_FileName = FileName.Replace(".EXE", ".exe");
		
            // Load size from the file info
            m_Size = (new FileInfo(FileName)).Length;

            // Make sure the checksum is lowercase
            m_Checksum = Checksum.ToLower();
        } catch (Exception ex) {
            MyToolkit.ErrorReporter(ex, "Fingerprint.Constructor2");
        }
    }

    public Fingerprint(string RootPath, string FileName, string Checksum, long Size)
    {
        try
        {
            m_RootPath = RootPath;
            m_FileName = FileName.Replace(".EXE", ".exe");

            // Make sure the checksum is lowercase
            m_Checksum = Checksum.ToLower();

            // ... yea... overcommenting!
            m_Size = Size;
        } catch (Exception ex) {
            MyToolkit.ErrorReporter(ex, "Fingerprint.Constructor2");
        }
    }

    /**********************************************************************/

    public void AddDownloadURL(string URL) {
        m_DownloadURLs.Add(new DownloadURL(URL));
    }

    /// <summary>
    /// Compare to another Fingerprint object
    /// </summary>
    /// <param name="other">The object to compare to</param>
    /// <returns>True if they are equal, false otherwise.</returns>

    public bool Equals(Fingerprint other)
    {
        if (this.Size != other.Size) return false;
        if (this.Checksum != other.Checksum) return false;
        return true;
    }

    /// <summary>
    /// Generates md5 checksum.
    /// </summary>
    /// <returns>MD5 checksum</returns>
    public string GenerateHash(string path)
    {
        try
        {
            MD5 md5Hash = MD5.Create();

            var buffer = md5Hash.ComputeHash(File.ReadAllBytes(path));

            var cs = new StringBuilder();

            for (int i = 0; i < buffer.Length; i++)
            {
                cs.Append(buffer[i].ToString("x2"));
            }

            // Return an uppercase version of the consolidated string.
            return cs.ToString().ToLower();
        }
        catch (Exception ex)
        {
            MyToolkit.ErrorReporter(ex, "Fingerprint.GenerateHash");
            return "";
        }
    }

    /// <summary>
    /// Short form overload that will use the member variable instead of taking it as a parameter.
    /// </summary>
    /// <returns>An md5 checksum, split in 4 chunks, each chunk inverted.</returns>
    public string GenerateHash()
    {
        return GenerateHash(FullName);
    }
    
}