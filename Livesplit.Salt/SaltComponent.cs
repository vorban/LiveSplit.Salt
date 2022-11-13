using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace LiveSplit.Salt
{
    public class SaltComponent : IComponent
    {
        private static readonly string[] BossItemNames =
        {
            "dice_dread",
            "dice_cutqueen",
            "dice_bull", // This is Kraekan Cyclops
            "dice_alchemist",
            "dice_fauxjester",
            "dice_dragon",
            "dice_torturetree",
            "dice_pirate",
            "dice_inquisitor",
            "dice_hippogriff",
            "dice_ruinaxe",
            "dice_lakewitch",
            "dice_monster",
            "dice_squiddragon",
            "dice_nameless",
            "dice_cloak",
            "dice_gasbag",
            "dice_clay",
            "dice_mummy",
            "dice_butterfly",
            "dice_deadking",
            "dice_broken"
        };

        private readonly TimerModel _model;
        private readonly SaltMemory _mem;
        private readonly Settings _settings = new Settings();

        private readonly Dictionary<string, InvLoot> _bossItems = new Dictionary<string, InvLoot>();
        private readonly List<EnemyHealthTracker> _enemyTrackers = new List<EnemyHealthTracker>();

        private bool _playerRandomized;
        private bool _playerOfNewRunLoaded;
        private string _playerPreviousAnimation;

        public string ComponentName { get; }

        public SaltComponent(LiveSplitState state, string name)
        {
            ComponentName = name;
            _mem = new SaltMemory();

            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            _model = new TimerModel
            {
                CurrentState = state
            };

            _model.CurrentState.OnStart -= SetPlayerOfNewRunLoaded;
            _model.CurrentState.OnStart += SetPlayerOfNewRunLoaded;

            _model.CurrentState.OnStart -= TimerStart;
            _model.CurrentState.OnStart += TimerStart;

            _model.CurrentState.OnStart -= SetPlayerNeedsRandomized;
            _model.CurrentState.OnStart += SetPlayerNeedsRandomized;
            _model.CurrentState.OnSplit -= SetPlayerNeedsRandomized;
            _model.CurrentState.OnSplit += SetPlayerNeedsRandomized;
            _model.CurrentState.OnUndoSplit -= SetPlayerNeedsRandomized;
            _model.CurrentState.OnUndoSplit += SetPlayerNeedsRandomized;
            _model.CurrentState.OnSkipSplit -= SetPlayerNeedsRandomized;
            _model.CurrentState.OnSkipSplit += SetPlayerNeedsRandomized;
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            _mem.Hook();
            if (!_mem.IsHooked)
            {
                return;
            }

            if (_model.CurrentState.CurrentSplitIndex == -1)
            {
                CheckStart();
                return;
            }

            //! The GameState is not stored at the same address anymore.
            //! I now use a different approach to check for the loaded player.
            //if (_mem.GetGameState() != GameState.Playing)
            //{
            //    return;
            //}

            //! This is the new way of checking for the loaded player.
            if (!_playerOfNewRunLoaded)
            {
                CheckPlayerOfNewRunLoaded();
                return;
            }
            
            //! This is executed on run start + on each split
            if (_settings.RandomizeSkins && !_playerRandomized)
            {
                _mem.RandomizePlayerAppearance(0);
                _playerRandomized = true;
            }

            CheckItemSplits();

            // TODO: Generic enemy kill split system
            // Unspeakable Deep
            int len = _mem.GetCharCount();
            for (int i = 0; i < len; i++)
            {
                if (_mem.GetCharType(i) != EnemyType.Leviathon || _mem.GetCharHealth(i) <= 0f ||
                    _enemyTrackers.Any(tracker => tracker.CharType == EnemyType.Leviathon))
                {
                    continue;
                }

                _enemyTrackers.Add(new EnemyHealthTracker(_mem, i));
            }

            CheckCharKills();

            if (_model.CurrentState.CurrentSplitIndex == _model.CurrentState.Run.Count - 1 && _mem.IsGameEnding())
            {
                _model.Split();
            }
        }

        private void CheckStart()
        {
            if (_mem.GetMenuLevel(0) == MenuLevel.VentureForth &&
                _mem.GetMenuTransitionMode(0) == TransitionMode.AllOut)
            {
                _model.Start();
            }
        }

        private void CheckPlayerOfNewRunLoaded()
        {
            // When the game is launched, after the "do not shutdown when you see this icon" screen,
            // it loads a default player (with animation "fly"). If you load a NG+ save,
            // and then go back to the menu, your character is still loaded as the current player.
            // If you then start a new game (thus beginning a new run), the autosplitter would start the timer
            // because a fade to black transition happens, and then scan for the player's inventory (new boss item = split).
            // But during the fade to black, the new character isn't loaded yet, and with the latter example,
            // your NG+ character which is still in memory would trigger all the splits in 0.0s.

            // As of 2022-11-05, I don't have the knowledge nor the resources to figure out the memory address of the
            // game state (menu / loading / playing).
            // My solution is simple: When a character is loaded, its animation is going to change during gameplay
            // So, when we start a run, we set _playerOfNewRunLoaded to false and save the current animation.
            // Then, when we detect a new animation, we set the bool to true.
            // While the bool is false, split checks are disabled (both inventory, game ending, and boss kill).

            if (_mem.GetPlayerAnim(0) != _playerPreviousAnimation && !_playerOfNewRunLoaded)
            {
                _playerOfNewRunLoaded = true;
            }
        }

        private void CheckItemSplits()
        {
            Console.WriteLine("Checking items");
            // Boss items
            foreach (string itemName in BossItemNames)
            {
                // Skipping nameless for now, will add back in after I make config
                if (itemName == "dice_nameless")
                {
                    continue;
                }

                if (!_mem.TryGetPlayerItem(0, itemName, out InvLoot item))
                {
                    continue;
                }

                Console.WriteLine("- [" + itemName + "] found");

                if (_bossItems.TryGetValue(itemName, out InvLoot existingItem) && item.count <= existingItem.count)
                {
                    Console.WriteLine("\tit was in the inventory last frame");
                    continue;
                }

                _bossItems[itemName] = item;
                _model.Split();
            }
        }

        private void CheckCharKills()
        {
            foreach (EnemyHealthTracker tracker in _enemyTrackers)
            {
                tracker.Update();
                if (tracker.ShouldSplit)
                {
                    _model.Split();
                }
            }

            _enemyTrackers.RemoveAll(tracker => tracker.Done);
        }

        private void TimerStart(object sender, EventArgs e)
        {
            _bossItems.Clear();
            _enemyTrackers.Clear();
        }

        private void SetPlayerNeedsRandomized(object sender, EventArgs e)
        {
            _playerRandomized = false;
        }

        private void SetPlayerOfNewRunLoaded(object sender, EventArgs e)
        {
            _playerOfNewRunLoaded = false;
            _playerPreviousAnimation = _mem.GetPlayerAnim(0);
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            return _settings;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement xmlSettings = document.CreateElement("Settings");

            XmlElement rndSkins = document.CreateElement(nameof(Settings.RandomizeSkins));
            rndSkins.InnerText = _settings.RandomizeSkins.ToString();
            xmlSettings.AppendChild(rndSkins);

            return xmlSettings;
        }

        public void SetSettings(XmlNode settings)
        {
            XmlNode rndSkinsNode = settings.SelectSingleNode(".//" + nameof(Settings.RandomizeSkins));
            if (bool.TryParse(rndSkinsNode?.InnerText, out bool rndSkins))
            {
                _settings.RandomizeSkins = rndSkins;
            }
        }

        public void Dispose()
        {
            _model.CurrentState.OnStart -= TimerStart;

            _model.CurrentState.OnStart -= SetPlayerNeedsRandomized;
            _model.CurrentState.OnSplit -= SetPlayerNeedsRandomized;
            _model.CurrentState.OnUndoSplit -= SetPlayerNeedsRandomized;
            _model.CurrentState.OnSkipSplit -= SetPlayerNeedsRandomized;

            _mem.Dispose();
        }

        #region Unused interface stuff
        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
        }

        public float HorizontalWidth => 0;
        public float MinimumHeight => 0;
        public float VerticalHeight => 0;
        public float MinimumWidth => 0;
        public float PaddingTop => 0;
        public float PaddingBottom => 0;
        public float PaddingLeft => 0;
        public float PaddingRight => 0;
        public IDictionary<string, Action> ContextMenuControls => null;
#endregion
    }
}
