using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SCJMapper_V2.CryXMLlib
{
  internal static class Extensions
  {
    /// <summary>
    /// Get the array slice between the two indexes.
    /// ... Inclusive for start index, exclusive end
    /// </summary>
    public static T[] SliceE<T>( this T[] source, UInt32 start, UInt32 end )
    {
      // Handles negative ends.
      if ( end < 0 ) {
        end = ( UInt32 )source.Length + end;
      }
      UInt32 len = end - start;

      // Return new array.
      T[] res = new T[len];
      /*
      for ( int i = 0; i < len; i++ ) {
        resOLD[i] = source[i + start];
      }
      */
      Array.ConstrainedCopy( source, ( int )start, res, 0, ( int )len );
      return res;
    }

    /// <summary>
    /// Get the array slice between the two indexes.
    /// ... Inclusive for start index, length.
    /// </summary>
    public static T[] SliceL<T>( this T[] source, UInt32 offset, UInt32 length )
    {
      UInt32 end = offset + length;
      // Handles negative ends.
      if ( end < 0 ) {
        end = ( UInt32 )source.Length + end;
      }
      UInt32 len = end - offset;

      // Return new array.
      T[] res = new T[len];
      /*
      for ( int i = 0; i < len; i++ ) {
        res[i] = source[i + offset];
      }
      */
      Array.ConstrainedCopy( source, ( int )offset, res, 0, ( int )len );
      return res;
    }
  }



  internal class Conversions
  {

    /// <summary>
    /// Converts a number of ASCII chars into a string
    /// </summary>
    /// <param name="bPtr">Mem Location of the ASCII Chars</param>
    /// <param name="size">Max number of ASCII chars to convert, stops at \0 however</param>
    /// <returns>The converted string</returns>
    static public string ToString( byte[] byteArr, uint size = 999 )
    {
      string s = "";
      for ( uint i = 0; i < size; i++ ) {
        if ( byteArr[i] != 0 )
          s += Char.ConvertFromUtf32( byteArr[i] );
        else
          break; // stop at char 0
      }
      return s;
    }

    /// <summary>
    /// Allocates and reads bytes of the size of one record 
    /// and returns the allocated bytes are structure - allowing structured access to binary data 
    /// Note: there is no error checking whatsoever here - so better make sure everything is OK
    /// </summary>
    /// <typeparam name="T">The record type to read</typeparam>
    /// <param name="reader">A binary reader</param>
    /// <returns>The read record</returns>
    public static T ByteToType<T>( byte[] bytes, UInt32 offset = 0 )
    {
      byte[] _bytes = bytes.SliceL( offset, ( UInt32 )Marshal.SizeOf( typeof( T ) ) ); // lets see if this works with Alloc below..

      GCHandle handle = GCHandle.Alloc( _bytes, GCHandleType.Pinned );
      T theStructure = ( T )Marshal.PtrToStructure( handle.AddrOfPinnedObject( ), typeof( T ) );
      handle.Free( );

      return theStructure;
    }


  }
}
