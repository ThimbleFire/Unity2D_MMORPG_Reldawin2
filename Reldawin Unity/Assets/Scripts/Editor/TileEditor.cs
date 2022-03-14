using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class TileEditor : BaseWindow
{
    private static TETileList activeList;
    private DEDoodadList doodadList;;
    private int tempProbabilityOptionIndex = 0;
    private int tempProbabilitySpawnRate = 0;
    
    //tile properties
    private string _tileName;
    private int _ID;
    private float _minHeight;
    private float _maxHeight;
    private int _layerIndex = 0;
    private List<Droprate> _probabilities = new List<Droprate>();

    [MenuItem( "Window/Tile Editor" )]
    public static void ShowWindow()
    {
        GetWindow( typeof( TileEditor ) );
    }

    private void OnGUI()
    {
        focusedWindow.minSize = new Vector2( 400, 100 );

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

    public void MainWindow()
    {
        if ( !loaded )
        {
            Load();
            LoadDoodads();
        }

        PaintLoadList();

        if ( GUILayout.Button("New") )
        {
            windowState = WindowState.Create;
            return;
        }
        if ( GUILayout.Button("Load Selected") )
        {
            windowState = WindowState.Create;

            _tileName = activeList.list[loadIndex].name;
            _ID = activeList.list[loadIndex].id;
            _minHeight = activeList.list[loadIndex].minHeight;
            _maxHeight = activeList.list[loadIndex].maxHeight;
            _layerIndex = activeList.list[loadIndex].layerIndex;
            _probabilities = new List<Droprate>(activeList.list[loadIndex].droprates );
            
            return;
        }
    }

    private void CreationWindow()
    {
        y = 4;

        PaintTileName();
        PaintID();
        PaintHeight();
        PaintHeightMin();
        PaintHeightMax();
        PaintLayerIndex();
        PaintHorizontalLine();
        DoodadSpawnProbability();
        PaintHorizontalLine();
        PaintProbabilities();

        PaintSaveButton();
        PaintBackButton();
    }

    private void PaintProbabilities()
    {
        foreach ( Droprate prob in _probabilities )
        {
            EditorGUI.LabelField( new Rect( 4, y, position.width - 12, 20 ), doodadList.GetNames[prob.id].ToString() + " (" + prob.id.ToString() + ")" );
            EditorGUI.LabelField( new Rect( position.width / 2, y, position.width / 2, 20 ), prob.percent.ToString() + " %" ); 
            // we need a button to remove probabilities
            //if (GUILayout.Button("x")) { _probabilities.Remove(prob); return; }
            y += 22;
        }
    }

    private void PaintHorizontalLine()
    {
        Handles.color = Color.gray;
        Handles.DrawLine( new Vector3( 4, y + 11 ), new Vector3( position.width - 8, y + 11 ) ); y += 22;
    }

    private void DoodadSpawnProbability()
    {
        tempProbabilityOptionIndex = EditorGUI.Popup(
            new Rect( 4, y, position.width - 12, 20 ),
            "Doodad", 
            tempProbabilityOptionIndex, 
            doodadList.GetNames ); y += 22;

        tempProbabilitySpawnRate = EditorGUI.IntSlider( new Rect( 4, y, position.width - 12, 20 ), "Spawnrate", tempProbabilitySpawnRate, 0, 100 ); y += 22;

        EditorGUI.BeginDisabledGroup( tempProbabilitySpawnRate == 0 );
        GUILayout.BeginArea( new Rect( 4, y, position.width - 12, 20 ), "Dicks" ); y += 22;
        if ( GUILayout.Button( "Add probability" ) )
        {
            _probabilities.Add( new Droprate() { id = tempProbabilityOptionIndex, percent = tempProbabilitySpawnRate } );
            tempProbabilityOptionIndex = 0;
            tempProbabilitySpawnRate = 0;
        }
        GUILayout.EndArea();
        EditorGUI.EndDisabledGroup();
    }

    private void PaintLayerIndex()
    {
        _layerIndex = EditorGUI.IntField( new Rect( 4, y, position.width - 12, 20 ), "Layer Index", _layerIndex ); y += 22;
    }

    private void PaintTileName()
    {
        _tileName = EditorGUI.TextField( new Rect( 4, y, position.width - 12, 20 ), "Tile Name", _tileName ); y += 22;
    }

    private void PaintID()
    {
        _ID = EditorGUI.IntField( new Rect( 4, y, position.width - 12, 20 ), "Base ID", _ID ); y += 22;
    }

    private void PaintHeight()
    {
        EditorGUIUtility.wideMode = true;
        {
            EditorGUI.MinMaxSlider(
                new Rect( 4, y, position.width - 12, 20 ),
                new GUIContent( "Height:" ),
                ref _minHeight, 
                ref _maxHeight,
                0.0f, 
                1.0f                
                 ); y += 22;
        }
        EditorGUIUtility.wideMode = false;
    }

    private void PaintHeightMin()
    {
        _minHeight = EditorGUI.FloatField( new Rect( 4, y, position.width - 12, 20 ), "        Min", _minHeight ); y += 22;
    }

    private void PaintHeightMax()
    {
        _maxHeight = EditorGUI.FloatField( new Rect( 4, y, position.width - 12, 20 ),"        Max", _maxHeight ); y += 22;
    }

    private void PaintSaveButton()
    {
        GUILayout.BeginArea( new Rect( 32, position.height - 70, position.width - 70, 20 ) );

        if ( GUILayout.Button( string.Format( "Save [ {0} ]", _tileName ) ) )
        {
            TETile newTile = new TETile
            {

                id = _ID,
                layerIndex = _layerIndex,
                maxHeight = _maxHeight,
                minHeight = _minHeight,
                name = _tileName,
                droprates = _probabilities.ToArray()
            };

            if ( activeList != null )
            {
                if ( activeList.list != null )
                {
                    TETile tileInList = activeList.list.Find( x => x.id == newTile.id );

                    if ( tileInList == null )
                        activeList.list.Add( newTile );
                    else
                    {
                        activeList.list.Remove( tileInList );
                        activeList.list.Add( newTile );
                    }
                    
                }
            }
            
            Save();

            _probabilities.Clear();
        }
        GUILayout.EndArea();
    }

    private void PaintBackButton()
    {
        GUILayout.BeginArea( new Rect( 32, position.height - 40, position.width - 70, 20 ) );
        if ( GUILayout.Button( "Back" ) )
        {
            windowState = WindowState.Main;
            loaded = false;
        }
        GUILayout.EndArea();
    }

    private void PaintLoadList()
    {
        string[] options = new string[activeList.list.Count];

        for ( int i = 0; i < options.Length; i++ )
        {
            options[i] = string.Format("{0} ({1})", activeList.list[i].name, activeList.list[i].id);
        }

        loadIndex = EditorGUI.Popup( new Rect( 4, 44, position.width - 8, 20 ), loadIndex, options );
    }

    private void Save()
    {
        XmlSerializer serializer = new XmlSerializer( typeof( TETileList ) );
        FileStream stream = new FileStream( Application.streamingAssetsPath + "/tiles.xml", FileMode.Create );
        serializer.Serialize( stream, activeList );
        stream.Close();

        //save to server
        stream = new FileStream( ServerDir + "tiles.xml", FileMode.Create );
        serializer.Serialize( stream, activeList );
        stream.Close();
    }

    private void Load()
    {
        loaded = true;

        try
        {
            XmlSerializer serializer = new XmlSerializer( typeof( TETileList ) );
            FileStream stream = new FileStream( Application.streamingAssetsPath + "/tiles.xml", FileMode.Open );
            activeList = serializer.Deserialize( stream ) as TETileList;
            stream.Close();
        }
        catch
        {
            Debug.LogWarning( "Could not open tiles.xml" );
        }


        if ( activeList == null )
            activeList = new TETileList();
    }

    private void LoadDoodads()
    {
        loaded = true;
        try
        {
            XmlSerializer serializer = new XmlSerializer( typeof( DEDoodadList ) );
        FileStream stream = new FileStream( Application.streamingAssetsPath + "/doodads.xml", FileMode.Open );
        doodadList = serializer.Deserialize( stream ) as DEDoodadList;
        stream.Close();
        }
        catch
        {
            Debug.LogWarning( "Could not open doodads.xml" );
        }

        if ( doodadList == null )
            doodadList = new DEDoodadList();
    }
}
