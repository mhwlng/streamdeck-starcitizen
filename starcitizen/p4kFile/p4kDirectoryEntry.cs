using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SCJMapper_V2.p4kFile
{
  /// <summary>
  /// Represents a Directory entry in the p4k file
  /// The file seems to be based on a Zip64 file structure
  /// </summary>
  internal class p4kDirectoryEntry
  {

    /// <summary>
    /// ctor: Create class from data returned by the Reader; starts reading at current stream position
    /// </summary>
    /// <param name="reader">A binary data reader for this type of data - positioned already</param>
    public p4kDirectoryEntry( p4kRecReader reader )
    {
      // sanity check only
      System.Diagnostics.Trace.Assert( Marshal.SizeOf( typeof( MyRecord ) ) == RecordLength,
            "Record size does not match!(" + Marshal.SizeOf( typeof( MyRecord ) ).ToString( ) + ")" );
      System.Diagnostics.Trace.Assert( Marshal.SizeOf( typeof( MyZ64ExtraRecord ) ) == Z64ExtraRecordLength,
            "Extra Record size does not match!(" + Marshal.SizeOf( typeof( MyZ64ExtraRecord ) ).ToString( ) + ")" );

      if ( reader.IsOpen( ) ) {
        try {
          // get the next Directory record
          long cPos = p4kSignatures.FindSignatureInPage( reader, p4kSignatures.CentralDirRecord );
          if ( cPos >= 0 ) {
            m_recordOffset = cPos;
            reader.Seek( cPos );
            m_item = p4kRecReader.ByteToType<MyRecord>( reader.TheReader );
            m_itemValid = true; // implicite from Find..
          }

          // get some file attributes
          if ( m_itemValid ) {
            if ( m_item.FilenameLength > 0 ) {
              ReadFilename( reader );
            }
            if ( m_item.ExtraFieldLength > 0 ) {
              ReadExtradata( reader ); // Likely Zip64 extensions
            }
            m_fileDateTime = p4kFileTStamp.FromDos( m_item.LastModDate, m_item.LastModTime ); // get the file time given in the container

            // check if standard fields or extension is used for size information
            if ( m_item.CompressedSize < 0xffffffff ) {
              m_fileSizeComp = m_item.CompressedSize;
              m_fileSizeUnComp = m_item.UncompressedSize;
            }
            else {
              m_fileSizeComp = (long)m_z64Item.CompressedSize;
              m_fileSizeUnComp = (long)m_z64Item.UncompressedSize;
            }
          }
        }
#pragma warning disable CS0168 // Variable is declared but never used
        catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
          m_itemValid = false;
          m_recordOffset = -1;
        }
        finally {
          if ( !m_itemValid ) {
            throw new OperationCanceledException( string.Format( "EOF - cannot find CentralDirRec in this page" ) );
          }
        }
      }
    }

    /// <summary>
    /// Get the filename from the data
    /// </summary>
    /// <param name="reader">The open and positioned reader</param>
    private void ReadFilename( p4kRecReader reader )
    {
      byte[] fileNameBytes = new byte[m_item.FilenameLength];
      fileNameBytes = reader.ReadBytes( m_item.FilenameLength );
      m_filename = Encoding.ASCII.GetString( fileNameBytes );
    }

    /// <summary>
    /// Read the extra data
    /// </summary>
    /// <param name="reader"></param>
    private void ReadExtradata( p4kRecReader reader )
    {
      // first the Zip64 extra record
      m_z64Item = p4kRecReader.ByteToType<MyZ64ExtraRecord>( reader.TheReader );
      // then the rest of the extra record (is another item with tag 0x0666 and the rest lenght (ignored)
      m_extraBytes = new byte[m_item.ExtraFieldLength - Z64ExtraRecordLength];
      m_extraBytes = reader.ReadBytes( m_extraBytes.Length );
      m_extraBytes = null; // dump it ...
      // now we would be able to read the file content
    }

    /// <summary>
    /// Return the file related to this entry
    /// </summary>
    /// <param name="reader">An open p4k File reader</param>
    /// <returns>The content of the file or an empty string</returns>
    public byte[] GetFile( p4kRecReader reader )
    {
      if ( !m_itemValid ) return new byte[] { }; // ERROR cannot..

      reader.Seek( FileHeaderOffset );
      p4kFileHeader p4Kfh = new p4kFileHeader( reader );
      return p4Kfh.GetFile( reader );
    }


    public bool IsValid { get => m_itemValid; }
    public long RecordOffset { get => m_recordOffset; }
    public string Filename { get => m_filename; }
    public long FileSizeComp { get => m_fileSizeComp; }
    public long FileSizeUnComp { get => m_fileSizeUnComp; }
    public DateTime FileModifyDate { get => m_fileDateTime; }
    public long FileHeaderOffset
    {
      get {
        if ( m_itemValid )
          return (long)m_z64Item.LocalHeaderOffset;
        else
          return -1;
      }
    }

    private MyRecord m_item;
    private bool m_itemValid = false;
    private MyZ64ExtraRecord m_z64Item;
    private byte[] m_extraBytes = null;
    private long m_recordOffset = -1;  // offset from start (current position)
    // Entry attributes
    private string m_filename = "";
    private DateTime m_fileDateTime = new DateTime( 1970,1,1 );
    private long m_fileSizeComp = 0;
    private long m_fileSizeUnComp = 0;

    // 4.3.12  Central directory structure:
    private const int RecordLength = 46;
    [StructLayout( LayoutKind.Sequential, Pack = 1 )] // , Size = RecordLength
    private struct MyRecord
    {
      [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
      public byte[] ID;                   //      central file header signature   4 bytes(0x02014b50)
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 VersionMadeBy;        //     version made by                 2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 ExtractVersion;       //     version made by                 2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 BitFlags;             //      general purpose bit flag        2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 CompressionMethod;    //      compression method              2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 LastModTime;          //      last mod file time              2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 LastModDate;          //      last mod file date              2 bytes
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 CRC32;                //      crc-32                          4 bytes
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 CompressedSize;       //      compressed size                 4 bytes
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 UncompressedSize;     //      uncompressed size               4 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 FilenameLength;       //      file name length                2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 ExtraFieldLength;     //      extra field length              2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 FilecommentLength;    //      file comment length             2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 DiskNumberStart;      //      disk number start               2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 IntFileAttr;          //      internal file attributes        2 bytes
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 ExtFileAttr;          //      external file attributes        4 bytes
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 RelOffsetHeader;      //      relative offset of local header 4 bytes
                                          //      file name( variable size )
                                          //      extra field( variable size )
                                          //      file comment( variable size )
    }


    private const int Z64ExtraRecordLength = 32;
    [StructLayout( LayoutKind.Sequential, Pack = 1 )] // , Size = RecordLength
    private struct MyZ64ExtraRecord
    {
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 ID;                   // (Zip64 ExtraHeader Signature)
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 Size;                 // Size 2 bytes Size of this "extra" block
      [MarshalAs( UnmanagedType.U8 )]
      public UInt64 UncompressedSize;     // Original Size    8 bytes    Original uncompressed file size
      [MarshalAs( UnmanagedType.U8 )]
      public UInt64 CompressedSize;       // Compressed Size  8 bytes    Size of compressed data
      [MarshalAs( UnmanagedType.U8 )]
      public UInt64 LocalHeaderOffset;    // Relative Header Offset     8 bytes    Offset of local header record
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 DiskStart;            //  Disk Start Number     4 bytes    Number of the disk on which this file starts
    }


  }
}
