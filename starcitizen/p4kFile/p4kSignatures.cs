using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCJMapper_V2.p4kFile
{
  internal class p4kSignatures
  {

    // From PKWare APPNOTE.TXT
    //4.3.6 Overall.ZIP file format:

    // [local file header 1]          LocalFileHeader OR LocalFileHeaderCry for this p4k version
    // [encryption header 1]
    // [file data 1]
    // [data descriptor 1]
    //   . 
    //   .
    //   .
    // [local file header n]
    // [encryption header n]
    // [file data n]
    // [data descriptor n]
    // [archive decryption header]      CentralDirRec
    // [archive extra data record]      ExtraDataRecord
    // [central directory header 1]     CentralDirRec
    //   .
    //   .
    //   .
    // [central directory header n]     CentralDirRec
    // [zip64 end of central directory record]    Z64CentralDirRecEnd
    // [zip64 end of central directory locator]   Z64CentralDirLocEnd
    // [end of central directory record]          CentralDirRecEnd


    public static readonly byte[] LocalFileHeader = { 0x50, 0x4B, 0x03, 0x04 };        // (0x04034b50) 4.3.7  Local file header:
    public static readonly byte[] LocalFileHeaderCry = { 0x50, 0x4B, 0x03, 0x14 };     // as found in the p4k files
    public static readonly byte[] ExtraDataRecord = { 0x50, 0x4B, 0x06, 0x08 };        // (0x08064b50) 4.3.11  Archive extra data record: 
    public static readonly byte[] CentralDirRecord = { 0x50, 0x4B, 0x01, 0x02 };          // (0x02014b50) 4.3.12  Central directory structure:
    public static readonly byte[] DigitalSignature = { 0x50, 0x4B, 0x05, 0x05 };       // (0x05054b50) 4.3.13 Digital signature:
    public static readonly byte[] Z64EndOfCentralDirRec = { 0x50, 0x4B, 0x06, 0x06 };    // (0x06064b50) 4.3.14  Zip64 end of central directory record
    public static readonly byte[] Z64EndOfCentralDirLocator = { 0x50, 0x4B, 0x06, 0x07 };    // (0x07064b50) 4.3.15 Zip64 end of central directory locator
    public static readonly byte[] EndOfCentralDirRecord = { 0x50, 0x4B, 0x05, 0x06 };       // (0x06054b50) 4.3.16  End of central directory record:

    /// <summary>
    /// Returns the position of the Signature within the stream
    ///  Searches one page from current location
    /// </summary>
    /// <param name="reader">A positioned reader</param>
    /// <returns>The position within the stream or -1 if not found</returns>
    public static long FindSignatureInPage( p4kRecReader reader, byte[] signature )
    {
      long pos = reader.Position;
      byte[] lPage = reader.GetPage( );
      for ( int i = 0; i < lPage.Length - 4; i++ ) {
        if ( lPage.Skip( i ).Take( 4 ).SequenceEqual( signature ) ) {
          // now this should be the start of the item
          return pos + i;
        }
      }
      return -1; // not found...
    }

    /// <summary>
    /// Returns the position of the Signature within the stream
    ///  Searches one page from current location starting at the end of the page
    /// </summary>
    /// <param name="reader">A positioned reader</param>
    /// <returns>The position within the stream or -1 if not found</returns>
    public static long FindSignatureInPageBackwards( p4kRecReader reader, byte[] signature )
    {
      long pos = reader.Position;
      byte[] lPage = reader.GetPage( );
      for ( int i = lPage.Length - 4; i > 0; i-- ) {
        if ( lPage.Skip( i ).Take( 4 ).SequenceEqual( signature ) ) {
          // now this should be the start of the item [end of central directory record]
          return pos + i;
        }
      }
      return -1; // not found...
    }



  }
}
