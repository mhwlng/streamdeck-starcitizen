using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCJMapper_V2.p4kFile
{
  class p4kFileTStamp
  {

    /// <summary>
    /// Converts from Zip (DOS) File date time to .Net DatetTime format 
    /// </summary>
    /// <param name="date">A DOS file date integer</param>
    /// <param name="time">A DOS file time integer</param>
    /// <returns>The DateTime conversion of the input</returns>
    public static DateTime FromDos( UInt16 date, UInt16 time )
    {
      // DOS date : hBit-- YYYYYYY MMMM TTTTT  (year + 1980)
      // DOS time : hBit-- hhhhh mmmmmm xxxxx  (x*2 = sec)

      int year = ( ( date >> 9 ) & 0x7f ) + 1980;
      int month = ( date >> 5 ) & 0x0f;
      int day = date & 0x01f;
      int hour = ( time >> 11 ) & 0x1f;
      int min = ( time >> 5 ) & 0x3f;
      int sec = ( time & 0x01f ) * 2;
      try {
        var ret = new DateTime( year, month, day, hour, min, sec );
        return ret;
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch ( Exception e ) {
#pragma warning restore CS0168 // Variable is declared but never used
        return new DateTime( 1970, 1, 1 );
      }
    }

  }
}
