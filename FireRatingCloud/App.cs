#region Namespaces
using System.Diagnostics;
using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
#endregion

namespace FireRatingCloud
{
  class App : IExternalApplication
  {
    /// <summary>
    /// Caption
    /// </summary>
    public const string Caption = "FireRatingCloud";

    /// <summary>
    /// Switch between subscribe 
    /// and unsubscribe commands.
    /// </summary>
    const string _subscribe = "Subscribe";
    const string _unsubscribe = "Unsubscribe";

    /// <summary>
    /// Subscription debugging benchmark timer.
    /// </summary>
    //static JtTimer _timer = null;

    /// <summary>
    /// Store the external event.
    /// </summary>
    static ExternalEvent _event = null;

    /// <summary>
    /// Executing assembly namespace
    /// </summary>
    static string _namespace = typeof( App ).Namespace;

    /// <summary>
    /// Command name prefix
    /// </summary>
    const string _cmd_prefix = "Cmd_";

    /// <summary>
    /// Currently executing assembly path
    /// </summary>
    static string _path = typeof( App )
      .Assembly.Location;

    /// <summary>
    /// Keep track of our ribbon buttons to toggle
    /// them on and off later and change their text.
    /// </summary>
    static RibbonItem[] _buttons;

    static int _subscribeButtonIndex = 4;

    #region Icon resource, bitmap image and ribbon panel stuff
    /// <summary>
    /// Return path to embedded resource icon
    /// </summary>
    static string IconResourcePath(
      string name,
      string size )
    {
      return _namespace
        + "." + "Icon" // folder name
        + "." + name + size // icon name
        + ".png"; // filename extension
    }

    /// <summary>
    /// Load a new icon bitmap from embedded resources.
    /// For the BitmapImage, make sure you reference 
    /// WindowsBase and PresentationCore, and import 
    /// the System.Windows.Media.Imaging namespace. 
    /// </summary>
    static BitmapImage GetBitmapImage(
      Assembly a,
      string path )
    {
      // to read from an external file:
      //return new BitmapImage( new Uri(
      //  Path.Combine( _imageFolder, imageName ) ) );

      string[] names = a.GetManifestResourceNames();

      Stream s = a.GetManifestResourceStream( path );

      Debug.Assert( null != s,
        "expected valid icon resource" );

      BitmapImage img = new BitmapImage();

      img.BeginInit();
      img.StreamSource = s;
      img.EndInit();

      return img;
    }

    /// <summary>
    /// Create a custom ribbon panel and populate
    /// it with our commands, saving the resulting
    /// ribbon items for later access.
    /// </summary>
    static void AddRibbonPanel(
      UIControlledApplication a )
    {
      string[] tooltip = new string[] {
        "Create and bind shared parameter definition.",
        "Export shared parameter values one by one creating new and updating existing documents.",
        "Export shared parameter values in batch after deleting all existing project documents.",
        "Import shared parameter values.",
        "Subscribe to or unsubscribe from updates.",
        "About " + Caption + ": ..."
      };

      string[] text = new string[] {
        "Bind Shared Parameter",
        "Export one by one",
        "Export batch",
        "Import",
        "Subscribe",
        "About..."
      };

      string[] classNameStem = new string[] {
        "1_CreateAndBindSharedParameter",
        "2a_ExportSharedParameterValues",
        "2b_ExportSharedParameterValuesBatch",
        "3_ImportSharedParameterValues",
        "4_Subscribe",
        "0_About"
      };

      string[] iconName = new string[] {
        "Knot",
        "1Up",
        "2Up",
        "1Down",
        "ZigZagRed",
        "Question"
      };

      int n = classNameStem.Length;

      Debug.Assert( text.Length == n
        && tooltip.Length == n
        && iconName.Length == n,
        "expected equal number of text and class name entries" );

      Debug.Assert(
        text[_subscribeButtonIndex].Equals( _subscribe ),
        "Did you set the correct _subscribeButtonIndex?" );

      _buttons = new RibbonItem[n];

      RibbonPanel panel
        = a.CreateRibbonPanel( Caption );

      SplitButtonData splitBtnData
        = new SplitButtonData( Caption, Caption );

      SplitButton splitBtn = panel.AddItem(
        splitBtnData ) as SplitButton;

      Assembly asm = typeof( App ).Assembly;

      for( int i = 0; i < n; ++i )
      {
        PushButtonData d = new PushButtonData(
          classNameStem[i], text[i], _path,
          _namespace + "." + _cmd_prefix
          + classNameStem[i] );

        d.ToolTip = tooltip[i];

        d.Image = GetBitmapImage( asm,
          IconResourcePath( iconName[i], "16" ) );

        d.LargeImage = GetBitmapImage( asm,
          IconResourcePath( iconName[i], "32" ) );

        d.ToolTipImage = GetBitmapImage( asm,
          IconResourcePath( iconName[i], "" ) );

        _buttons[i] = splitBtn.AddPushButton( d );
      }
    }
    #endregion // Icon resource, bitmap image and ribbon panel stuff

    #region Idling subscription and external event creation
    /// <summary>
    /// Are we currently subscribed 
    /// to automatic cloud updates?
    /// </summary>
    public static bool Subscribed
    {
      get
      {
        bool rc = _buttons[_subscribeButtonIndex]
          .ItemText.Equals( _unsubscribe );

        Debug.Assert( ( _event != null ) == rc, 
          "expected synchronised handler and button text" );

        return rc;
      }
    }

    /// <summary>
    /// Toggle on and off subscription to automatic 
    /// cloud updates. Return true when subscribed.
    /// </summary>
    public static bool ToggleSubscription2(
      IExternalEventHandler handler ) 
    {
      if( Subscribed )
      {
        Debug.Print( "Unsubscribing..." );

        _event.Dispose();
        _event = null;

        _buttons[_subscribeButtonIndex].ItemText 
          = _subscribe;

        //_timer.Stop();
        //_timer.Report( "Subscription timing" );
        //_timer = null;

        Debug.Print( "Unsubscribed." );
      }
      else
      {
        Debug.Print( "Subscribing..." );

        _event = ExternalEvent.Create( handler );

        _buttons[_subscribeButtonIndex].ItemText 
          = _unsubscribe;

        //_timer = new JtTimer( "Subscription" );

        Debug.Print( "Subscribed." );
      }
      return null != _event;
    }

    /// <summary>
    /// Provide public read-only access to external event.
    /// </summary>
    public static ExternalEvent Event
    {
      get { return _event; }
    }
    #endregion // Idling subscription and external event creation

    public Result OnStartup(
      UIControlledApplication a )
    {
      AddRibbonPanel( a );

      return Result.Succeeded;
    }

    public Result OnShutdown(
      UIControlledApplication a )
    {
      if( Subscribed )
      {
        _event.Dispose();
      }
      return Result.Succeeded;
    }
  }
}
