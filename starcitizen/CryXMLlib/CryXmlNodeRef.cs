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

  // Summary:
  //	 XmlNodeRef, wrapper class IXmlNode.
  // See also:
  //	 IXmlNode
  public class CryXmlNodeRef
  {
    // CLASS
    private IXmlNode p = null;

    public CryXmlNodeRef( ) { }
    public CryXmlNodeRef( IXmlNode p_ )
    {
      p = p_;
    }
    public CryXmlNodeRef( CryXmlNodeRef p_ )
    {
      p = p_.p;
    }

    // conversions 

    /// <summary>
    /// Implicitly converts a XmlString to a IXmlNode.
    /// </summary>
    /// <param name="value">The IXmlNode to convert.</param>
    /// <returns>A new XmlString with the specified value.</returns>
    public static implicit operator CryXmlNodeRef( IXmlNode value )
    {
      return new CryXmlNodeRef( value );
    }

    /// <summary>
    /// Implicitly converts a IXmlNode to a XmlNodeRef.
    /// </summary>
    /// <param name="value">The XmlNodeRef to convert.</param>
    /// <returns>A new IXmlNode with the specified value.</returns>
    public static implicit operator IXmlNode( CryXmlNodeRef value )
    {
      return value.p;
    }

    // Misc compare functions.
    public static bool operator ==( CryXmlNodeRef p1, CryXmlNodeRef p2 )
    {
      // If both are null, or both are same instance, return true.
      if ( System.Object.ReferenceEquals( p1, p2 ) ) {
        return true;
      }

      // If one is null, but not both, return false.
      if ( ( ( object )p1 == null ) || ( ( object )p2 == null ) ) {
        return false;
      }

      // Return true if the fields match:
      return p1 == p2;

    }
    public static bool operator !=( CryXmlNodeRef p1, CryXmlNodeRef p2 ) { return p1 != p2; }

    public override bool Equals( System.Object obj )
    {
      // If parameter is null return false.
      if ( obj == null ) {
        return false;
      }

      // If parameter cannot be cast to Point return false.
      CryXmlNodeRef objP = obj as CryXmlNodeRef;
      if ( ( System.Object )p == null ) {
        return false;
      }

      // Return true if the fields match:
      return ( p == objP.p );
    }

    public bool Equals( CryXmlNodeRef objP )
    {
      // If parameter is null return false:
      if ( ( object )objP == null ) {
        return false;
      }

      // Return true if the fields match:
      return ( p == objP.p );
    }

    public override int GetHashCode( )
    {
      return base.GetHashCode( ) ^ p.GetHashCode( );
    }
  }

}
