using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SCJMapper_V2.p4kFile
{
  /// Represents an Z64EndOfCentralDirLocator entry in the p4k file
  /// The file seems to be based on a Zip64 file structure
  internal class p4kZ64EndOfCentralDirLocator
  {
    /// <summary>
    /// ctor: Create class from data returned by the Reader
    /// </summary>
    /// <param name="reader">A binary data reader for this type of data already positioned</param>
    public p4kZ64EndOfCentralDirLocator( p4kRecReader reader )
    {
      // sanity check only
      System.Diagnostics.Trace.Assert( Marshal.SizeOf( typeof( MyRecord ) ) == RecordLength,
            "Record size does not match!(" + Marshal.SizeOf( typeof( MyRecord ) ).ToString( ) + ")" );

      if ( reader.IsOpen( ) ) {
        try {
          long cPos = p4kSignatures.FindSignatureInPageBackwards( reader, p4kSignatures.Z64EndOfCentralDirLocator );
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
            throw new OperationCanceledException( string.Format( "EOF - cannot find Z64EndOfCentralDirLocator" ) );
          }
        }
      }
    }

    public bool IsValid { get => m_itemValid; }
    public long RecordOffset { get => m_recordOffset; }
    public long Z64EndOfCentralDir
    {
      get {
        if ( m_itemValid )
          return (long)m_item.OffsetOfz64EofCDir;
        else
          return -1;
      }
    }

    private MyRecord m_item;
    private bool m_itemValid = false;
    private long m_recordOffset = -1;  // offset from start (current position)


    //4.3.15 Zip64 end of central directory locator
    private const int RecordLength = 20;
    [StructLayout( LayoutKind.Sequential, Pack = 1 )] // , Size = RecordLength
    private struct MyRecord
    {
      [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
      public byte[] ID;                   //   zip64 end of central dir locator signature        4 bytes(0x07064b50)
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 DiskNumber;           //   number of the disk with the start of the zip64 end of central directory     4 bytes
      [MarshalAs( UnmanagedType.U8 )]
      public UInt64 OffsetOfz64EofCDir;   //   relative offset of the zip64 end of central directory record 8 bytes (pts to Z64CentralDirRecEnd)
      [MarshalAs( UnmanagedType.U4 )]
      public UInt32 TotalNumEntries;      //   total number of disks           4 bytes
    }



  }
}
