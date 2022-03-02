using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

public class ItemEditor : EditorWindow
{
    [Serializable, XmlRoot( "Item" )]
    public class IEItem
    {
        public string name { get; set; }
        public int id { get; set; }
        public string itemSpriteFileName16x16 { get; set; }
        public string itemSpriteFileName32x32 { get; set; }
        public string itemSpriteOnFloorFileName { get; set; }
        public string flavourText { get; set; }
    }

    [Serializable, XmlRoot( "Items" )]
    public class IEItemList
    {
        public List<IEItem> list = new List<IEItem>();

        public string[] GetNames
        {
            get
            {
                string[] names = new string[list.Count];

                for ( int i = 0; i < list.Count; i++ )
                {
                    names[i] = list[i].name;
                }

                return names;
            }
        }
    }

    enum WindowState { Main, Create }

    //select item to load
    private WindowState windowState = WindowState.Main;
    private const byte BaseTwo = 2;
    int loadIndex = 0;
    bool loaded = false; int y = 0;

    private const string ServerDir = "C:/Users/Retri/Documents/GitHub/ReldawinUnity2D/ReldawinServerMaster/ReldawinServerMaster/bin/Debug/";


    //item properties
    string _itemName = string.Empty;
    string _itemSpriteFileName16x16 = string.Empty;
    string _itemSpriteFileName32x32 = string.Empty;
    string _itemSpriteOnFloorFileName = string.Empty;
    Sprite _sprite16;
    Sprite _sprite32;
    Sprite _spriteOnFloor;
    string _flavourText;
    int _ID;

    static IEItemList activeList;

    [MenuItem( "Window/Item Editor" )]

    public static void ShowWindow()
    {
        activeList = new IEItemList();
        GetWindow( typeof( ItemEditor ) );
    }

    [System.Obsolete]
    private void OnGUI()
    {
        focusedWindow.minSize = new Vector2( 400, 100 );

        switch ( windowState )
        {
            case WindowState.Main:
                Main();
                break;
            case WindowState.Create:
                Create();
                break;
        }
    }

    private void Main()
    {
        if ( !loaded )
            Load();

        PaintLoadList();

        if ( GUILayout.Button( "New" ) )
        {
            windowState = WindowState.Create;

            _itemName = string.Empty;
            _itemSpriteFileName16x16 = string.Empty;
            _itemSpriteFileName32x32 = string.Empty;
            _itemSpriteOnFloorFileName = string.Empty;
            _sprite16 = null;
            _sprite32 = null;
            _spriteOnFloor = null;
            _flavourText = string.Empty;
            _ID = 0;

            return;
        }
        if ( GUILayout.Button( "Load Selected" ) )
        {
            windowState = WindowState.Create;

            _itemName = activeList.list[loadIndex].name;
            _itemSpriteFileName16x16 = activeList.list[loadIndex].itemSpriteFileName16x16;
            _itemSpriteFileName32x32 = activeList.list[loadIndex].itemSpriteFileName32x32;
            _itemSpriteOnFloorFileName = activeList.list[loadIndex].itemSpriteOnFloorFileName;
            _flavourText = activeList.list[loadIndex].flavourText;
            _ID = activeList.list[loadIndex].id;

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
    }

    private void PaintLoadList()
    {
        string[] options = new string[activeList.list.Count];

        for ( int i = 0; i < options.Length; i++ )
        {
            options[i] = string.Format( "{0} ({1})", activeList.list[i].name, activeList.list[i].id );
        }

        loadIndex = EditorGUI.Popup( new Rect( 4, 42, position.width - 8, 20 ), loadIndex, options );
    }

    [System.Obsolete]
    private void Create()
    {
        y = 4;

        PaintItemName();

        _sprite16 = PaintSpriteField( _sprite16, ref _itemSpriteFileName16x16 );
        _sprite32 = PaintSpriteField( _sprite32, ref _itemSpriteFileName32x32 );
        _spriteOnFloor = PaintSpriteField( _spriteOnFloor, ref _itemSpriteOnFloorFileName);

        PaintID();
        PaintFlavourText();
        PaintSaveButton();
        PaintBackButton();
    }

    private void PaintItemName()
    {
        EditorGUI.LabelField( new Rect( 74, y, position.width, 20 ), "Item Name" ); y += 20;
        _itemName = EditorGUI.TextField( new Rect( 74, y, position.width - 86, y ), _itemName ); y += 30;
    }

    private Sprite PaintSpriteField(Sprite sprite, ref string fileName)
    {
        sprite = (Sprite)EditorGUI.ObjectField( new Rect( 4, y, 64, 64 ), sprite, typeof( Sprite ) );

        if ( sprite != null )
        {
            fileName = sprite.name;
            EditorGUI.LabelField( new Rect( 74, y, position.width - 86, 20 ), "16x16 sprite name" ); y += 20;
            EditorGUI.LabelField( new Rect( 74, y, position.width - 86, 20 ), fileName ); y += 60;
        }
        else
            y += 80;

        return sprite;
    }

    private void PaintFlavourText()
    {
        _flavourText = EditorGUI.TextField( new Rect( 4, y, position.width - 12, 20 ), "Flavour Text", _flavourText ); y += 20;

    }

    private void PaintID()
    {
        _ID = EditorGUI.IntField( new Rect( 4, y, position.width - 12, 20 ), "Base ID", _ID ); y += 22;
    }

    private void PaintSaveButton()
    {
        GUILayout.BeginArea( new Rect( 32, position.height - 70, position.width - 70, 20 ) );
        if ( GUILayout.Button( string.Format( "Save [ {0} ]", _itemName ) ) )
        {
            IEItem newItem = new IEItem();

            newItem.id = this._ID;
            newItem.flavourText = this._flavourText;
            newItem.name = this._itemName;
            newItem.itemSpriteFileName16x16 = this._itemSpriteFileName16x16;
            newItem.itemSpriteFileName32x32 = this._itemSpriteFileName32x32;
            newItem.itemSpriteOnFloorFileName = this._itemSpriteOnFloorFileName;

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

            Save();
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

    public void Save()
    {
        //save to client streaming assets
        XmlSerializer serializer = new XmlSerializer( typeof( IEItemList ) );
        FileStream stream = new FileStream( Application.streamingAssetsPath + "/items.xml", FileMode.Create );
        serializer.Serialize( stream, activeList );
        stream.Close();

        //save to server
        stream = new FileStream( ServerDir + "items.xml", FileMode.Create );
        serializer.Serialize( stream, activeList );
        stream.Close();
    }

    public void Load()
    {
        loaded = true;
        XmlSerializer serializer = new XmlSerializer( typeof( IEItemList ) );
        FileStream stream = new FileStream( Application.streamingAssetsPath + "/items.xml", FileMode.Open );
        activeList = serializer.Deserialize( stream ) as IEItemList;
        stream.Close();
    }
}