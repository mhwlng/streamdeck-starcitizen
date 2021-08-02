using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCJMapper_V2.CryXMLlib
{
  /// <summary>
  /// Derived work from CryEngine: XMLBinaryHeaders.h:
  ///  CryEngine Source File.
  ///  Copyright (C), Crytek Studios, 2001-2006.
  /// -------------------------------------------------------------------------
  ///  File name:   xml.h
  ///  Created:     21/04/2006 by Timur.
  ///  Description: 
  /// -------------------------------------------------------------------------
  /// </summary>
  /// 


  /// <summary>
  /// Memory context of the binary XML file
  /// </summary>
  internal class CryXmlBinContext // CBinaryXmlData
  {

    public CryXMLNode[]       pNodes = null;         // gets a copy of all Node entries
    public CryXMLAttribute[]  pAttributes= null;     // gets a copy of all Attribute entries
    public CryXMLNodeIndex[]  pChildIndices= null;   // gets a copy of all Note relations
    public byte[]             pStringData = null;    // gets a copy of the string data
    public UInt32             pStringDataLength = 0; // length of the string data

    /// <summary>
    /// Returns an XMLString from the stringData area
    /// </summary>
    /// <param name="sOffset">The start offset of the string to return</param>
    /// <returns>The string</returns>
    public XmlString _string( UInt32 sOffset )
    {
      return Conversions.ToString( pStringData.SliceE( sOffset, pStringDataLength ) );
    }

    public List<CryXmlBinNode> pBinaryNodes= null;    // list of binary nodes - one to one with the real nodes
  }


}
