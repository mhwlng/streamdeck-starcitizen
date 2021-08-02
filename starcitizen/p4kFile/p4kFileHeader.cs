using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ZstdNet;

namespace SCJMapper_V2.p4kFile
{
  /// <summary>
  /// Represents a Fileheader entry in the p4k File
  /// seems to be a Zip64 based file and therefore using those headers
  /// </summary>
  internal class p4kFileHeader : IDisposable
  {

    //4.3.7  Local file header:

    //   local file header signature     4 bytes(0x04034b50)
    //   version needed to extract       2 bytes
    //   general purpose bit flag        2 bytes
    //   compression method              2 bytes
    //   last mod file time              2 bytes
    //   last mod file date              2 bytes
    //   crc-32                          4 bytes
    //   compressed size                 4 bytes
    //   uncompressed size               4 bytes
    //   file name length                2 bytes
    //   extra field length              2 bytes

    //   file name( variable size )
    //   extra field( variable size )

    //4.3.8  File data

    //   Immediately following the local header for a file
    //   SHOULD be placed the compressed or stored data for the file.
    //   If the file is encrypted, the encryption header for the file
    //   SHOULD be placed after the local header and before the file
    //   data. The series of[local file header][encryption header]
    //   [file data][data descriptor] repeats for each file in the
    //   .ZIP archive. 

    //   Zero-byte files, directories, and other file types that
    //   contain no content MUST not include file data.

    //   4.5.3 -Zip64 Extended Information Extra Field(0x0001):

    //      The following is the layout of the zip64 extended
    //      information "extra" block.If one of the size or
    //      offset fields in the Local or Central directory
    //      record is too small to hold the required data,
    //      a Zip64 extended information record is created.
    //      The order of the fields in the zip64 extended
    //      information record is fixed, but the fields MUST
    //      only appear if the corresponding Local or Central
    //      directory record field is set to 0xFFFF or 0xFFFFFFFF.

    //      Note: all fields stored in Intel low - byte / high - byte order.

    //        Value      Size       Description
    //        ---- - ---------------
    //(ZIP64)0x0001     2 bytes    Tag for this "extra" block type
    //        Size       2 bytes    Size of this "extra" block
    //        Original
    //        Size       8 bytes    Original uncompressed file size
    //        Compressed
    //        Size       8 bytes    Size of compressed data
    //        Relative Header
    //        Offset     8 bytes    Offset of local header record
    //        Disk Start
    //        Number     4 bytes    Number of the disk on which
    //                              this file starts

    //      This entry in the Local header MUST include BOTH original
    //      and compressed file size fields.If encrypting the
    //      central directory and bit 13 of the general purpose bit
    //      flag is set indicating masking, the value stored in the
    //      Local Header for the original file size will be zero.

    /// <summary>
    /// ctor: Create class from data returned by the Reader
    /// </summary>
    /// <param name="reader">A binary data reader for this type of data</param>
    public p4kFileHeader( p4kRecReader reader )
    {
      // sanity check only
      System.Diagnostics.Trace.Assert( Marshal.SizeOf( typeof( MyRecord ) ) == RecordLength,
            "Record size does not match!(" + Marshal.SizeOf( typeof( MyRecord ) ).ToString( ) + ")" );
      System.Diagnostics.Trace.Assert( Marshal.SizeOf( typeof( MyZ64ExtraRecord ) ) == Z64ExtraRecordLength,
            "Extra Record size does not match!(" + Marshal.SizeOf( typeof( MyZ64ExtraRecord ) ).ToString( ) + ")" );

      if ( reader.IsOpen( ) ) {
        try {
          long cPos = reader.Position;
          do {
            // Fileheaders are Page aligned - scan to find one
            reader.AdvancePage( ); // to next page
            cPos = reader.Position;
            string cPosS = cPos.ToString( "X" );
            m_recordOffset = cPos;
            m_item = p4kRecReader.ByteToType<MyRecord>( reader.TheReader );
            m_itemValid = m_item.ID.SequenceEqual( p4kSignatures.LocalFileHeaderCry );
          } while ( ( cPos < reader.Length ) && !m_itemValid );

          // get some file attributes
          if ( m_itemValid ) {
            if ( m_item.FilenameLength > 0 ) {
              ReadFilename( reader );
            }
            if ( m_item.ExtraFieldLength > 0 ) {
              ReadExtradata( reader ); // Likely Zip64 extensions
            }
            m_fileDateTime = p4kFileTStamp.FromDos( m_item.LastModDate, m_item.LastModTime );

            // check if standard fields or extension is used for size information
            if ( m_item.CompressedSize < 0xffffffff ) {
              m_fileSizeComp = m_item.CompressedSize;
              m_fileSizeUnComp = m_item.UncompressedSize;
            }
            else {
              m_fileSizeComp = (long)m_z64Item.CompressedSize;
              m_fileSizeUnComp = (long)m_z64Item.UncompressedSize;
            }
            // now we would be able to read the file content
            // but we skip it for now to process the next header
            m_fileOffset = reader.TheReader.BaseStream.Position; // save position of this item
            reader.TheReader.BaseStream.Seek( m_fileSizeComp, SeekOrigin.Current );
          }
          else {
            // actually invalid but good manner ..
            m_recordOffset = -1;
            m_fileOffset = -1;
            m_fileSizeComp = 0;
            m_fileSizeUnComp = 0;
          }

        }
        catch {
          m_itemValid = false;
        }
        finally {
          if ( !m_itemValid ) {
            if ( m_item.ID.SequenceEqual( p4kSignatures.CentralDirRecord ) ) {
              // read beyond the file entries
              throw new OperationCanceledException( string.Format( "EOF - found Central Directory header {0}", m_item.ID.ToString( ) ) );
            }
            else {
              // other error
              throw new NotSupportedException( string.Format( "Cannot process fileheader ID {0}", m_item.ID.ToString( ) ) );
            }
          }
        }
      }
    }


    /// <summary>
    /// Return the file related to this entry
    /// </summary>
    /// <param name="reader">An open p4k File reader</param>
    /// <returns>The content of the file or an empty string</returns>
    public byte[] GetFile(  p4kRecReader reader )
    {
      if ( !m_itemValid ) return new byte[] { }; // ERROR cannot..

      reader.Seek( m_fileOffset );
      // ?? big files may have trouble here - may be we need to read and write chunks for this
      // but for now we only want to get XMLs out and that is OK with byte alloc on the heap
      byte[] fileBytes = new byte[m_fileSizeComp]; 
      fileBytes = reader.ReadBytes( fileBytes.Length );

      byte[] decompFile = null;
      if ( m_item.CompressionMethod == 0x64 ) {
        // this indicates p4k ZStd compression
        using ( var decompressor = new Decompressor( ) ) {
          try {
            decompFile = decompressor.Unwrap( fileBytes );
            return decompFile;
          }
          catch ( ZstdException e ) {
            Console.WriteLine( "ZStd - Cannot decode file: " + m_filename );
            Console.WriteLine( "Error: " + e.Message );
            //Console.ReadLine();
            return  new byte[] { };
          }
        }
      }
      else {
        // plain write - might be wrong if another compression was applied..
        decompFile = fileBytes;
        return decompFile;
      }
    }


    public bool IsValid { get => m_itemValid; }
    public string Filename { get => m_filename; }
    public long RecordOffset { get => m_recordOffset; }
    public long FileOffset { get => m_fileOffset; }
    public long FileSizeComp { get => m_fileSizeComp; }
    public long FileSizeUnComp { get => m_fileSizeUnComp; }
    public DateTime FileModifyDate { get => m_fileDateTime; }



    private void ReadFilename( p4kRecReader reader )
    {
      byte[] fileNameBytes = new byte[m_item.FilenameLength];
      fileNameBytes = reader.ReadBytes( m_item.FilenameLength );
      m_filename = Encoding.ASCII.GetString( fileNameBytes );
    }

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


    private MyRecord m_item;
    private bool m_itemValid = false;
    private MyZ64ExtraRecord m_z64Item;
    private byte[] m_extraBytes = null;
    private long m_recordOffset = -1;  // offset from start (current position)
    private long m_fileOffset = 0;  // offset from start (current position)
    // Entry attributes
    private string m_filename = "";
    private DateTime m_fileDateTime = new DateTime( 1970, 1, 1 );
    private long m_fileSizeComp = 0;
    private long m_fileSizeUnComp = 0;



    private const int RecordLength = 30;
    [StructLayout( LayoutKind.Sequential, Pack = 1 )] // , Size = RecordLength
    private struct MyRecord
    {
      [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
      public byte[] ID;                   //      local file header signature     4 bytes(0x14034b50) NOTE p4k here uses 0x14 and not 0x04 
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 ExtractVersion;       //      version made by                 2 bytes
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

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose( bool disposing )
    {
      if ( !disposedValue ) {
        if ( disposing ) {
          // TODO: dispose managed state (managed objects).
          m_itemValid = false;
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.
        m_extraBytes = null;

        disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~p4kFileHeader() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose( true );
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
    #endregion

  }
}
