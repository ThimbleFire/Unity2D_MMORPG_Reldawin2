using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System;

public class DoodadEditor : EditorBase
{
    private static DEDoodadList activeList;
    private static IEItemList itemList;
    //doodad properties
    private string _doodadName = string.Empty;
    private int _ID = 0;
    private List<Droprate> yieldRates = new List<Droprate>();
    private int tempProbabilityOptionIndex = 0;
    private int tempProbabilitySpawnRate = 0;
    DEDoodad.Interact _interact = DEDoodad.Interact.NONE;

    [MenuItem( "Window/Doodad Editor" )]
    private static void ShowWindow()
    {
        GetWindow( typeof( DoodadEditor ) );
    }

    protected override void MainWindow()
    {
        IncludeLoadList = true;
        base.MainWindow();
    }
    protected override void CreationWindow()
    {

        PaintTextField( ref _doodadName, "Name" );
        PaintIntField( ref _ID, "ID" );
        _interact = (DEDoodad.Interact)PaintPopup( Enum.GetNames( typeof( DEDoodad.Interact ) ), (int)_interact );
        PaintHorizontalLine();
        tempProbabilityOptionIndex = PaintAddProbability( ref tempProbabilityOptionIndex, ref tempProbabilitySpawnRate, ref yieldRates, ref itemList );
        PaintHorizontalLine();
        PaintDroprates( yieldRates, itemList );

        base.CreationWindow();
    }
    protected override void Load()
    {
        activeList = Load<DEDoodadList>( "/doodads.xml" );
        LoadOptions = activeList.GetNames;

        if ( File.Exists( Application.streamingAssetsPath + "/items.xml" ) )
            itemList = Load<IEItemList>( "/items.xml" );

        IncludeLoadList = true;
        IncludeSaveBtn = true;
        IncludeBackBtn = true;
    }
    protected override void OnClick_SaveButton()
    {
        DEDoodad newDoodad = new DEDoodad
        {
            id = _ID,
            name = _doodadName,
            droprates = yieldRates.ToArray(),
            interact = _interact
        };

        if ( activeList == null )
            return;

        if ( activeList.list == null )
            return;

        DEDoodad doodadInList = activeList.list.Find( x => x.id == _ID );

        if ( doodadInList == null )
            activeList.list.Add( newDoodad );
        else
        {
            activeList.list.Remove( doodadInList );
            activeList.list.Add( newDoodad );
        }

        Save( activeList, "/doodads.xml" );

        yieldRates.Clear();
    }
    protected override void LoadProperties()
    {
        _doodadName = activeList.list[LoadIndex].name;
        _ID = activeList.list[LoadIndex].id;
        yieldRates = new List<Droprate>( activeList.list[LoadIndex].droprates );
        _interact = activeList.list[LoadIndex].interact;
    }
    protected override void ResetProperties()
    {
        _doodadName = string.Empty;
        _ID = 0;
        yieldRates.Clear();
        _interact = DEDoodad.Interact.NONE;
    }
}