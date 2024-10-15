using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Client.Core;
using Intersect.Client.Framework.File_Management;
using Intersect.Client.Framework.Gwen;
using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Framework.Gwen.Control.EventArguments;
using Intersect.Client.General;
using Intersect.Client.Localization;
using Intersect.Client.Networking;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network.Packets.Server;

namespace Intersect.Client.Interface.Game.Character
{

    public partial class CharacterWindow
    {

        //Equipment List
        public List<EquipmentItem> Items = new List<EquipmentItem>();

        Label mIntellingenceLabel;

        Button mAddIntellingenceBtn;

        Button mAddAttackBtn;

        Button mAddDefenseBtn;

        Button mAddVitalityBtn;

        Button mAddSpeedBtn;
        Label mMagicLabel;
        Button mAddMagicBtn;

        Label mDexterityLabel;
        Button mAddDexterityBtn;

        Label mCuresLabel;
        Label mPotencyLabel;
        Label mDamageLabel;
        Label mEvasionLabel;
        //Stats
        Label mAttackLabel;

        private ImagePanel mCharacterContainer;

        private Label mCharacterLevelAndClass;

        private Label mCharacterName;

        private ImagePanel mCharacterPortrait;

        private string mCharacterPortraitImg = "";

        //Controls
        private WindowControl mCharacterWindow;

        private string mCurrentSprite = "";

        Label mDefenseLabel;

        private ItemProperties mItemProperties = null;

        Label mVitalityLabel;

        Label mPointsLabel;

        Label mSpeedLabel;

        public ImagePanel[] PaperdollPanels;

        public string[] PaperdollTextures;

        //Location
        public int X;

        public int Y;

        //Extra Buffs
        ClassBase mPlayer;

        Label mHpRegen;

        int HpRegenAmount;

        Label mManaRegen;

        int ManaRegenAmount;

        Label mLifeSteal;

        int LifeStealAmount = 0;

        Label mAttackSpeed;

        Label mExtraExp;

        int ExtraExpAmount = 0;

        Label mLuck;

        int LuckAmount = 0;

        Label mTenacity;

        int TenacityAmount = 0;

        Label mCooldownReduction;

        int CooldownAmount = 0;

        Label mManaSteal;
       
        int ManaStealAmount = 0;

        public float Damage { get; set; }

        //Init
        public CharacterWindow(Canvas gameCanvas)
        {
            mCharacterWindow = new WindowControl(gameCanvas, Strings.Character.title, false, "CharacterWindow");
            mCharacterWindow.DisableResizing();

            mCharacterName = new Label(mCharacterWindow, "CharacterNameLabel");
            mCharacterName.SetTextColor(Color.White, Label.ControlState.Normal);

            mCharacterLevelAndClass = new Label(mCharacterWindow, "ChatacterInfoLabel");
            mCharacterLevelAndClass.SetText("");

            mCharacterContainer = new ImagePanel(mCharacterWindow, "CharacterContainer");

            mCharacterPortrait = new ImagePanel(mCharacterContainer);

            PaperdollPanels = new ImagePanel[Options.EquipmentSlots.Count + 1];
            PaperdollTextures = new string[Options.EquipmentSlots.Count + 1];
            for (var i = 0; i <= Options.EquipmentSlots.Count; i++)
            {
                PaperdollPanels[i] = new ImagePanel(mCharacterContainer);
                PaperdollTextures[i] = "";
                PaperdollPanels[i].Hide();
            }

            var equipmentLabel = new Label(mCharacterWindow, "EquipmentLabel");
            equipmentLabel.SetText(Strings.Character.equipment);

            var statsLabel = new Label(mCharacterWindow, "StatsLabel");
            statsLabel.SetText(Strings.Character.stats);

            mAttackLabel = new Label(mCharacterWindow, "AttackLabel");

            mAddAttackBtn = new Button(mCharacterWindow, "IncreaseAttackButton");
            mAddAttackBtn.Clicked += _addAttackBtn_Clicked;

            mDefenseLabel = new Label(mCharacterWindow, "DefenseLabel");
           
            mSpeedLabel = new Label(mCharacterWindow, "SpeedLabel");
         

            mIntellingenceLabel = new Label(mCharacterWindow, "AbilityPowerLabel");
            mAddIntellingenceBtn = new Button(mCharacterWindow, "IncreaseAbilityPowerButton");
            mAddIntellingenceBtn.Clicked += _addAbilityPwrBtn_Clicked;

            mVitalityLabel = new Label(mCharacterWindow, "VitalityLabel");
            mAddVitalityBtn = new Button(mCharacterWindow, "IncreaseVitalityButton");
            mAddVitalityBtn.Clicked += _addMagicResistBtn_Clicked;
            // Initialize Agility Label and Button
            mMagicLabel = new Label(mCharacterWindow, "MagicLabel");
            mAddMagicBtn = new Button(mCharacterWindow, "IncreaseMagicButton");
            mAddMagicBtn.Clicked += _addMagicBtn_Clicked;
            mDamageLabel = new Label(mCharacterWindow, "DamageLabel");
            // Initialize Dexterity Label and Button
            mDexterityLabel = new Label(mCharacterWindow, "DexterityLabel");
            mAddDexterityBtn = new Button(mCharacterWindow, "IncreaseDexterityButton");
            mAddDexterityBtn.Clicked += _addDexterityBtn_Clicked;

            // Initialize Cures Label and Button
            mCuresLabel = new Label(mCharacterWindow, "CuresLabel");
           
            // Initialize Potency Label and Button
            mPotencyLabel = new Label(mCharacterWindow, "PotencyLabel");
         
            mPointsLabel = new Label(mCharacterWindow, "PointsLabel");

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                Items.Add(new EquipmentItem(i, mCharacterWindow));
                Items[i].Pnl = new ImagePanel(mCharacterWindow, "EquipmentItem" + i);
                Items[i].Setup();
            }

            var extraBuffsLabel = new Label(mCharacterWindow, "ExtraBuffsLabel");
            extraBuffsLabel.SetText(Strings.Character.ExtraBuffs);

            mHpRegen = new Label(mCharacterWindow, "HpRegen");
            mManaRegen = new Label(mCharacterWindow, "ManaRegen");
            mLifeSteal = new Label(mCharacterWindow, "Lifesteal");
            mAttackSpeed = new Label(mCharacterWindow, "AttackSpeed");
            mExtraExp = new Label(mCharacterWindow, "ExtraExp");
            mLuck = new Label(mCharacterWindow, "Luck");
            mTenacity = new Label(mCharacterWindow, "Tenacity");
            mCooldownReduction = new Label(mCharacterWindow, "CooldownReduction");
            mManaSteal = new Label(mCharacterWindow, "Manasteal");
            mEvasionLabel = new Label(mCharacterWindow, "Evasion");
            mCharacterWindow.LoadJsonUi(GameContentManager.UI.InGame, Graphics.Renderer.GetResolutionString());
        }

        private void _addDexterityBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int)Stat.Dexterity);
        }

        private void _addMagicBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int)Stat.Magic);
        }

        //Update Button Event Handlers
        void _addMagicResistBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int) Stat.Vitality);
        }

        void _addAbilityPwrBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int) Stat.Intelligence);
        }

       /* void _addSpeedBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int) Stat.Speed);
        }

        void _addDefenseBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int) Stat.Defense);
        }
       */
        void _addAttackBtn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUpgradeStat((int) Stat.Attack);
        }

        //Methods
        public void Update()
        {
            if (mCharacterWindow.IsHidden)
            {
                return;
            }

            mCharacterName.Text = Globals.Me.Name;
            mCharacterLevelAndClass.Text = Strings.Character.levelandclass.ToString(
                Globals.Me.Level, ClassBase.GetName(Globals.Me.Class)
            );

            //Load Portrait
            //UNCOMMENT THIS LINE IF YOU'D RATHER HAVE A FACE HERE GameTexture faceTex = Globals.ContentManager.GetTexture(Framework.Content.TextureType.Face, Globals.Me.Face);
            var entityTex = Globals.ContentManager.GetTexture(
                Framework.Content.TextureType.Entity, Globals.Me.Sprite
            );

            /* UNCOMMENT THIS BLOCK IF YOU"D RATHER HAVE A FACE HERE if (Globals.Me.Face != "" && Globals.Me.Face != _currentSprite && faceTex != null)
             {
                 _characterPortrait.Texture = faceTex;
                 _characterPortrait.SetTextureRect(0, 0, faceTex.GetWidth(), faceTex.GetHeight());
                 _characterPortrait.SizeToContents();
                 Align.Center(_characterPortrait);
                 _characterPortrait.IsHidden = false;
                 for (int i = 0; i < Options.EquipmentSlots.Count; i++)
                 {
                     _paperdollPanels[i].Hide();
                 }
             }
             else */
            if (!string.IsNullOrWhiteSpace(Globals.Me.Sprite) && Globals.Me.Sprite != mCurrentSprite && entityTex != null)
            {
                for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
                    var paperdoll = "";
                    if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z]) > -1)
                    {
                        var equipment = Globals.Me.MyEquipment;
                        if (equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] > -1 &&
                            equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] <
                            Options.MaxInvItems)
                        {
                            var itemNum = Globals.Me
                                .Inventory[equipment[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])]]
                                .ItemId;

                            if (ItemBase.TryGet(itemNum, out var itemDescriptor))
                            {
                                paperdoll = Globals.Me.Gender == 0
                                    ? itemDescriptor.MalePaperdoll : itemDescriptor.FemalePaperdoll;
                                PaperdollPanels[z].RenderColor = itemDescriptor.Color;
                            }
                        }
                    }
                    else if (Options.PaperdollOrder[1][z] == "Player")
                    {
                        PaperdollPanels[z].Show();
                        PaperdollPanels[z].Texture = entityTex;
                        PaperdollPanels[z].SetTextureRect(0, 0, entityTex.GetWidth() / Options.Instance.Sprites.NormalFrames, entityTex.GetHeight() / Options.Instance.Sprites.Directions);
                        PaperdollPanels[z].SizeToContents();
                        PaperdollPanels[z].RenderColor = Globals.Me.Color;
                        Align.Center(PaperdollPanels[z]);
                    }

                    if (string.IsNullOrWhiteSpace(paperdoll) && !string.IsNullOrWhiteSpace(PaperdollTextures[z]) && Options.PaperdollOrder[1][z] != "Player")
                    {
                        PaperdollPanels[z].Texture = null;
                        PaperdollPanels[z].Hide();
                        PaperdollTextures[z] = "";
                    }
                    else if (paperdoll != "" && paperdoll != PaperdollTextures[z])
                    {
                        var paperdollTex = Globals.ContentManager.GetTexture(
                            Framework.Content.TextureType.Paperdoll, paperdoll
                        );

                        PaperdollPanels[z].Texture = paperdollTex;
                        if (paperdollTex != null)
                        {
                            PaperdollPanels[z]
                                .SetTextureRect(
                                    0, 0, PaperdollPanels[z].Texture.GetWidth() / Options.Instance.Sprites.NormalFrames,
                                    PaperdollPanels[z].Texture.GetHeight() / Options.Instance.Sprites.Directions
                                );

                            PaperdollPanels[z]
                                .SetSize(
                                    PaperdollPanels[z].Texture.GetWidth() / Options.Instance.Sprites.NormalFrames,
                                    PaperdollPanels[z].Texture.GetHeight() / Options.Instance.Sprites.Directions
                                );

                            PaperdollPanels[z]
                                .SetPosition(
                                    mCharacterContainer.Width / 2 - PaperdollPanels[z].Width / 2,
                                    mCharacterContainer.Height / 2 - PaperdollPanels[z].Height / 2
                                );
                        }

                        PaperdollPanels[z].Show();
                        PaperdollTextures[z] = paperdoll;
                    }
                }
            }
            else if (Globals.Me.Sprite != mCurrentSprite && Globals.Me.Face != mCurrentSprite)
            {
                mCharacterPortrait.IsHidden = true;
                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    PaperdollPanels[i].Hide();
                }
            }

            mAttackLabel.SetText(
     Strings.Character.stat0.ToString(Strings.Combat.stat0, Globals.Me.Stat[(int)Stat.Attack])
 );

            mDefenseLabel.SetText(
                Strings.Character.stat2.ToString(Strings.Combat.stat2, Globals.Me.Stat[(int)Stat.Defense])
            );

            mSpeedLabel.SetText(
                Strings.Character.stat4.ToString(Strings.Combat.stat4, Globals.Me.Stat[(int)Stat.Speed])
            );

            mIntellingenceLabel.SetText(
                Strings.Character.stat1.ToString(Strings.Combat.stat1, Globals.Me.Stat[(int)Stat.Intelligence])
            );

            mVitalityLabel.SetText(
                Strings.Character.stat3.ToString(Strings.Combat.stat3, Globals.Me.Stat[(int)Stat.Vitality])
            );

            // New stats added below:

            mMagicLabel.SetText(
                Strings.Character.stat5.ToString(Strings.Combat.stat5, Globals.Me.Stat[(int)Stat.Magic])
            );

            mDexterityLabel.SetText(
                Strings.Character.stat6.ToString(Strings.Combat.stat6, Globals.Me.Stat[(int)Stat.Dexterity])
            );

            mPotencyLabel.SetText(
                Strings.Character.stat7.ToString(Strings.Combat.stat7, Globals.Me.Stat[(int)Stat.Potency])
            );

            mCuresLabel.SetText(
                Strings.Character.stat8.ToString(Strings.Combat.stat8, Globals.Me.Stat[(int)Stat.Cures])
            );
            var MinDamage = Damage * .975;
            var MaxDamage = Damage * 1.025;
            mDamageLabel.SetText(Strings.Character.Damage.ToString(MinDamage,MaxDamage));
            mPointsLabel.SetText(Strings.Character.points.ToString(Globals.Me.StatPoints));
            mAddIntellingenceBtn.IsHidden = Globals.Me.StatPoints == 0 ||
                                         Globals.Me.Stat[(int) Stat.Intelligence] == Options.MaxStatValue;

            mAddAttackBtn.IsHidden =
                Globals.Me.StatPoints == 0 || Globals.Me.Stat[(int) Stat.Attack] == Options.MaxStatValue;

           
            mAddVitalityBtn.IsHidden = Globals.Me.StatPoints == 0 ||
                                          Globals.Me.Stat[(int) Stat.Vitality] == Options.MaxStatValue;

           UpdateExtraBuffs();

            for (var i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (Globals.Me.MyEquipment[i] > -1 && Globals.Me.MyEquipment[i] < Options.MaxInvItems)
                {
                    if (Globals.Me.Inventory[Globals.Me.MyEquipment[i]].ItemId != Guid.Empty)
                    {
                        Items[i]
                            .Update(
                                Globals.Me.Inventory[Globals.Me.MyEquipment[i]].ItemId,
                                Globals.Me.Inventory[Globals.Me.MyEquipment[i]].ItemProperties
                            );

                        UpdateExtraBuffs(Globals.Me.Inventory[Globals.Me.MyEquipment[i]].ItemId);
                    }
                    else
                    {
                        Items[i].Update(Guid.Empty, mItemProperties);
                    }
                }
                else
                {
                    Items[i].Update(Guid.Empty, mItemProperties);
                }
            }
        }

        /// <summary>
        /// Update Extra Buffs Effects like hp/mana regen and items effect types
        /// </summary>
        public void UpdateExtraBuffs()
        {
            mPlayer = ClassBase.Get(Globals.Me?.Class ?? Guid.Empty);

            //Getting HP and Mana Regen
            if (mPlayer != null)
            {
                HpRegenAmount = mPlayer.VitalRegen[0];
                mHpRegen.SetText(Strings.Character.HealthRegen.ToString(HpRegenAmount));
                ManaRegenAmount = mPlayer.VitalRegen[1];
                mManaRegen.SetText(Strings.Character.ManaRegen.ToString(ManaRegenAmount));
            }

            CooldownAmount = 0;
            LifeStealAmount = 0;
            TenacityAmount = 0;
            LuckAmount = 0;
            ExtraExpAmount = 0;
            ManaStealAmount = 0;

            mLifeSteal.SetText(Strings.Character.Lifesteal.ToString(0));
            mExtraExp.SetText(Strings.Character.ExtraExp.ToString(0));
            mLuck.SetText(Strings.Character.Luck.ToString(0));
            mTenacity.SetText(Strings.Character.Tenacity.ToString(0));
            mCooldownReduction.SetText(Strings.Character.CooldownReduction.ToString(0));
            mManaSteal.SetText(Strings.Character.Manasteal.ToString(0));

            mAttackSpeed.SetText(Strings.Character.AttackSpeed.ToString(Globals.Me.CalculateAttackTime() / 1000f));
            CalculateDamage(mPlayer);
        }

        public void CalculateDamage(ClassBase classBase)
        {          
            // Daño base del arma equipada
            int weaponDamage = GetWeaponDamage();

            // Daño base del ataque
            float baseDamage = classBase.Damage + weaponDamage;

            // Escalamiento como porcentaje (se puede ajustar)
            float scaling = 100f;                      

            // Obtener el daño escalado por las estadísticas de la clase
            float scalingStatValue = Globals.Me.Stat[(int)Stat.Attack]; // Escalamiento basado en la inteligencia, por ejemplo
            float scaleFactor = scaling / 100f;

            // Cálculo del daño total
            Damage = (baseDamage + (scalingStatValue * scaleFactor));

        }

        private int GetWeaponDamage()
        {
            return 0;
        }

        /// <summary>
        /// Update Extra Buffs Effects like hp/mana regen and items effect types
        /// </summary>
        /// <param name="itemId">Id of item to update extra buffs</param>
        public void UpdateExtraBuffs(Guid itemId)
        {
            var item = ItemBase.Get(itemId);

            if (item == null)
            {
                return;
            }

            //Getting HP and Mana Regen
            if (item.VitalsRegen[0] != 0)
            {
                HpRegenAmount += item.VitalsRegen[0];
                mHpRegen?.SetText(Strings.Character.HealthRegen.ToString(HpRegenAmount));
            }

            if (item.VitalsRegen[1] != 0)
            {
                ManaRegenAmount += item.VitalsRegen[1];
                mManaRegen?.SetText(Strings.Character.ManaRegen.ToString(ManaRegenAmount));
            }

            //Getting extra buffs
            if (item.Effects.Find(effect => effect.Type != ItemEffect.None && effect.Percentage > 0) != default)
            {
                foreach(var effect in item.Effects)
                {
                    if (effect.Percentage <= 0)
                    {
                        continue;
                    }

                    switch (effect.Type)
                    {
                        case ItemEffect.CooldownReduction:
                            CooldownAmount += effect.Percentage;
                            mCooldownReduction?.SetText(Strings.Character.CooldownReduction.ToString(CooldownAmount));

                            break;
                        case ItemEffect.Lifesteal:
                            LifeStealAmount += effect.Percentage;
                            mLifeSteal?.SetText(Strings.Character.Lifesteal.ToString(LifeStealAmount));

                            break;
                        case ItemEffect.Tenacity:
                            TenacityAmount += effect.Percentage;
                            mTenacity?.SetText(Strings.Character.Tenacity.ToString(TenacityAmount));

                            break;
                        case ItemEffect.Luck:
                            LuckAmount += effect.Percentage;
                            mLuck?.SetText(Strings.Character.Luck.ToString(LuckAmount));

                            break;
                        case ItemEffect.EXP:
                            ExtraExpAmount += effect.Percentage;
                            mExtraExp?.SetText(Strings.Character.ExtraExp.ToString(ExtraExpAmount));

                            break;
                        case ItemEffect.Manasteal:
                            ManaStealAmount += effect.Percentage;
                            mManaSteal?.SetText(Strings.Character.Manasteal.ToString(ManaStealAmount));

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Show the window
        /// </summary>
        public void Show()
        {
            mCharacterWindow.IsHidden = false;
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns if window is visible</returns>
        public bool IsVisible()
        {
            return !mCharacterWindow.IsHidden;
        }

        /// <summary>
        /// Hide the window
        /// </summary>
        public void Hide()
        {
            mCharacterWindow.IsHidden = true;
        }

    }

}
