using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;

public class ItemEditor : EditorBase
{
    IEItemList activeList;

    //item properties
    private string _itemName = string.Empty;
    private string _itemSpriteFileName16x16 = string.Empty;
    private string _itemSpriteFileName32x32 = string.Empty;
    private string _itemSpriteOnFloorFileName = string.Empty;
    private Sprite _sprite16;
    private Sprite _sprite32;
    private Sprite _spriteOnFloor;
    private string _flavourText;
    private int _ID;

    [MenuItem( "Window/Item Editor" )]
    private static void ShowWindow()
    {
        GetWindow( typeof( ItemEditor ) );
    }

    protected override void MainWindow()
    {
        IncludeLoadList = true;
        base.MainWindow();
    }
    protected override void ResetProperties()
    {
        _itemName = string.Empty;
        _itemSpriteFileName16x16 = string.Empty;
        _itemSpriteFileName32x32 = string.Empty;
        _itemSpriteOnFloorFileName = string.Empty;
        _sprite16 = null;
        _sprite32 = null;
        _spriteOnFloor = null;
        _flavourText = string.Empty;
        _ID = 0;
    }
    protected override void LoadProperties()
    {
        _itemName = activeList.list[LoadIndex].name;
        _itemSpriteFileName16x16 = activeList.list[LoadIndex].itemSpriteFileName16x16;
        _itemSpriteFileName32x32 = activeList.list[LoadIndex].itemSpriteFileName32x32;
        _itemSpriteOnFloorFileName = activeList.list[LoadIndex].itemSpriteOnFloorFileName;
        _flavourText = activeList.list[LoadIndex].flavourText;
        _ID = activeList.list[LoadIndex].id;

        Sprite[] sprites16 = Resources.LoadAll<Sprite>( "Sprites/Interface/Items/Items_16x16" );
        Sprite[] sprites32 = Resources.LoadAll<Sprite>( "Sprites/Interface/Items/Items_32x32" );

        foreach ( Sprite sprite in sprites16 )
        {
            if ( sprite.name == _itemSpriteFileName16x16 )
            {
                _sprite16 = sprite;
                break;
            }
        }

        foreach ( Sprite sprite in sprites32 )
        {
            if ( sprite.name == _itemSpriteFileName32x32 )
            {
                _sprite32 = sprite;
                _spriteOnFloor = sprite; //temporary. Eventually we want a unique sprite for items on floor
                break;
            }
        }
    }
    protected override void CreationWindow()
    {
        PaintTextField( ref _itemName, "Name" );
        PaintTextField( ref _flavourText, "Flavour Text" );
        PaintSpriteField( ref _sprite16, ref _itemSpriteFileName16x16, "16x16" );
        PaintSpriteField( ref _sprite32, ref _itemSpriteFileName32x32, "32x32" );
        PaintSpriteField( ref _spriteOnFloor, ref _itemSpriteOnFloorFileName, "OnFloor" );

        base.CreationWindow();
    }
    protected override void OnClick_SaveButton()
    { 
        IEItem newItem = new IEItem
        {
            id = _ID,
            flavourText = _flavourText,
            name = _itemName,
            itemSpriteFileName16x16 = _itemSpriteFileName16x16,
            itemSpriteFileName32x32 = _itemSpriteFileName32x32,
            itemSpriteOnFloorFileName = _itemSpriteOnFloorFileName,
        };

        if ( activeList != null )
        {
            if ( activeList.list != null )
            {
                IEItem itemInList = activeList.list.Find( x => x.id == newItem.id );

                if ( itemInList == null )
                    activeList.list.Add( newItem );
                else
                {
                    activeList.list.Remove( itemInList );
                    activeList.list.Add( newItem );
                }
            }
        }

        Save( activeList, "/items.xml" );
    }
    protected override void Load()
    {
        activeList = Load<IEItemList>( "/items.xml" );
        LoadOptions = activeList.GetNames;
    }
}