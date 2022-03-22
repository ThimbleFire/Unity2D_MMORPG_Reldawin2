using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class TileEditor : EditorBase
{
    private static TETileList activeList;
    private DEDoodadList doodadList;
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
    private static void ShowWindow()
    {
        GetWindow( typeof( TileEditor ) );
    }

    protected override void ResetProperties()
    {
        _tileName = string.Empty;
        _ID = 0;
        _minHeight = 0;
        _maxHeight = 0;
        _layerIndex = 0;
        _probabilities.Clear();
        tempProbabilityOptionIndex = 0;
        tempProbabilitySpawnRate = 0;
    }

    protected override void OnClick_SaveButton()
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

        Save( activeList, "/tiles.xml" );

        _probabilities.Clear();
    }

    protected override void CreationWindow()
    {
        PaintTextField(ref _tileName, "Name");
        PaintIntField( ref _ID, "ID" );
        PaintFloatField( ref _minHeight, "Min Height" );
        PaintFloatField( ref _maxHeight, "Max Height" );
        PaintIntField( ref _layerIndex, "Layer Index" );
        PaintHorizontalLine();
        PaintAddProbability( ref tempProbabilityOptionIndex, ref tempProbabilitySpawnRate, ref _probabilities, ref doodadList );
        PaintHorizontalLine();
        PaintDroprates( _probabilities, doodadList );

        base.CreationWindow();
    }

    protected override void Load()
    {
        activeList = Load<TETileList>( "/tiles.xml" );
        LoadOptions = activeList.GetNames;
        doodadList = Load<DEDoodadList>( "/doodads.xml" );
    }

    protected override void LoadProperties()
    {
        _tileName = activeList.list[LoadIndex].name;
        _ID = activeList.list[LoadIndex].id;
        _minHeight = activeList.list[LoadIndex].minHeight;
        _maxHeight = activeList.list[LoadIndex].maxHeight;
        _layerIndex = activeList.list[LoadIndex].layerIndex;
        _probabilities = new List<Droprate>( activeList.list[LoadIndex].droprates );
    }
}