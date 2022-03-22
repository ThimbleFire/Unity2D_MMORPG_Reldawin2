using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class EditorBase : EditorWindow
{
    protected enum WindowStates { Main, Create }

    protected WindowStates WindowState { get; set; }
    protected int LoadIndex { get; set; }
    protected bool Loaded { get; set; }
    protected string[] LoadOptions { get; set; }

    protected int y { get; set; }
    protected void AddRow() { y += 22; }
    protected void ResetRow() { y = 4; }

    private void OnGUI()
    {
        focusedWindow.minSize = new Vector2( 400, 600 );

        ResetRow();

        if ( !Loaded )
        {
            Load();
        }

        switch ( WindowState )
        {
            case WindowStates.Main:
                MainWindow();
                break;
            case WindowStates.Create:
                CreationWindow();
                break;
        }
    }

    protected void MainWindow()
    {
        GUILayout.BeginArea( new Rect( 4, y, position.width - 12, 20 ) );
        if ( GUILayout.Button( "New" ) )
        {
            ResetProperties();
            WindowState = WindowStates.Create;
        }
        GUILayout.EndArea();
        AddRow();

        GUILayout.BeginArea( new Rect( 4, y, position.width - 12, 20 ) );
        if ( GUILayout.Button( "Load Selected" ) )
        {
            LoadProperties();
            WindowState = WindowStates.Create;
        }
        GUILayout.EndArea();

        AddRow();

        PaintLoadList();
    }

    /// <summary>Called when opening a new form.</summary>
    protected virtual void ResetProperties()
    {

    }

    /// <summary>The paint method for creation window that houses paint calls</summary>
    protected virtual void CreationWindow()
    {
        PaintSaveButton();
        PaintBackButton();
    }
    protected virtual void OnClick_SaveButton()
    {
        //override me
    }

    ///<summary> Paints the dropdown box with all the elements </summary>
    protected void PaintLoadList()
    {
        LoadIndex = PaintPopup( LoadOptions, LoadIndex );
    }

    protected void PaintIntField( ref int value, string label = "" )
    {
        value = EditorGUI.IntField( new Rect( 4, y, position.width - 12, 20 ), label, value );
        AddRow();
    }
    protected void PaintFloatField( ref float value, string label = "" )
    {
        value = EditorGUI.FloatField( new Rect( 4, y, position.width - 12, 20 ), label, value );
        AddRow();
    }
    protected void PaintTextField( ref string value, string label = "" )
    {
        value = EditorGUI.TextField( new Rect( 4, y, position.width - 12, 20 ), label, value );
        AddRow();
    }
    protected void PaintFloatRange( ref float min, ref float max, float minRange, float maxRange, string label = "" )
    {
        EditorGUIUtility.wideMode = true;
        {
            EditorGUI.MinMaxSlider(
                new Rect( 4, y, position.width - 12, 20 ),
                new GUIContent( label ),
                ref min,
                ref max,
                minRange,
                maxRange
                );
        }
        EditorGUIUtility.wideMode = false;
        AddRow();
    }
    protected int PaintAddProbability( ref int value, ref int probability, ref List<Droprate> droprates, ref IEItemList container )
    {
        if ( container == null )
            return 0;
        if ( container.list == null )
            return 0;

        int v = EditorGUI.Popup( new Rect( 4, y, position.width - 8, 20 ), "Item", value, container.GetNames );
        AddRow();

        PaintIntSlider( ref probability, 0, 100, "Probability" );

        EditorGUI.BeginDisabledGroup( probability == 0 );
        GUILayout.BeginArea( new Rect( 4, y, position.width - 12, 20 ) );
        AddRow();
        if ( GUILayout.Button( "Add probability" ) )
        {
            droprates.Add( new Droprate() { id = container.list[v].id, percent = probability } );
            v = 0;
            probability = 0;
        }
        GUILayout.EndArea();
        EditorGUI.EndDisabledGroup();

        return v;
    }
    protected int PaintAddProbability( ref int value, ref int probability, ref List<Droprate> droprates, ref DEDoodadList container )
    {
        if ( container == null )
            return 0;
        if ( container.list == null )
            return 0;

        int v = EditorGUI.Popup( new Rect( 4, y, position.width - 8, 20 ), "Item", value, container.GetNames );
        AddRow();

        PaintIntSlider( ref probability, 0, 100, "Probability" );

        EditorGUI.BeginDisabledGroup( probability == 0 );
        GUILayout.BeginArea( new Rect( 4, y, position.width - 12, 20 ) );
        AddRow();
        if ( GUILayout.Button( "Add probability" ) )
        {
            droprates.Add( new Droprate() { id = container.list[v].id, percent = probability } );
            v = 0;
            probability = 0;
        }
        GUILayout.EndArea();
        EditorGUI.EndDisabledGroup();

        return v;
    }
    protected void PaintHorizontalLine()
    {
        Handles.color = Color.gray;
        Handles.DrawLine( new Vector3( 4, y + 11 ), new Vector3( position.width - 8, y + 11 ) );
        AddRow();
    }
    protected int  PaintPopup( string[] options, int value, string label = "" )
    {
        int v = EditorGUI.Popup( new Rect( 4, y, position.width - 8, 20 ), value, options );
        AddRow();
        return v;
    }
    protected void PaintSpriteField( ref Sprite sprite, ref string fileName, string label = "" )
    {
        sprite = (Sprite)EditorGUI.ObjectField( new Rect( 4, y, 64, 64 ), sprite, typeof( Sprite ), false );

        if ( sprite != null )
        {
            fileName = sprite.name;
            EditorGUI.LabelField( new Rect( 74, y, position.width - 86, 20 ), label );
            AddRow();
            EditorGUI.LabelField( new Rect( 74, y, position.width - 86, 20 ), label );
            AddRow();
            AddRow();
            AddRow();
        }
        else
        {
            AddRow();
            AddRow();
            AddRow();
            AddRow();
        }
    }
    protected void PaintIntSlider( ref int value, int min, int max, string label = "" )
    {
        value = EditorGUI.IntSlider( new Rect( 4, y, position.width - 12, 20 ), label, value, min, max );
        AddRow();
    }
    protected void PaintDroprates( List<Droprate> droprates, DEDoodadList doodadList )
    {
        if ( droprates != null )
            if ( droprates.Count > 0 )
                foreach ( Droprate droprate in droprates )
                {
                    EditorGUI.LabelField( new Rect( 4, y, position.width - 12, 20 ), doodadList.list.Find( x => x.id == droprate.id ).name );
                    EditorGUI.LabelField( new Rect( position.width - 40, y, position.width - 12, 20 ), ( droprate.percent ).ToString() + " %" );
                    AddRow();
                }
    }
    protected void PaintDroprates( List<Droprate> droprates, IEItemList itemList )
    {
        if ( droprates != null )
            if ( droprates.Count > 0 )
                foreach ( Droprate droprate in droprates )
                {
                    EditorGUI.LabelField( new Rect( 4, y, position.width - 12, 20 ), itemList.list.Find( x => x.id == droprate.id ).name );
                    EditorGUI.LabelField( new Rect( position.width - 40, y, position.width - 12, 20 ), ( droprate.percent ).ToString() + " %" );
                    AddRow();
                }
    }

    private void   PaintSaveButton()
    {
        GUILayout.BeginArea( new Rect( 32, position.height - 70, position.width - 70, 20 ) );
        if ( GUILayout.Button( string.Format( "Save" ) ) )
        {
            OnClick_SaveButton();
        }
        GUILayout.EndArea();
    }
    private void   PaintBackButton()
    {
        GUILayout.BeginArea( new Rect( 32, position.height - 40, position.width - 70, 20 ) );
        if ( GUILayout.Button( "Back" ) )
        {
            WindowState = WindowStates.Main;
            Loaded = false;
        }
        GUILayout.EndArea();
    }

    /// <summary>Called on save button click</summary>
    protected void Save<T>( T dataToSerialize, string filename )
    {
        string ServerDir = "C:/Users/Retri/Documents/GitHub/Reldawin/ReldawinServerMaster/ReldawinServerMaster/bin/Debug";

        XmlSerializer serializer = new XmlSerializer( typeof( T ) );

        using ( FileStream stream = new FileStream( Application.streamingAssetsPath + filename, FileMode.Create ) )
        serializer.Serialize( stream, dataToSerialize );

        using ( FileStream stream = new FileStream( ServerDir + filename, FileMode.Create ) )
        serializer.Serialize( stream, dataToSerialize );
    }
    
    /// <summary>Called by Load in inheriting class</summary>
    protected T Load<T>(string filename)
    {
        XmlSerializer xmlSerializer = new XmlSerializer( typeof( T ) );
        using TextReader reader = new StreamReader( Application.streamingAssetsPath + filename );
        return (T)xmlSerializer.Deserialize( reader );
    }
    
    /// <summary>Called when opening the editor window. To be overriden</summary>
    protected virtual void Load()
    {
        //override me
    }
    
    /// <summary>Called when loading selected</summary>
    protected virtual void LoadProperties() { }

}
