using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;


namespace SCJMapper_V2.p4kFile
{


  /// <summary>
  /// Limited Directory scanner for p4k files
  /// </summary>
  public class p4kDirectory
  {
        //  4.3.6 Overall.ZIP file format:
        //[local file header 1]
        //[encryption header 1]
        //[file data 1]
        //[data descriptor 1]
        //  . 
        //  .
        //  .
        //[local file header n]
        //[encryption header n]
        //[file data n]
        //[data descriptor n]

        //[archive decryption header]
        //[archive extra data record]

        //[central directory header 1]
        //  .
        //  .
        //  .
        //[central directory header n]
        //[zip64 end of central directory record]
        //[zip64 end of central directory locator]
        //[end of central directory record]


    private static readonly byte[] Key = new byte[] { 0x5E, 0x7A, 0x20, 0x02, 0x30, 0x2E, 0xEB, 0x1A, 0x3B, 0xB6, 0x17, 0xC3, 0x0F, 0xDE, 0x1E, 0x47 };

    public byte[] GetFile(string p4kFilename, p4kFile file)
    {
        if (!File.Exists(p4kFilename)) return new byte[] { };
        /*
        using ( p4kRecReader reader = new p4kRecReader( p4kFilename ) ) {
          return file.GetFile( reader );
        } */

        using (var pakFile = File.OpenRead(p4kFilename))
        {
            var pak = new ZipFile(pakFile) { Key = Key };

            var entry = pak.GetEntry(file.Filename.Replace("\\", "/"));

            using (Stream s = pak.GetInputStream(entry))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    s.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }

    // scans file directory entries
    // finds the directory structs and processes from there

    private p4kEndOfCentralDirRecord m_endOfCentralDirRecord = null;
    private p4kZ64EndOfCentralDirLocator m_z64EndOfCentralDirLocator = null;
    private p4kZ64EndOfCentralDirRecord m_z64EndOfCentralDirRecord = null;

    /// <summary>
    /// Scans directory entries and return the a file descriptor (string.EndsWith is used)
    /// </summary>
    /// <param name="p4kFilename">The p4k file</param>
    /// <param name="filename">The filename to look for</param>
    public p4kFile ScanDirectoryFor( string p4kFilename, string filename )
    {
      if ( !File.Exists( p4kFilename ) ) return null;

      using ( p4kRecReader reader = new p4kRecReader( p4kFilename ) ) {
        // work from the end of the file
        reader.GotoLastPage( );
        m_endOfCentralDirRecord = new p4kEndOfCentralDirRecord( reader );

        // position first
        reader.Seek( m_endOfCentralDirRecord.RecordOffset - p4kRecReader.PageSize );
        m_z64EndOfCentralDirLocator = new p4kZ64EndOfCentralDirLocator( reader );

        // for the next the position should be found already - seek it
        reader.Seek( m_z64EndOfCentralDirLocator.Z64EndOfCentralDir );
        m_z64EndOfCentralDirRecord = new p4kZ64EndOfCentralDirRecord( reader );
        // now we should have the start of the directory entries...


        // position first
        reader.Seek( m_z64EndOfCentralDirRecord.Z64StartOfCentralDir );
        // loop all file - as per dir reporting
        for ( long i = 0; i < m_z64EndOfCentralDirRecord.NumberOfEntries; i++ ) {
          p4kDirectoryEntry de = new p4kDirectoryEntry( reader );
          if ( !string.IsNullOrEmpty( filename ) && de.Filename.ToLower( ).EndsWith( filename.ToLower( ) ) ) {
            var p = new p4kFile( de ); // FOUND
            reader.TheReader.Close( );
            return p; // bail out if found
          }
        }
      }
      return null;
    }


    /// <summary>
    /// Scans directory entries and return the a list of matching file descriptors (string.Contains is used)
    /// </summary>
    /// <param name="p4kFilename">The p4k file</param>
    /// <param name="filenamepart">The filename part to look for</param>
    public IList<p4kFile> ScanDirectoryContaining( string p4kFilename, string filenamepart )
    {
      if ( !File.Exists( p4kFilename ) ) return null;

      List<p4kFile> fileList = new List<p4kFile>( );

      using ( p4kRecReader reader = new p4kRecReader( p4kFilename ) ) {
        // work from the end of the file
        reader.GotoLastPage( );
        m_endOfCentralDirRecord = new p4kEndOfCentralDirRecord( reader );

        // position first
        reader.Seek( m_endOfCentralDirRecord.RecordOffset - p4kRecReader.PageSize );
        m_z64EndOfCentralDirLocator = new p4kZ64EndOfCentralDirLocator( reader );

        // for the next the position should be found already - seek it
        reader.Seek( m_z64EndOfCentralDirLocator.Z64EndOfCentralDir );
        m_z64EndOfCentralDirRecord = new p4kZ64EndOfCentralDirRecord( reader );
        // now we should have the start of the directory entries...

        // position first
        reader.Seek( m_z64EndOfCentralDirRecord.Z64StartOfCentralDir );
        // loop all file - as per dir reporting
        for ( long i = 0; i < m_z64EndOfCentralDirRecord.NumberOfEntries; i++ ) {
          p4kDirectoryEntry de = new p4kDirectoryEntry( reader );
          if ( !string.IsNullOrEmpty( filenamepart ) && de.Filename.ToLower( ).Contains( filenamepart.ToLower( ) ) ) {
            var p = new p4kFile( de ); // FOUND
            fileList.Add( p );
          }
        }
      }
      return fileList;
    }


  }
}
