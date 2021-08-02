using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SCJMapper_V2.p4kFile
{
  /// Represents an Z64EndOfCentralDirRecord entry in the p4k file
  /// The file seems to be based on a Zip64 file structure
  internal class p4kZ64EndOfCentralDirRecord
  {
    /// <summary>
    /// ctor: Create class from data returned by the Reader
    /// </summary>
    /// <param name="reader">A binary data reader for this type of data - positioned already</param>
    public p4kZ64EndOfCentralDirRecord( p4kRecReader reader )
    {
      // sanity check only
      System.Diagnostics.Trace.Assert( Marshal.SizeOf( typeof( MyRecord ) ) == RecordLength,
            "Record size does not match!(" + Marshal.SizeOf( typeof( MyRecord ) ).ToString( ) + ")" );

      if ( reader.IsOpen( ) ) {
        try {
          long cPos = p4kSignatures.FindSignatureInPage( reader, p4kSignatures.Z64EndOfCentralDirRec );
          if ( cPos >= 0 ) {
            m_recordOffset = cPos;
            reader.Seek( cPos );
            m_item = p4kRecReader.ByteToType<MyRecord>( reader.TheReader );
            m_itemValid = true;
          }
        }
        catch {
          m_itemValid = false;
          m_recordOffset = -1;
        }
        finally {
          if ( !m_itemValid ) {
            throw new OperationCanceledException( string.Format( "EOF - cannot find Z64EndOfCentralDirRecord" ) );
          }
        }
      }
    }

    public bool IsValid { get => m_itemValid; }
    public long RecordOffset { get => m_recordOffset; }
    public long Z64StartOfCentralDir
    {
      get {
        if ( m_itemValid )
          return (long)m_item.OffsetOfZ64CDir;
        else
          return -1;
      }
    }
    public long NumberOfEntries
    {
      get {
        if ( m_itemValid )
          return (long)m_item.NumEntriesTotal;
        else
          return -1;
      }
    }

    private MyRecord m_item;
    private bool m_itemValid = false;
    private long m_recordOffset = -1;  // offset from start (current position)


    //4.3.14  Zip64 end of central directory record
    private const int RecordLength = 56;
    [StructLayout( LayoutKind.Sequential, Pack = 1 )] // , Size = RecordLength
    private struct MyRecord
    {
      [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
      public byte[] ID;                   //   zip64 end of central dir signature              4 bytes(0x06064b50)
      [MarshalAs( UnmanagedType.U8 )]
      public UInt64 SizeOfZ64CDir;        //   size of zip64 end of central directory record   8 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 VersionMadeBy;        //     version made by                 2 bytes
      [MarshalAs( UnmanagedType.U2 )]
      public UInt16 ExtractVersion;       //     version made by                 2 bytes
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 DiskNumber;           //     number of this disk             4 bytes
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 DiskNumbersFromStart; //     number of the disk with the start of the central directory  4 bytes
      [MarshalAs( UnmanagedType.U8 )]
      public UInt64 NumEntriesOnDisk;     //     total number of entries in the central directory on this disk  8 bytes
      [MarshalAs( UnmanagedType.U8 )]
      public UInt64 NumEntriesTotal;      //     total number of entries in the central directory               8 bytes
      [MarshalAs( UnmanagedType.U8 )]
      public UInt64 SizeOfCDir;           //     size of the central directory   8 bytes
      [MarshalAs( UnmanagedType.U8 )]
      public UInt64 OffsetOfZ64CDir;      //     offset of start of central directory with respect to the starting disk number        8 bytes
                                          //     zip64 extensible data sector    (variable size)
    }



  }
}
