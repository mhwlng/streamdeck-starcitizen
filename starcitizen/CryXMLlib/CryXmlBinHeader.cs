using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SCJMapper_V2.CryXMLlib
{
  /// <summary>
  /// Derived (file structure) work from CryEngine: XMLBinaryHeaders.h:
  /// ---------------------------------------------------------------------------
  /// Copyright 2006 Crytek GmbH
  /// Created by: Michael Smith
  ///---------------------------------------------------------------------------
  /// </summary>


  /// <summary>
  /// kind of typedef (curtesy http://www.codeproject.com/Questions/141385/typedef-in-C)
  ///  A wrapper around <see cref="System.UInt32"/>.
  /// </summary>
  internal struct CryXMLNodeIndex
  {
    static public UInt32 MySize( ) { return ( UInt32 )Marshal.SizeOf( typeof( CryXMLNodeIndex ) ); }

    private UInt32 value;

    // As we are using implicit conversions we can keep the constructor private
    private CryXMLNodeIndex( UInt32 value )
    {
      this.value = value;
    }
    private CryXMLNodeIndex( int value )
    {
      this.value = ( CryXMLNodeIndex )value;
    }

    /// <summary>
    /// Implicitly converts a <see cref="System.UInt32"/> to a Record.
    /// </summary>
    /// <param name="value">The <see cref="System.UInt32"/> to convert.</param>
    /// <returns>A new Record with the specified value.</returns>
    public static implicit operator CryXMLNodeIndex( UInt32 value )
    {
      return new CryXMLNodeIndex( value );
    }
    public static implicit operator CryXMLNodeIndex( int value )
    {
      return new CryXMLNodeIndex( value );
    }
    /// <summary>
    /// Implicitly converts a Record to a <see cref="System.UInt32"/>.
    /// </summary>
    /// <param name="record">The Record to convert.</param>
    /// <returns>
    /// A <see cref="System.UInt32"/> that is the specified Record's value.
    /// </returns>
    public static implicit operator UInt32( CryXMLNodeIndex record )
    {
      return record.value;
    }
    public static implicit operator int( CryXMLNodeIndex record )
    {
      return ( int )record.value;
    }
  }


  /// <summary>
  /// Maps the Node from binary file to memory
  /// </summary>
  [StructLayout( LayoutKind.Sequential, Pack = 1 )] //, Size = 5 * 4 + 2 * 2 = 24 (28 according to dump)
  internal struct CryXMLNode
  {
    static public UInt32 MySize( ) { return ( UInt32 )Marshal.SizeOf( typeof( CryXMLNode ) ); }

    public CryXMLNodeIndex nParentIndex { get { return _nParentIndex; } } // type conversion only
    public CryXMLNodeIndex nFirstAttributeIndex { get { return _nFirstAttributeIndex; } } // type conversion only
    public CryXMLNodeIndex nFirstChildIndex { get { return _nFirstChildIndex; } } // type conversion only

    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nTagStringOffset;         // offset in CBinaryXmlData::pStringData
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nContentStringOffset;     // offset in CBinaryXmlData::pStringData
    [MarshalAs( UnmanagedType.U2 )]
    public UInt16 nAttributeCount;
    [MarshalAs( UnmanagedType.U2 )]
    public UInt16 nChildCount;
    [MarshalAs( UnmanagedType.U4 )]
    private UInt32 _nParentIndex;
    [MarshalAs( UnmanagedType.U4 )]
    private UInt32 _nFirstAttributeIndex;
    [MarshalAs( UnmanagedType.U4 )]
    private UInt32 _nFirstChildIndex;

    [MarshalAs( UnmanagedType.U4 )]
    private UInt32 _PAD; // according to hex analysis we have a 4 byte pad at the end of each node
  }


  /// <summary>
  /// Maps the Attribute from binary file to memory
  /// </summary>
  [StructLayout( LayoutKind.Sequential, Pack = 1 )] //, Size = 2 * 4 = 8 (OK)
  internal struct CryXMLAttribute
  {
    static public UInt32 MySize( ) { return ( UInt32 )Marshal.SizeOf( typeof( CryXMLAttribute ) ); }

    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nKeyStringOffset;         // offset in CBinaryXmlData::pStringData
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nValueStringOffset;       // offset in CBinaryXmlData::pStringData
  }


  /// <summary>
  /// Maps the Header from binary file to memory
  /// </summary>
  [StructLayout( LayoutKind.Sequential, Pack = 1 )] //, Size = 9 * 4 + 8 = 44
  internal struct CryXMLHeader
  {
    static public UInt32 MySize( ) { return ( UInt32 )Marshal.SizeOf( typeof( CryXMLHeader ) ); }

    public bool HasCorrectSignature { get { return ( szSignature == "CryXmlB" ); } }

    public string szSignature { get { return Conversions.ToString( _szSignature, 8 ); } } // from ASCIIZ

    [MarshalAs( UnmanagedType.ByValArray, SizeConst = 8 )]
    private byte[] _szSignature;
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nXMLSize;
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nNodeTablePosition;
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nNodeCount;
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nAttributeTablePosition;
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nAttributeCount;
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nChildTablePosition;
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nChildCount;
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nStringDataPosition;
    [MarshalAs( UnmanagedType.U4 )]
    public UInt32 nStringDataSize;

  }




}
