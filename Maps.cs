using System;
using UnityEngine;
using CustomMapUtility;
using BigDLL4221.Extensions;

namespace TheHead
{
    public class EnemyTeamStageManager_TheHead : EnemyTeamStageManager
    {
        private int _wave3RoundCount;

        public override void OnWaveStart()
        {
            _cmu.InitCustomMap<MapManager_TheHead>("TheHead");

            foreach (BattleUnitModel friendly in BattleObjectManager.instance.GetAliveList(Faction.Player))
            {
                friendly.SetKnockoutInsteadOfDeath(true);
            }

            foreach (BattleUnitModel enemy in BattleObjectManager.instance.GetAliveList(Faction.Enemy))
            {
                enemy.SetKnockoutInsteadOfDeath(true);
            }
        }

        public override void OnRoundStart()
        {
            _cmu.EnforceMap();

            if (_controller.CurrentWave >= 3)
            {
                _wave3RoundCount++;

                if (CanEndForcefully())
                {
                    EndBattle();
                }
            }
        }

        public void EndBattle()
        {
            StorySerializer.LoadEffectFile(1000, 5, 4);
            SingletonBehavior<BattleManagerUI>.Instance.ui_battleStory.OpenStory(delegate
            {
                EndingStory(End);
            }, false, true);
        }

        private FinalEpisode_FinalEnd EndingStory(Action callback)
        {
            GameObject gameObject = Util.LoadPrefab("Battle/FinalEpisode/FinalEpisode_FinalEnd");
            FinalEpisode_FinalEnd component = gameObject.GetOrAddComponent<FinalEpisode_FinalEnd>();
            component.Init(callback, new EnemyTeamStageManager_FinalFinal());
            SingletonBehavior<BattleDirectingManager>.Instance.Add(gameObject);
            return component;
        }

        private void End()
        {
            var faction = Faction.Player;
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetAliveList(faction))
            {
                _controller.RemoveUnit(faction, battleUnitModel.index);
            }

            _controller.CheckEndBattle();
        }

        public override void OnRoundEndTheLast()
        {
            if (_controller.CurrentWave >= 3 && CheckLose())
            {
                EndBattle();
            }

            base.OnRoundEndTheLast();
        }

        public override bool CanExitRoundEndPhase()
        {
            if (_controller.CurrentWave >= 3 && CheckLose())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// If we dont have units left, or round count is high enough, end the battle.
        /// We check 11, because 10 is another mass attack.
        /// </summary>
        /// <returns></returns>
        private bool CanEndForcefully()
        {
            return _wave3RoundCount == 11 || CheckLose();
        }

        private bool CheckLose()
        {
            return BattleObjectManager.instance.GetAliveList(Faction.Player).Count == 0;
        }

        private StageController _controller => Singleton<StageController>.Instance;
        private CustomMapHandler _cmu = CustomMapHandler.GetCMU(Parameters.PackageId);
    }

    public class MapManager_TheHead : CustomMapManager
    {
        protected override string[] CustomBGMs
        {
            get
            {
                return new string[]
                {
                    "HeadFight.ogg"
                };
            }
        }
    }
}
