using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCJMapper_V2.CryXMLlib
{
  /// <summary>
  /// Derived work from CryEngine: IXml.h:
  /// 
  ///  Crytek Engine Source File.
  ///  Copyright (C), Crytek Studios, 2002.
  /// -------------------------------------------------------------------------
  ///  File name:   ixml.h
  ///  Version:     v1.00
  ///  Created:     16/7/2002 by Timur.
  ///  Compilers:   Visual Studio.NET
  ///  Description: 
  // -------------------------------------------------------------------------
  /// </summary>


  /// <summary>
  /// kind of typedef (curtesy http://www.codeproject.com/Questions/141385/typedef-in-C)
  // Summary:
  //	 Special string wrapper for xml nodes.
  /// </summary>
  public struct XmlString
  {
    // A struct's fields should not be exposed
    private string value;

    // As we are using implicit conversions we can keep the constructor private
    private XmlString( string value )
    {
      this.value = value;
    }

    /// <summary>
    /// Implicitly converts a XmlString to a <see cref="System.string"/>.
    /// </summary>
    /// <param name="value">The <see cref="System.string"/> to convert.</param>
    /// <returns>A new XmlString with the specified value.</returns>
    public static implicit operator XmlString( string value )
    {
      return new XmlString( value );
    }
    /// <summary>
    /// Implicitly converts a XmlString to a <see cref="System.string"/>.
    /// </summary>
    /// <param name="record">The XmlString to convert.</param>
    /// <returns>
    /// A <see cref="System.string"/> that is the specified XmlString's value.
    /// </returns>
    public static implicit operator string( XmlString record )
    {
      return record.value;
    }

    public static XmlString c_str( byte[] sz, uint len = 999 )
    {
      return Conversions.ToString( sz, len );
    }
  }


  /// <summary>
  ///	 Abstract IXmlNode class
  /// Notes:
  ///	 Never use IXmlNode directly instead use reference counted XmlNodeRef.
  /// See also:
  ///	 XmlNodeRef
  /// </summary>
  public abstract class IXmlNode
  {

    /// <summary>
    ///	 Gets XML node tag.
    /// </summary>
    /// <returns></returns>
    public abstract string getTag( );

    /// <summary>
    ///  Returns true if a given tag equal to node tag.
    /// </summary>
    /// <param name="tag">The tag</param>
    /// <returns>True if it is the tag of the node</returns>
    public abstract bool isTag( string tag );

    /// <summary>
    /// Checks if attributes with specified key exist.
    /// </summary>
    /// <param name="key">The attribute key</param>
    /// <returns>True if this attribute exists</returns>
    public abstract bool haveAttr( string key );

    //	
    /// <summary>
    ///  Gets XML Node attributes count
    /// </summary>
    /// <returns>The number of attributes of this node</returns>
    public abstract int getNumAttributes( );

    /// <summary>
    ///  Returns attribute key and value by attribute index.
    /// </summary>
    /// <param name="index">The attribute index 0..</param>
    /// <param name="key">Out: the attribute key</param>
    /// <param name="value">Out: the attribute value</param>
    /// <returns>True if returned key and value are valid</returns>
    public abstract bool getAttributeByIndex( int index, out string key, out string value );

    /// <summary>
    ///  Gets XML Node attribute for specified key.
    /// </summary>
    /// <param name="key">The attribute key</param>
    /// <returns>The attribute value or an empty string if it does not exist</returns>
    public abstract string getAttr( string key );

    /// <summary>
    /// Gets XML Node attribute for specified key.
    /// </summary>
    /// <param name="key">The attribute key</param>
    /// <param name="value">Out: the attribute value</param>
    /// <returns>True if the attribute with key exists and the returned value is valid</returns>
    public abstract bool getAttr( string key, out string value );

    /// <summary>
    ///  Gets attribute value of node.
    /// </summary>
    /// <param name="key">The attribute key</param>
    /// <param name="value">Out: the attribute value as XmlString</param>
    /// <returns>True if the attribute with key exists and the returned value is valid</returns>
    public abstract bool getAttr( string key, out XmlString value );

    /// <summary>
    ///  Gets number of child XML nodes.
    /// </summary>
    /// <returns>The number of child nodes of this node</returns>
    public abstract int getChildCount( );

    /// <summary>
    ///  Gets XML Node child nodes.
    /// </summary>
    /// <param name="i">The child index of the node to return</param>
    /// <returns>A valid nodeRef or null if the element does not exist</returns>
    public abstract CryXmlNodeRef getChild( int i );

    /// <summary>
    ///  Finds node with specified tag.
    /// </summary>
    /// <param name="tag">The node tag of the child node to return</param>
    /// <returns>A valid nodeRef or null if the element does not exist</returns>
    public abstract CryXmlNodeRef findChild( string tag );

    /// <summary>
    ///  Gets parent XML node.
    /// </summary>
    /// <returns>A valid nodeRef or null if the element does not exist</returns>
    public abstract CryXmlNodeRef getParent( );

    /// <summary>
    ///  Returns content of this node.
    /// </summary>
    /// <returns>The node content as string</returns>
    public abstract string getContent( );

  }


}
