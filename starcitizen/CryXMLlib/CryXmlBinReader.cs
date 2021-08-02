using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace SCJMapper_V2.CryXMLlib
{
  /// <summary>
  /// Inspired... by work from CryEngine: XMLBinaryReader.h/.cpp:
  /// ---------------------------------------------------------------------------
  /// Copyright 2006 Crytek GmbH
  /// Created by: Michael Smith
  /// ---------------------------------------------------------------------------
  /// 
  /// BUT uses a complete different memory management and processing eliminating extensive Ptr juggling from C++
  /// </summary>
  public class CryXmlBinReader
  {
    /// <summary>
    /// The processing result Enum
    /// </summary>
    public enum EResult
    {
      Success,
      NotBinXml,
      Error,
    }

    private string m_errorDescription = "";
    private void SetErrorDescription( string text )
    {
      m_errorDescription = text;
    }

    /// <summary>
    /// Get error information if something goes wrong
    /// </summary>
    /// <returns>The error description</returns>
    public string GetErrorDescription( ) { return m_errorDescription; }

    /// <summary>
    /// Load a Cry Binary XML file into memory and return the NodeRef for the root element
    /// </summary>
    /// <param name="filename">The file to read</param>
    /// <param name="result">Read result</param>
    /// <returns>The NodeRef of the root element or NULL</returns>
    public CryXmlNodeRef LoadFromFile( string filename, out EResult result )
    {
      m_errorDescription = "";
      result = EResult.Error;

      if ( !File.Exists( filename ) ) {
        SetErrorDescription( "Can't open file (file not found)." );
        return null;
      }

      using ( BinaryReader binReader = new BinaryReader( File.Open( filename, FileMode.Open ) ) ) {
        try {
          UInt32 fileSize = ( UInt32 )binReader.BaseStream.Length;

          if ( fileSize < CryXMLHeader.MySize( ) ) {
            result = EResult.NotBinXml;
            SetErrorDescription( "File is not a binary XML file (file size is too small)." );
            return null;
          }

          // read from binary file and map the content into the memory
          CryXmlBinContext pData = Create( binReader, fileSize, out result );

          if ( result != EResult.Success ) return null; // Well...

          // Return first node
          CryXmlNodeRef n = pData.pBinaryNodes[0];
          return n;

        } catch {
          result = EResult.NotBinXml;
          SetErrorDescription( "Exception in BinaryReader." );
          return null;
        }
      }
    }


    /// <summary>
    /// Load a Cry Binary XML buffer into memory and return the NodeRef for the root element
    /// </summary>
    /// <param name="filename">The byte buffer to read</param>
    /// <param name="result">Read result</param>
    /// <returns>The NodeRef of the root element or NULL</returns>
    public CryXmlNodeRef LoadFromBuffer( byte[] binBuffer, out EResult result )
    {
      m_errorDescription = "";
      result = EResult.Error;

      UInt32 fileSize = ( UInt32 )binBuffer.Length;

      try {
        if ( fileSize < CryXMLHeader.MySize( ) ) {
          result = EResult.NotBinXml;
          SetErrorDescription( "File is not a binary XML file (file size is too small)." );
          return null;
        }

        // read from binary file and map the content into the memory
        CryXmlBinContext pData = Create( binBuffer, out result );

        if ( result != EResult.Success ) return null; // Well...

        // Return first node
        CryXmlNodeRef n = pData.pBinaryNodes[0];
        return n;

      } catch {
        result = EResult.NotBinXml;
        SetErrorDescription( "Exception in buffer reader" );
        return null;
      }
    }


    /// <summary>
    /// Reads the binary file into the internal structures
    ///  Note: use GetErrorDescription() if the result is not Success
    /// </summary>
    /// <param name="bReader">An open binary reader</param>
    /// <param name="size">The length to read</param>
    /// <param name="result">Out: the result of the collection of file content</param>
    /// <returns>A valid memory context or null</returns>
    private CryXmlBinContext Create( BinaryReader bReader, UInt32 size, out EResult result )
    {
      result = EResult.Error;

      // read data from file into a local buffer
      byte[] fileContents = bReader.ReadBytes( ( int )size );   // get all from the reader

      return Create( fileContents, out result );
    }


    /// <summary>
    /// Reads the binary buffer into the internal structures
    ///  Note: use GetErrorDescription() if the result is not Success
    /// </summary>
    /// <param name="fileContents">An filled byte buffer</param>
    /// <param name="result">Out: the result of the collection of file content</param>
    /// <returns>A valid memory context or null</returns>
    private CryXmlBinContext Create( byte[] fileContents, out EResult result )
    {
      result = EResult.Error;

      CryXmlBinContext pData = new CryXmlBinContext( );
      if ( pData == null ) {
        SetErrorDescription( "Can't allocate memory for binary XML object." );
        return null;
      }

      CryXMLHeader header = Conversions.ByteToType<CryXMLHeader>( fileContents ); // reads the header, mapping from binary to struct
      if ( !header.HasCorrectSignature ) {
        SetErrorDescription( "File is not a binary XML object (wrong header signature)." );
        return null;
      }

      // Create binary nodes to wrap the ones to read later 
      pData.pBinaryNodes = new List<CryXmlBinNode>( ); // dynamic list used, not an array
      for ( int i = 0; i < header.nNodeCount; i++ ) { pData.pBinaryNodes.Add( new CryXmlBinNode( ) ); }

      // map file content to binary.. here we really allocate arrays of elements and copy rather than the original which worked well with ptrs in c++

      try {
        pData.pAttributes = ( CryXMLAttribute[] )Array.CreateInstance( typeof( CryXMLAttribute ), header.nAttributeCount ); // alloc enough
        UInt32 incr = CryXMLAttribute.MySize( ); // size of one element to read
        for ( UInt32 aIdx = 0; aIdx < header.nAttributeCount; aIdx++ ) {
          pData.pAttributes[aIdx] = Conversions.ByteToType<CryXMLAttribute>( fileContents, header.nAttributeTablePosition + aIdx * incr );
        }
      } catch ( Exception e ) {
        SetErrorDescription( string.Format( "EXC Attributes: {0}", e.Message ) );
        return null;
      }

      try {
        pData.pChildIndices = ( CryXMLNodeIndex[] )Array.CreateInstance( typeof( CryXMLNodeIndex ), header.nChildCount ); // alloc enough
        UInt32 incr = CryXMLNodeIndex.MySize( ); // size of one element to read
        for ( UInt32 aIdx = 0; aIdx < header.nChildCount; aIdx++ ) {
          pData.pChildIndices[aIdx] = Conversions.ByteToType<CryXMLNodeIndex>( fileContents, header.nChildTablePosition + aIdx * incr );
        }
      } catch ( Exception e ) {
        SetErrorDescription( string.Format( "EXC ChildIndices: {0}", e.Message ) );
        return null;
      }

      try {
        pData.pNodes = ( CryXMLNode[] )Array.CreateInstance( typeof( CryXMLNode ), header.nNodeCount ); // alloc enough
        UInt32 incr = CryXMLNode.MySize( ); // size of one element to read
        for ( UInt32 aIdx = 0; aIdx < header.nNodeCount; aIdx++ ) {
          pData.pNodes[aIdx] = Conversions.ByteToType<CryXMLNode>( fileContents, header.nNodeTablePosition + aIdx * incr );
        }
      } catch ( Exception e ) {
        SetErrorDescription( string.Format( "EXC Nodes: {0}", e.Message ) );
        return null;
      }

      try {
        pData.pStringDataLength = header.nStringDataSize;
        pData.pStringData = fileContents.SliceL( header.nStringDataPosition, header.nStringDataSize );
      } catch ( Exception e ) {
        SetErrorDescription( string.Format( "EXC StringData: {0}", e.Message ) );
        return null;
      }

      // init binary nodes
      for ( UInt32 nNode = 0; nNode < header.nNodeCount; ++nNode ) {
        pData.pBinaryNodes[( int )nNode].m_pData = pData; // add data space
        pData.pBinaryNodes[( int )nNode].m_pNodeIndex = nNode; // self ref..
      }

      // and back..
      result = EResult.Success;
      return pData;
    }

  }


}
