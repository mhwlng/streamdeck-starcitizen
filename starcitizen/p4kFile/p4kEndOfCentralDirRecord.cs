using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SCJMapper_V2.p4kFile
{
  /// <summary>
  /// Represents an EndOfCentralDirRecord entry in the p4k file
  /// The file seems to be based on a Zip64 file structure
  /// </summary>
  internal class p4kEndOfCentralDirRecord
  {

    /// <summary>
    /// ctor: Create class from data returned by the Reader
    /// </summary>
    /// <param name="reader">A binary data reader for this type of data - positioned at last page</param>
    public p4kEndOfCentralDirRecord( p4kRecReader reader )
    {
      // sanity check only
      System.Diagnostics.Trace.Assert( Marshal.SizeOf( typeof( MyRecord ) ) == RecordLength,
            "Record size does not match!(" + Marshal.SizeOf( typeof( MyRecord ) ).ToString( ) + ")" );

      if ( reader.IsOpen( ) ) {
        try {
          long thisPos = 0;
          // 20180804BM- fix when the last page is barely used and the sig is one or more pages before 
          //  - backup one or more pages to find end of dir (max 10 times)
          int tries = 10;
          while ( !m_itemValid && ( tries > 0 ) ) {
            thisPos = reader.Position;
            long cPos = p4kSignatures.FindSignatureInPage( reader, p4kSignatures.EndOfCentralDirRecord );
            if ( cPos >= 0 ) {
              m_recordOffset = cPos;
              reader.Seek( cPos );
              m_item = p4kRecReader.ByteToType<MyRecord>( reader.TheReader );
              m_itemValid = true;
            }
            else {
              // backup one page again
              reader.Seek( thisPos );
              reader.BackwardPage( );
              tries--;
            }
          }
        }
        catch {
          m_itemValid = false;
          m_recordOffset = -1;
        }
        finally {
          if ( !m_itemValid ) {
            throw new OperationCanceledException( string.Format( "EOF - cannot find EndOfCentralDirRecord" ) );
          }
        }
      }
    }

    public bool IsValid { get => m_itemValid; }
    public long RecordOffset { get => m_recordOffset; }


    private MyRecord m_item;
    private bool m_itemValid = false;
    private long m_recordOffset = -1;  // offset from start (current position)

    //4.3.16  End of central directory record:
    private const int RecordLength = 22;
    [StructLayout( LayoutKind.Sequential, Pack = 1 )] // , Size = RecordLength
    private struct MyRecord
    {
      [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
      public byte[] ID;                   //   end of central dir signature    4 bytes(0x06054b50)
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 DiskNumber;           //   number of this disk             2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 DiskNumbersFromStart; //   number of the disk with the start of the central directory  2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 NumEntriesOnDisk;     //   total number of entries in the central directory on this disk  2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 TotalNumEntries;      //   total number of entries in the central directory           2 bytes
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 SizeOfCDir;           //   size of the central directory   4 bytes
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 OffsetOfCDir;         //   offset of start of central directory with respect to the starting disk number        4 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 CommentLen;           //   .ZIP file comment length        2 bytes
                                          //   .ZIP file comment( variable size )
    }



  }
}
