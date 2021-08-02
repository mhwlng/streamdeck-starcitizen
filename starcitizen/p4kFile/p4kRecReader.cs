using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SCJMapper_V2.p4kFile
{
  /// <summary>
  /// Implements a binary reader for p4k files
  /// </summary>
  class p4kRecReader : IDisposable
  {
    public const long PageSize = 0x1000;

    private FileStream m_filestr = null;
    private BinaryReader m_reader = null;

    private DateTime m_fileCreatedT;  // hold the file creation time


    /// <summary>
    /// ctor: 
    /// </summary>
    /// <param name="filename">The filename</param>
    public p4kRecReader( string filename )
    {
      Open( filename );
      m_fileCreatedT = File.GetCreationTimeUtc( m_filestr.Name );
    }

    /// <summary>
    /// Open the file
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <returns>True when successfull</returns>
    private bool Open( string filename )
    {
      if ( File.Exists( filename ) ) {
        m_filestr = File.OpenRead( filename );
        m_reader = new BinaryReader( m_filestr );
        return true;
      }

      return false;
    }


    /// <summary>
    /// Returns the Reader
    /// </summary>
    public BinaryReader TheReader
    {
      get { return m_reader; }
    }


    /// <summary>
    /// Allocates and reads bytes of the size of one record 
    /// and returns the allocated bytes are structure - allowing structured access to binary data 
    /// Note: there is no error checking whatsoever here - so better make sure everything is OK
    /// </summary>
    /// <typeparam name="T">The record type to read</typeparam>
    /// <param name="reader">A binary reader</param>
    /// <returns>The read record</returns>
    public static T ByteToType<T>( BinaryReader reader )
    {
      byte[] bytes = reader.ReadBytes( Marshal.SizeOf( typeof( T ) ) );

      GCHandle handle = GCHandle.Alloc( bytes, GCHandleType.Pinned );
      T theStructure = (T)Marshal.PtrToStructure( handle.AddrOfPinnedObject( ), typeof( T ) );
      handle.Free( );

      return theStructure;
    }

    /// <summary>
    /// Returns the file status
    /// </summary>
    /// <returns>True if the file is open and the reader can read</returns>
    public bool IsOpen()
    {
      if ( m_filestr != null ) return m_filestr.CanRead;
      else return false;
    }

    /// <summary>
    /// Returns the current Position of the underlying stream
    /// </summary>
    public long Position { get => m_filestr.Position; }

    /// <summary>
    /// Returns the Length of the underlying stream
    /// </summary>
    public long Length { get => m_filestr.Length; }

    /// <summary>
    /// Returns a number of bytes as array (from the underlying reader)
    /// </summary>
    /// <param name="count">Number of bytes to read</param>
    /// <returns>A byte array</returns>
    public byte[] ReadBytes(int count)
    {
      return m_reader.ReadBytes( count );
    }


    /// <summary>
    /// Seeks from to pos from beginning
    /// </summary>
    /// <param name="pos">A position in the filestream</param>
    /// <returns>The current starting position before seek</returns>
    public long Seek( long pos )
    {
      long thisPos = m_filestr.Position;
      m_filestr.Seek( pos, SeekOrigin.Begin );
      return thisPos;
    }

    /// <summary>
    /// As the file is 4k page oriented - this advances to the next page
    /// </summary>
    public void AdvancePage()
    {
      long current = TheReader.BaseStream.Position;
      long remainder = current % PageSize;
      if ( remainder > 0 ) {
        long seek = PageSize - remainder;
        TheReader.BaseStream.Seek( seek, SeekOrigin.Current );
      }
    }

    /// <summary>
    /// As the file is 4k page oriented - this backups to the previous page
    /// </summary>
    public void BackwardPage()
    {
      long current = TheReader.BaseStream.Position;
        long seek = -PageSize;
        TheReader.BaseStream.Seek( seek, SeekOrigin.Current );
    }

    /// <summary>
    /// Places the stream at the last Page of the file
    /// </summary>
    /// <returns>Position of the stream</returns>
    public long GotoLastPage()
    {
      TheReader.BaseStream.Seek( -PageSize, SeekOrigin.End );
      return TheReader.BaseStream.Position;
    }

    /// <summary>
    /// Returns one Page of the file @ current pos
    /// </summary>
    /// <returns>PageSize bytes at the current pos of the stream</returns>
    public byte[] GetPage()
    {
      return TheReader.ReadBytes( (int)PageSize );
    }

    /// <summary>
    /// Returns the file creation timestamp
    /// </summary>
    public DateTime FileDateTime
    {
      get { return m_fileCreatedT; }
      set {; }
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose( bool disposing )
    {
      if ( !disposedValue ) {
        if ( disposing ) {
          // TODO: dispose managed state (managed objects).
          if ( m_reader != null ) {
            m_reader.Close( );
            m_reader.Dispose( );
            m_reader = null;
          }
          if ( m_filestr != null ) {
            m_filestr.Close( );
            m_filestr.Dispose( );
            m_filestr = null;
          }
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~p4kRecReader() {
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
