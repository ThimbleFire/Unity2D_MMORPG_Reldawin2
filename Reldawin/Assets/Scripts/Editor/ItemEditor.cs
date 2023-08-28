using System;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor;
using UnityEngine;

namespace AlwaysEast
{
    public class ItemFileData
    {
        [Flags]
        public enum TAffixes : short
        {
            Plus_Accuracy =          0b00000000,
            Dmg_Phys_Min =           0b00000001,
            Dmg_Phys_Max =           0b00000010,
            Dmg_Phys_Percent =       0b00000011,
            Dmg_Ele_Fire =           0b00000100,
            Dmg_Ele_Cold =           0b00000101,
            Dmg_Ele_Lightning =      0b00000110,
            Dmg_Ele_Poison =         0b00000111,
            Def_Phys_Flat =          0b00001000,
            Def_Phys_Percent =       0b00001001,
            Def_Dmg_Reduction_Phys = 0b00001010,
            Def_Dmg_Reduction_Magic =0b00001011,
            Def_Dmg_Reduction_All =  0b00001100,
            Def_Ele_Res_Fire =       0b00001101,
            Def_Ele_Res_Cold =       0b00001110,
            Def_Ele_Res_Lightning =  0b00001111,
            Def_Ele_Res_Poison =     0b00010000,
            Def_Ele_Res_All =        0b00010001,
            On_Hit_Life =            0b00010010,
            On_Kill_Life =           0b00010011,
            Plus_Durability =        0b00010100,
            On_Hit_Mana =            0b00010101,
            Plus_Life =              0b00010110,
            Plus_Mana =              0b00010111,
            Plus_Regen_Life =        0b00011000,
            Plus_Regen_Mana =        0b00011001,
            Plus_Str =               0b00011010,
            Plus_Dex =               0b00011011,
            Plus_Con =               0b00011100,
            Plus_Int =               0b00011101,
            Plus_Speed_Phys =        0b00011110,
            Plus_Speed_Magic =       0b00011111,
            Plus_Speed_Movement =    0b00100000,
            Plus_Block_Recovery =    0b00100001,
            Plus_Stagger_Recovery =  0b00100010,
            Plus_Magic_Find =        0b00100011,
            Prefix                 = 0b10000000,
            Suffix                 = 0b01000000
        }

        public decimal binary;

        //public int Id { get { return Convert.ToInt32( binary.Substring( 0, 8 ) ); } }
    }

    public class ItemEditor : EditorBase
    {
        //private SQLiteConnection _connection;

        private Vector2 scrollView;

        //private ItemState activeItem;
        //private TextAsset obj;
        //public List<ItemState.Implicit> Implicits = new List<ItemState.Implicit>();
        //public List<ItemState.Prefix> Prefixes = new List<ItemState.Prefix>();
        //public List<ItemState.Suffix> Suffixes = new List<ItemState.Suffix>();
        //public UnityEngine.Sprite SpriteUI;
        //public UnityEngine.AnimatorOverrideController animatorOverrideController;

        [MenuItem( "Window/Editor/Items" )]
        private static void ShowWindow() {
            GetWindow( typeof( ItemEditor ) );
        }

        private void Awake() {
            //so = new SerializedObject( this );
            //activeItem = new ItemState();
            //Implicits = new List<ItemState.Implicit>();
            //Prefixes = new List<ItemState.Prefix>();
            //Suffixes = new List<ItemState.Suffix>();
        }

        protected override void MainWindow() {
            scrollView = EditorGUILayout.BeginScrollView( scrollView, false, true, GUILayout.Width( position.width ) );
            {
                //obj = PaintXMLLookup( obj, "Resource File", true );
                //if( PaintButton( "Save" ) ) {
                //    Save();
                //}
                //PaintTextField( ref activeItem.Name, "Item Name" );
                //activeItem.ItemType = (ItemState.Type)PaintPopup( Helper.ItemTypeNames, (int)activeItem.ItemType, "Item Type" );
                //PaintSpriteField( ref SpriteUI );
                //animatorOverrideController = PaintAnimationOverrideControllerLookup( animatorOverrideController );
                //PaintIntField( ref activeItem.qlvl, "Quality Level" );
                //PaintIntField( ref activeItem.DmgMin, "Min Damage" );
                //PaintIntField( ref activeItem.DmgMax, "Max Damage" );
                //PaintIntField( ref activeItem.DefMin, "Min Defense" );
                //PaintIntField( ref activeItem.DefMax, "Max Defense" );
                //PaintIntField( ref activeItem.Blockrate, "Chance to Block" );
                //PaintIntField( ref activeItem.Durability, "Durability" );
                //PaintTextField( ref activeItem.Description, "Item Description" );
                //PaintIntSlider( ref activeItem.ReqStr, 0, 255, "Str Requirement" );
                //PaintIntSlider( ref activeItem.ReqDex, 0, 255, "Dex Requirement" );
                //PaintIntSlider( ref activeItem.ReqInt, 0, 255, "Int Requirement" );
                //PaintIntSlider( ref activeItem.ReqCons, 0, 255, "Con Requirement" );
                //PaintIntSlider( ref activeItem.ReqLvl, 0, 60, "Lvl Requirement" );
                //if( Checkbox( ref activeItem.Unique, "Unique" ) ) {
                //    PaintList<ItemState.Prefix>( "Prefixes" );
                //    PaintList<ItemState.Suffix>( "Suffixes" );
                //}
                //PaintList<ItemState.Implicit>( "Implicits" );
            }
            EditorGUILayout.EndScrollView();

            base.MainWindow();
        }

        protected override void ResetProperties() {
        }

        protected override void LoadProperties( TextAsset textAsset ) {
            //activeItem = XMLUtility.Load<ItemState>( textAsset );

            //Implicits = activeItem.Implicits;
            //Prefixes = activeItem.Prefixes;
            //Suffixes = activeItem.Suffixes;

            //SpriteUI = Resources.Load<Sprite>( activeItem.SpriteUIFilename );
        }

        protected override void CreationWindow() {
            base.CreationWindow();
        }

        private void Save() {
            //activeItem.Implicits = Implicits;
            //activeItem.Prefixes = Prefixes;
            //activeItem.Suffixes = Suffixes;

            //string filePath = string.Empty;

            //// UI Sprite
            //if( SpriteUI != null ) {
            //    filePath = AssetDatabase.GetAssetPath( SpriteUI ).Substring( S_RESOURCE_DIR_LENGTH );
            //    filePath = filePath.Substring( 0, filePath.Length - S_PNG_EXTENSION_LENGTH );
            //    activeItem.SpriteUIFilename = SpriteUI == null ? string.Empty : filePath;
            //}

            //// Animation
            //if( animatorOverrideController != null ) {
            //    filePath = AssetDatabase.GetAssetPath( animatorOverrideController ).Substring( S_RESOURCE_DIR_LENGTH );
            //    filePath = filePath.Substring( 0, filePath.Length - S_OVERRIDECONTROLLER_LENGTH );
            //    activeItem.animationName = animatorOverrideController == null ? string.Empty : filePath;
            //}

            //System.Text.StringBuilder t = new System.Text.StringBuilder(S_ITEMS_DIR);
            //t.Append( activeItem.ItemType );
            //t.Append( "/" );
            //t.Append( activeItem.qlvl );
            //t.Append( "/" );

            //XMLUtility.Save<ItemState>( activeItem, t.ToString(), activeItem.Name );
        }
    }
}