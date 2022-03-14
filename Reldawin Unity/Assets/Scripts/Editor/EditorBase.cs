
public class EditorBase : EditorWindow
{
    protected enum WindowState { Main, Create }
    protected const string ServerDir = "C:/Users/Retri/Documents/GitHub/Reldawin/ReldawinServerMaster/ReldawinServerMaster/bin/Debug";

    protected WindowState windowState { get; set; }
    protected int loadIndex { get; set; }    
    protected string FileName { get; set; }
    protected bool loaded { get; set; }
    
    protected T 
    
    private int y { get; set; }
    protected void AddRow()
    {
        y += 22;
    }
    
    protected static void ShowWindow()
    {
        //Override me
    }
    
    private void OnGUI()
    {
        focusedWindow.minSize = new Vector2( 400, 100 );
        
        if ( !loaded )
        {
            Load();
        }

        switch ( windowState )
        {
            case WindowState.Main:
                MainWindow();
                break;
            case WindowState.Create:
                CreationWindow();
                break;
        }
    }
    
    protected void MainWindow()
    {
        PaintLoadList();
        
        if ( GUILayout.Button("New") )
        {
            windowState = WindowState.Create;
        }
        if ( GUILayout.Button("Load Selected") )
        {
            LoadProperties();
            windowState = WindowState.Create;
        }        
    }
    
    //Override this method and end with this.Base();
    protected void CreationWindow()
    {
        PaintSaveButton();
        PaintBackButton();
    }
    
    protected void LoadProperties()
    {
    
    }
    
    protected void PaintIntField(string label, int out value)
    {
        value = EditorGUI.IntField(new Rect(4, y, position.width - 12, 20), label, value);
        AddRow();
    }
    
    protected void PaintFloatField(string label, float out value)
    {
        value = EditorGUI.FloatField(new Rect(4, y, position.width - 12, 20), label, value);
        AddRow();
    }
    
    protected void PaintTextField(string label, string out value)
    {
        value = EditorGUI.TextField(new Rect(4, y, position.width - 12, 20), label, value);
        AddRow();
    }
    
    protected void PaintFloatRange(string label, float out min, float out max, float minRange, float maxRange)
    {
        EditorGUIUtility.wideMode = true;
        {
            EditorGUI.MinMaxSlider(
                new Rect( 4, y, position.width - 12, 20 ),
                new GUIContent( label ),
                min, 
                max,
                minRange, 
                maxRange                
                );
        }
        EditorGUIUtility.wideMode = false;
        AddRow();
    }
    
    protected void PaintHorizontalLine()
    {
        Handles.color = Color.gray;
        Handles.DrawLine( new Vector3( 4, y + 11 ), new Vector3( position.width - 8, y + 11 ) );
        AddRow();
    }
    
    protected void PaintPopup(string label, string[] options, int out value)
    {
        value = EditorGUI.Popup(new Rect(4, y, position.width - 8, 20, value, options);
        AddRows();
    }
    
    protected void Load<T>
    {
        loaded = true;
        
        try
        {
            XmlSerializer serializer = new XmlSerializer( typeof( T ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + FileName, FileMode.Open );
            obj = serializer.Deserialize( stream ) as T;
            stream.Close();
        }
        catch
        {
            Debug.LogWarning( "Could not open " + FileName );
        }
       
    }
}
