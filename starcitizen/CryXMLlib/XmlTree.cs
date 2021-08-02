using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCJMapper_V2.CryXMLlib
{
  /// <summary>
  /// Processes a CryXmlNodeRef and reports the node and its childs as XML string list
  /// </summary>
  public class XmlTree
  {

    private List<string> m_doc = new List<string>( ); // internal list of strings

    /// <summary>
    /// Return the derived XML text as List of strings
    /// </summary>
    public List<string> XML_list { get { return m_doc; } }

    /// <summary>
    /// Return the derived XML text as string
    /// </summary>
    public string XML_string
    {
      get {
        string xml = ""; string CR = string.Format("\n");
        foreach ( string s in m_doc ) xml += ( s + CR );
        return xml;
      }
    }


    /// <summary>
    /// Processes all attributed of a node
    /// </summary>
    /// <param name="nodeRef">The node to process</param>
    /// <returns>an string containing all attributes in XML compatible format ( key="value" )</returns>
    private string DoAttribute( CryXmlNodeRef nodeRef )
    {
      IXmlNode node = nodeRef;
      string xml = "";

      // collect all attributes into one line
      for ( int ac=0; ac < node.getNumAttributes( ); ac++ ) {
        string key = ""; string value = "";
        node.getAttributeByIndex(ac, out key, out value );
        xml += string.Format( " {0}=\"{1}\" ", key, value);
      }
      return xml;
    }

    /// <summary>
    /// Processes a node and it's children
    /// </summary>
    /// <param name="nodeRef">The node to process</param>
    /// <param name="level">The depth of the node to indent the XML text</param>
    private void DoNode( CryXmlNodeRef nodeRef, int level )
    {
      string tabs = "".PadRight(level, '\t'); // level is depth - will be left padded with Tabs on Add()
      string xml = "";

      IXmlNode node = nodeRef;

      // first gather and handle the attributes
      string attr = DoAttribute( nodeRef );
      xml = string.Format( "<{0} {1} ", node.getTag( ), attr );

      // then do some formatting dependent of child nodes to be printed
      if ( node.getChildCount( ) < 1 ) {
        // no child - close with end tag immediately
        xml += string.Format( "/>" ); m_doc.Add( tabs + xml ); // add line w/o NL
      }
      else {
        // with children - close tag only
        xml += string.Format( ">" ); m_doc.Add( tabs + xml ); // add line w/o NL

        // do the children
        for ( int cc=0; cc < node.getChildCount( ); cc++ ) {
          CryXmlNodeRef childRef = node.getChild( cc );
          DoNode( childRef, level+1 ); // recursion
        }
        xml = string.Format( "</{0}>\n", node.getTag( ) ); m_doc.Add( tabs + xml ); // add line with NL to space them
      }
    }


    /// <summary>
    /// Processes a CryXmlNodeRef to derive the XML formatted structure
    ///  Note: the created XML text can be retrieved through the XML property of this object
    /// </summary>
    /// <param name="rootRef">The node to start from</param>
    public void BuildXML( CryXmlNodeRef rootRef )
    {
      m_doc.Clear( );

      DoNode( rootRef, 0 );
    }

  }
}
