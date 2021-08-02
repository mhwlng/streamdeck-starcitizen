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
  /// Implemented IXmlNode
  /// </summary>
  internal class CryXmlBinNode : IXmlNode // CBinaryXmlNode
  {

    internal CryXmlBinContext m_pData = null; // ref to the binary data area
    internal UInt32 m_pNodeIndex = 0;      // Index of ourself

    // Return current node in binary data.
    private CryXMLNode _node( )
    {
      return m_pData.pNodes[m_pNodeIndex];
    }

    private XmlString _string( UInt32 nIndex )
    {
      return m_pData._string( nIndex );
    }

    private string GetValue( string key )
    {
      CryXMLNodeIndex nFirst = _node( ).nFirstAttributeIndex;
      CryXMLNodeIndex nLast = nFirst + _node( ).nAttributeCount;
      for ( CryXMLNodeIndex i = nFirst; i < nLast; i++ ) {
        XmlString attrKey = _string( m_pData.pAttributes[i].nKeyStringOffset );
        if ( key == attrKey ) {
          string attrValue =  _string( m_pData.pAttributes[i].nValueStringOffset );
          return attrValue;
        }
      }
      return "";
    }


    // Get XML node tag.
    public override string getTag( )
    {
      return _string( _node( ).nTagStringOffset );
    }

    // Return true if given tag is equal to node tag.
    public override bool isTag( string tag )
    {
      return ( tag == getTag( ) );
    }


    // Check if attributes with specified key exist.
    public override bool haveAttr( string key )
    {
      return ( !string.IsNullOrEmpty( GetValue( key ) ) );
    }

    // Get XML Node attributes.
    public override int getNumAttributes( ) { return ( int )_node( ).nAttributeCount; }

    // Return attribute key and value by attribute index.
    public override bool getAttributeByIndex( int index, out string key, out string value )
    {
      key = ""; value = "";

      CryXMLNode pNode = _node( );
      if ( index >= 0 && index < pNode.nAttributeCount ) {
        CryXMLAttribute attr = m_pData.pAttributes[pNode.nFirstAttributeIndex + index];
        key = _string( attr.nKeyStringOffset );
        value = _string( attr.nValueStringOffset );
        return true;
      }
      return false;
    }

    // Return attribute key and value by attribute index, string version.
    public virtual bool getAttributeByIndex( int index, out XmlString key, out XmlString value )
    {
      string _key, _value;
      bool retVal = getAttributeByIndex( index, out _key, out _value );
      key = ( XmlString )_key; value = ( XmlString )_value;
      return retVal;
    }

    // Get XML Node attribute for specified key.
    public override string getAttr( string key )
    {
      return GetValue( key );
    }

    // Get XML Node attribute for specified key.
    // Returns true if the attribute exists, false otherwise.
    public override bool getAttr( string key, out string value )
    {
      string svalue = GetValue( key );
      if ( !string.IsNullOrEmpty( svalue ) ) {
        value = svalue;
        return true;
      }
      else {
        value = "";
        return false;
      }
    }

    // Get attribute value of node.
    public override bool getAttr( string key, out XmlString value )
    {
      string v = "";
      bool  boHasAttribute = getAttr( key, out v );
      value = v;
      return boHasAttribute;
    }

    // Get number of child XML nodes.
    public override int getChildCount( ) { return ( int )_node( ).nChildCount; }

    // Get XML Node child nodes.
    public override CryXmlNodeRef getChild( int i )
    {
      CryXMLNode pNode = _node( );
      if ( i < 0 || i > ( int )pNode.nChildCount ) {
        return null;
      }
      CryXmlNodeRef n = m_pData.pBinaryNodes[m_pData.pChildIndices[pNode.nFirstChildIndex + i]];
      return n;
    }

    // Find node with specified tag.
    public override CryXmlNodeRef findChild( string tag )
    {
      CryXMLNode pNode = _node( );
      CryXMLNodeIndex nFirst = pNode.nFirstChildIndex;
      CryXMLNodeIndex nAfterLast = pNode.nFirstChildIndex + pNode.nChildCount;
      for ( CryXMLNodeIndex i = nFirst; i < nAfterLast; i++ ) {
        string sChildTag = _string( m_pData.pNodes[m_pData.pChildIndices[i]].nTagStringOffset );
        if ( tag == sChildTag ) {
          CryXmlNodeRef n = m_pData.pBinaryNodes[m_pData.pChildIndices[i]];
          return n;
        }
      }
      return null;
    }


    // Get parent XML node.
    public override CryXmlNodeRef getParent( )
    {
      CryXMLNode pNode = _node( );
      if ( pNode.nParentIndex != ( CryXMLNodeIndex )( -1 ) ) { // murks..
        CryXmlNodeRef n = m_pData.pBinaryNodes[pNode.nParentIndex];
        return n;
      }
      // has no parent i.e. toplevel
      return this;
    }

    // Returns content of this node.
    public override string getContent( ) { return _string( _node( ).nContentStringOffset ); }


  }

}
