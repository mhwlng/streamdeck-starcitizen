using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCJMapper_V2.p4kFile
{
  /// <summary>
  /// General File descriptor - knows about locations and can extract files
  /// </summary>
  public class p4kFile
  {
    // File Properties 
    private string m_filename = "";
    public string Filename { get => m_filename; set => m_filename = value; }

    private long m_fileHeaderOffset = -1; // file location of the fileheader in the p4k file
    public long FileHeaderPosition { get => m_fileHeaderOffset; }

    private long m_compressedSize = -1;
    public long CompressedSize { get => m_compressedSize; }

    private long m_uncompressedSize = -1;
    public long FileSize { get => m_uncompressedSize; }

    private DateTime m_fileDateTime = new DateTime( 1970, 1, 1 );
    public DateTime FileModifyDate { get => m_fileDateTime; }

    private string m_fileDateTimeS = new DateTime( 1970, 1, 1 ).ToString("s"); // pre fabricated is faster to sort than always creating the string...
    public string FileModifyDateS { get => m_fileDateTimeS; }


    


    /// <summary>
    /// cTor: from fileHeader
    /// </summary>
    /// <param name="fileHeader">A File Header</param>
    internal p4kFile( p4kFileHeader fileHeader )
    {
      Filename = fileHeader.Filename;
      m_compressedSize = fileHeader.FileSizeComp;
      m_uncompressedSize = fileHeader.FileSizeUnComp;
      m_fileDateTime = fileHeader.FileModifyDate;
      m_fileDateTimeS = m_fileDateTime.ToString( "s" );
      m_fileHeaderOffset = fileHeader.RecordOffset;
    }


    /// <summary>
    /// cTor: from Directory Entry
    /// </summary>
    /// <param name="dirEntry">A Directory Entry</param>
    internal p4kFile( p4kDirectoryEntry dirEntry )
    {
      Filename = dirEntry.Filename;
      m_compressedSize = dirEntry.FileSizeComp;
      m_uncompressedSize = dirEntry.FileSizeUnComp;
      m_fileDateTime = dirEntry.FileModifyDate;
      m_fileDateTimeS = m_fileDateTime.ToString( "s" );
      m_fileHeaderOffset = dirEntry.FileHeaderOffset;
    }

    public p4kFile()
    {
    }


    /// <summary>
    /// Return the file related to this entry
    /// </summary>
    /// <param name="reader">An open p4k File reader</param>
    /// <returns>The content of the file or an empty string</returns>
    internal byte[] GetFile(  p4kRecReader reader )
    {
      if ( ( m_fileHeaderOffset >= 0 ) && reader.IsOpen( ) ) {
        // seek file header
        reader.Seek( m_fileHeaderOffset );
        byte[] fContent = { };
        // re-create the header
        using ( p4kFileHeader p4KFileHeader = new p4kFileHeader( reader ) ) {
          // dump the file
          fContent = p4KFileHeader.GetFile( reader );
        }
        return fContent;
      }
      return new byte[] { };
    }

  }
}
