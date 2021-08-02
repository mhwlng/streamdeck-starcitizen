using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCJMapper_V2.SC
{
  /// <summary>
  /// One SC asset file
  /// </summary>
  ///
  [Serializable()]
  class SCFile 
  {
    public enum FileType
    {
      UnknownFile = -1,
      PakFile = 0,
      DefProfile = 1,
      LangFile=3,
    }

    public FileType Filetype { get; set; }
    public string Filename { get; set; }
    public string Filepath { get; set; }
    public DateTime FileDateTime { get; set; }
    public string Filedata { get; set; }

    public SCFile()
    {
      Filetype = FileType.UnknownFile;
      Filename = "";
      Filepath = "";
      FileDateTime = new DateTime( 1970, 1, 1, 0, 0, 0 );
      Filedata = "";
    }


  }
}
