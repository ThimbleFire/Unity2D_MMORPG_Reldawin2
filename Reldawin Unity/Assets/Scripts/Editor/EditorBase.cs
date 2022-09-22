using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class EditorBase : EditorWindow
{
    private const byte Right = 20;

    protected Sprite[] btnState;
    private Vector2 scrollPos;

    protected enum WindowStates { Main, Create }
    protected WindowStates WindowState { get; set; }
    protected int LoadIndex { get; set; }
    protected bool Loaded { get; set; }
    protected string[] LoadOptions { get; set; }
    protected bool IncludeLoadList { get; set; }
    protected bool IncludeSaveBtn { get; set; }
    protected bool IncludeBackBtn { get; set; }

    protected int Y { get; set; }

    private void OnGUI()
    {
        ResetRow();

        if ( !Loaded )
        {
            focusedWindow.minSize = new Vector2( 400, 600 );
            Load();
        }

        switch ( WindowState )
        {
            case WindowStates.Main:
                MainWindow();
                break;
            case WindowStates.Create:
                EditorGUILayout.BeginVertical();
                scrollPos = EditorGUILayout.BeginScrollView( scrollPos, false, true );
                {
                    if ( IncludeSaveBtn )
                        PaintSaveButton();
                    if ( IncludeBackBtn )
                        PaintBackButton();
                    CreationWindow();
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                break;
        }
    }

    protected virtual void MainWindow()
    {
        GUILayout.BeginArea( new Rect( 4, Y, position.width - Right, 20 ) );
        if ( GUILayout.Button( "New" ) )
        {
            ResetProperties();
            WindowState = WindowStates.Create;
        }

        GUILayout.EndArea();
        AddRow();

        if ( IncludeLoadList )
        {
            GUILayout.BeginArea( new Rect( 4, Y, position.width - Right, 20 ) );
            if ( GUILayout.Button( "Load Selected" ) )
            {
                LoadProperties();
                WindowState = WindowStates.Create;
            }
            GUILayout.EndArea();

            AddRow();

            PaintLoadList();
        }
    }

    /// <summary>Called when clicking New button</summary>
    protected virtual void ResetProperties()
    {

    }
    protected virtual void CreationWindow()
    {

    }
    protected virtual void OnClick_SaveButton()
    {
        //override me
    }
    private void AddRow() { Y += 22; EditorGUILayout.Space( 22, true ); }
    private void ResetRow() { Y = 4; }

    private void PaintSaveButton()
    {
        GUILayout.BeginArea( new Rect( 4, Y, position.width - Right, 21 ) );
        if ( GUILayout.Button( string.Format( "Save" ) ) )
        {
            OnClick_SaveButton();
        }
        GUILayout.EndArea();
        AddRow();
    }
    private void PaintBackButton()
    {
        GUILayout.BeginArea( new Rect( 4, Y, position.width - Right, 21 ) );
        if ( GUILayout.Button( "Back" ) )
        {
            WindowState = WindowStates.Main;
            Loaded = false;
        }
        GUILayout.EndArea();
        AddRow();
    }
    protected void PaintLoadList()
    {
        LoadIndex = PaintPopup( LoadOptions, LoadIndex );
    }
    protected void PaintIntField( ref int value, string label = "" )
    {
        value = EditorGUI.IntField( new Rect( 4, Y, position.width - Right, 20 ), label, value );
        AddRow();
    }
    protected void PaintFloatField( ref float value, string label = "" )
    {
        value = EditorGUI.FloatField( new Rect( 4, Y, position.width - Right, 20 ), label, value );
        AddRow();
    }
    protected void PaintTextField( ref string value, string label = "" )
    {
        value = EditorGUI.TextField( new Rect( 4, Y, position.width - Right, 20 ), label, value );
        AddRow();
    }
    protected void PaintFloatRange( ref float min, ref float max, float minRange, float maxRange, string label = "" )
    {
        EditorGUIUtility.wideMode = true;
        {
            EditorGUI.MinMaxSlider(
                new Rect( 4, Y, position.width - Right, 20 ),
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

        int v = EditorGUI.Popup( new Rect( 4, Y, position.width - Right, 20 ), "Item", value, container.GetNames );
        AddRow();

        PaintIntSlider( ref probability, 0, 100, "Probability" );

        EditorGUI.BeginDisabledGroup( probability == 0 );
        GUILayout.BeginArea( new Rect( 4, Y, position.width - Right, 20 ) );
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

        int v = EditorGUI.Popup( new Rect( 4, Y, position.width - Right, 20 ), "Item", value, container.GetNames );
        AddRow();

        PaintIntSlider( ref probability, 0, 100, "Probability" );

        EditorGUI.BeginDisabledGroup( probability == 0 );
        GUILayout.BeginArea( new Rect( 4, Y, position.width - Right, 20 ) );
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
        Handles.DrawLine( new Vector3( 4, Y + 11 ), new Vector3( position.width - Right, Y + 11 ) );
        AddRow();
    }
    protected int PaintPopup( string[] options, int value, string label = "" )
    {
        int v = EditorGUI.Popup( new Rect( 4, Y, position.width - Right, 20 ), value, options );
        AddRow();
        return v;
    }
    protected void PaintSpriteField( ref Sprite sprite, ref string fileName, string label = "" )
    {
        sprite = (Sprite)EditorGUI.ObjectField( new Rect( 4, Y, 64, 64 ), sprite, typeof( Sprite ), false );

        if ( sprite != null )
        {
            fileName = sprite.name;
            EditorGUI.LabelField( new Rect( 74, Y, position.width - 86, 20 ), label );
            AddRow();
            EditorGUI.LabelField( new Rect( 74, Y, position.width - 86, 20 ), label );
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
        value = EditorGUI.IntSlider( new Rect( 4, Y, position.width - Right, 20 ), label, value, min, max );
        AddRow();
    }
    protected void PaintDroprates( List<Droprate> droprates, DEDoodadList doodadList )
    {
        if ( droprates != null )
            if ( droprates.Count > 0 )
                foreach ( Droprate droprate in droprates )
                {
                    EditorGUI.LabelField( new Rect( 4, Y, position.width - Right, 20 ), doodadList.list.Find( x => x.id == droprate.id ).name );
                    EditorGUI.LabelField( new Rect( position.width - 40, Y, position.width - Right, 20 ), ( droprate.percent ).ToString() + " %" );
                    AddRow();
                }
    }
    protected void PaintDroprates( List<Droprate> droprates, IEItemList itemList )
    {
        if ( droprates != null )
            if ( droprates.Count > 0 )
                foreach ( Droprate droprate in droprates )
                {
                    EditorGUI.LabelField( new Rect( 4, Y, position.width - Right, 20 ), itemList.list.Find( x => x.id == droprate.id ).name );
                    EditorGUI.LabelField( new Rect( position.width - 40, Y, position.width - Right, 20 ), ( droprate.percent ).ToString() + " %" );
                    AddRow();
                }
    }
    protected void PaintSpriteAtlasKey( Sprite sprite, ref short[] state )
    {
        short[] orderOfNeighbours = new short[8] { 7, 0, 4, 3, 1, 6, 2, 5 };
        EditorGUI.ObjectField( new Rect( 4, Y, 64, 32 ), sprite, typeof( Sprite ), false );

        for ( int counter = 0, z = 0; z < 3; z++ )
        {
            for ( int x = 0; x < 3; x++ )
            {
                if ( x == 1 && z == 1 )
                {
                    x = 2;
                }

                GUILayout.BeginArea( new Rect( 72 + x * 24, Y + z * 24, 24, 24 ) );
                {
                    switch ( state[orderOfNeighbours[counter]] )
                    {
                        case 0:
                            if ( GUILayout.Button( new GUIContent( string.Empty, btnState[0].texture ) ) )
                                state[orderOfNeighbours[counter]] = 1;
                            break;
                        case 1:
                            if ( GUILayout.Button( new GUIContent( string.Empty, btnState[1].texture ) ) )
                                state[orderOfNeighbours[counter]] = 2;
                            break;
                        case 2:
                            if ( GUILayout.Button( new GUIContent( string.Empty, btnState[2].texture ) ) )
                                state[orderOfNeighbours[counter]] = 0;
                            break;
                    }
                }
                GUILayout.EndArea();

                GUILayout.BeginArea( new Rect( 150 + x * 24, Y + z * 24, 24, 24 ) );
                {
                    GUILayout.Label( orderOfNeighbours[counter].ToString() );
                }
                GUILayout.EndArea();

                counter++;
            }
        }

        AddRow();
        AddRow();
        AddRow();
        AddRow();
    }
    protected bool PaintButton( string message )
    {
        bool result = false;

        GUILayout.BeginArea( new Rect( 4, Y, position.width - Right, 20 ) );
        if ( GUILayout.Button( message ) )
        {
            result = true;
        }
        GUILayout.EndArea();
        AddRow();

        return result;
    }
    protected void PaintLabel( string message )
    {
        EditorGUI.LabelField( new Rect( 4, Y, position.width - Right, 20 ), message );
        AddRow();
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
    protected T Load<T>( string filename )
    {
        XmlSerializer xmlSerializer = new XmlSerializer( typeof( T ) );
        try
        {
            using TextReader reader = new StreamReader( Application.streamingAssetsPath + filename );
            return (T)xmlSerializer.Deserialize( reader );
        }
        catch ( System.Exception )
        {
            Debug.LogError( filename + " not found" );
            return (T)default;
        }
    }

    /// <summary>Called when opening the editor window. To be overriden</summary>
    protected virtual void Load()
    {
        btnState = new Sprite[3]
        {
            Resources.Load<Sprite>( "Sprites/System/EditorButtons" ),
            Resources.Load<Sprite>( "Sprites/System/EditorButtons2" ),
            Resources.Load<Sprite>( "Sprites/System/EditorButtons3" )
        };

        Loaded = true;
        //override me
    }

    /// <summary>Called when loading selected</summary>
    protected virtual void LoadProperties() { }

}