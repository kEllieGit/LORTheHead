using BigDLL4221.Utils;
using LOR_DiceSystem;

namespace TheHead
{
    public class PassiveAbility_ArbiterShimmer : PassiveAbilityBase
    {
        private int _pattern;

        public override void OnWaveStart()
        {
            _pattern = 0;
        }

        public override void OnRoundStart()
        {
            base.OnRoundStart();

            owner.allyCardDetail.ExhaustAllCards();
            owner.cardSlotDetail.RecoverPlayPoint(owner.MaxPlayPoint);
            AddNewCard(new LorId(Parameters.PackageId, 379320028), 0);
            AddNewCard(new LorId(Parameters.PackageId, 379320028), 0);
            AddNewCard(new LorId(Parameters.PackageId, 379320030), 0);
            AddNewCard(new LorId(Parameters.PackageId, 379320030), 0);
            AddNewCard(new LorId(Parameters.PackageId, 379320029), 0);
            AddNewCard(new LorId(Parameters.PackageId, 379320031), 0);
            AddNewCard(new LorId(Parameters.PackageId, 379320033), 0);

            if (!owner.breakDetail.IsBreakLifeZero())
            {
                _pattern++;

                if (_pattern >= 5)
                {
                    AddNewCard(new LorId(Parameters.PackageId, 379320032), 25, true);
                    _pattern = 0;
                }
            }
        }

        private void AddNewCard(LorId id, int priorityAdder, bool zeroCost = false)
        {
            BattleDiceCardModel battleDiceCardModel = owner.allyCardDetail?.AddTempCard(id);

            if (battleDiceCardModel != null)
            {
                battleDiceCardModel.SetPriorityAdder(priorityAdder);

                if (zeroCost)
                    battleDiceCardModel.SetCostToZero();

                battleDiceCardModel.temporary = true;
            }
        }
    }

    public class PassiveAbility_ClawShimmer : PassiveAbilityBase
    {
        private int _state;

        public override void OnWaveStart()
        {
            _state = 0;
        }

        public override void OnRoundStart()
        {
            base.OnRoundStart();

            owner.allyCardDetail.ExhaustAllCards();
            owner.cardSlotDetail.RecoverPlayPoint(owner.MaxPlayPoint);
            AddNewCard(new LorId(Parameters.PackageId, 379320015), 0);
            AddNewCard(new LorId(Parameters.PackageId, 379320016), 0);
            AddNewCard(new LorId(Parameters.PackageId, 379320020), 0);
            AddNewCard(new LorId(Parameters.PackageId, 379320017), 0);
            AddNewCard(new LorId(Parameters.PackageId, 379320019), 0);

            if (_state >= 3)
            {
                AddNewCard(new LorId(Parameters.PackageId, 379320018), 10);
                _state = 0;
            }
            else
            {
                _state++;
            }
        }

        private void AddNewCard(LorId id, int priorityAdder, bool zeroCost = false)
        {
            BattleDiceCardModel battleDiceCardModel = owner.allyCardDetail?.AddTempCard(id);

            if (battleDiceCardModel != null)
            {
                battleDiceCardModel.SetPriorityAdder(priorityAdder);

                if (zeroCost)
                    battleDiceCardModel.SetCostToZero();

                battleDiceCardModel.temporary = true;
            }
        }
    }

    /// <summary>
    /// "An Arbiter" passive
    /// </summary>
    public class PassiveAbility_AnArbiter : PassiveAbilityBase
    {
        public override int SpeedDiceNumAdder()
        {
            return 2;
        }

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (GetRange(attacker.currentDiceAction.card) == CardRange.Near && !owner.breakDetail.IsBreakLifeZero())
                return true;

            return base.BeforeTakeDamage(attacker, dmg);
        }

        public CardRange GetRange(BattleDiceCardModel card)
        {
            return card.GetSpec().Ranged;
        }
    }

    /// <summary>
    /// Librarian Version
    /// </summary>
    public class PassiveAbility_AnArbiter_Nerfed : PassiveAbility_AnArbiter
    {
        public override int SpeedDiceNumAdder()
        {
            return 0;
        }

        public override void OnWaveStart()
        {
            UnitUtil.DrawUntilX(owner, 5);
        }

        public override void OnRoundStart()
        {
        }

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            return GetRange(attacker.currentDiceAction.card) == CardRange.Near && !owner.breakDetail.IsBreakLifeZero();
        }
    }

    /// <summary>
    /// "Executioner" passive
    /// </summary>
    public class PassiveAbility_Executioner : PassiveAbilityBase
    {
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            if (IsLowHP(behavior.card.target))
            {
                behavior.card?.target?.Die(owner);
            }
        }

        private bool IsLowHP(BattleUnitModel model)
        {
            return model.hp / model.MaxHp < 0.35f;
        }
    }

    /// <summary>
    /// Librarian version
    /// </summary>
    public class PassiveAbility_Executioner_Nerfed : PassiveAbilityBase
    {
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            if (IsLowHP(behavior.card.target) && behavior.card.target.breakDetail.IsBreakLifeZero())
            {
                behavior.card?.target?.Die(owner);
            }
        }

        private bool IsLowHP(BattleUnitModel model)
        {
            return model.hp / model.MaxHp < 0.25f;
        }
    }

    /// <summary>
    /// "A Claw" passive
    /// </summary>
    public class PassiveAbility_AClaw : PassiveAbilityBase
    {
        public virtual int SpeedExtraDiceNum => SpeedDiceNumAdder() + 1;

        public virtual States CurrentState
        {
            get => _currentState;
            set
            {
                PreviousState = _currentState;
                _currentState = value;

                // Take 25% damage from MaxHp.
                int damage = (int)(0.25 * owner.MaxHp);
                owner.LoseHp(damage);
            }
        }

        public States PreviousState { get; private set; }

        private int _roundsSinceStateSwitch;

        private States _currentState;

        public override int SpeedDiceNumAdder()
        {
            return 1;
        }

        public override void OnWaveStart()
        {
            _roundsSinceStateSwitch = 0;
            CurrentState = States.Strength;
        }

        public override void OnRoundStart()
        {
            if (!owner.breakDetail.IsBreakLifeZero())
            {
                _roundsSinceStateSwitch++;
                GetCurrentStateBuffs();

                // When we reach the final state, indicated by the End enum.
                if (_roundsSinceStateSwitch == (int)States.End)
                {
                    int nextState = (int)_currentState + 1;

                    if (nextState == 3)
                    {
                        nextState = 0;
                    }

                    CurrentState = (States)nextState;
                    _roundsSinceStateSwitch = 0;
                }
            }
        }

        public virtual void GetCurrentStateBuffs()
        {
            switch (CurrentState)
            {
                case States.Strength:
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, 3);
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.DmgUp, 3);
                    break;
                case States.Speed:
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Quickness, 3);
                    owner.Book.SetSpeedDiceNum(SpeedExtraDiceNum);
                    break;
                case States.Defense:
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Protection, 3);
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.BreakProtection, 3);
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Endurance, 3);
                    owner.Book.SetSpeedDiceNum(SpeedDiceNumAdder());
                    break;
                default:
                    break;
            }
        }

        public enum States
        {
            Strength,
            Speed,
            Defense,
            End
        }
    }

    /// <summary>
    /// Librarian version
    /// </summary>
    public class PassiveAbility_AClaw_Nerfed : PassiveAbility_AClaw
    {
        public override int SpeedDiceNumAdder()
        {
            return 0;
        }

        public override void GetCurrentStateBuffs()
        {
            switch (CurrentState)
            {
                case States.Strength:
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, 3);
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.DmgUp, 3);
                    break;
                case States.Speed:
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Quickness, 3);
                    owner.Book.SetSpeedDiceNum(SpeedExtraDiceNum);
                    break;
                case States.Defense:
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Protection, 3);
                    owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.BreakProtection, 3);
                    owner.Book.SetSpeedDiceNum(SpeedDiceNumAdder());
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// "Counter" passive
    /// </summary>
    public class PassiveAbility_ClawBuff : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            owner.bufListDetail.AddBuf(new BattleUnitBuf_clawCounter());
            owner.SetKnockoutInsteadOfDeath(true);
        }
    }

    /// <summary>
    ///	"Suppressant Singularity" passive
    /// </summary>
    public class PassiveAbility_HeadImmunity : PassiveAbilityBase
    {
        public override bool IsImmune(KeywordBuf buf)
        {
            return buf == KeywordBuf.Stun;
        }
    }

    /// <summary>
    /// "Paladin Armor" passive
    /// </summary>
    public class PassiveAbility_PaladinArmor : PassiveAbilityBase
    {
        private bool _active;

        public override void OnRoundStart()
        {
            _active = !owner.breakDetail.IsBreakLifeZero();
            base.OnRoundStart();
        }

        public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
        {
            if (_active)
            {
                return AtkResist.Endure;
            }
            return base.GetResistHP(origin, detail);
        }
    }

    /// <summary>
    /// "Infused Blade" passive
    /// </summary>
    public class PassiveAbility_InfusedBlade : PassiveAbilityBase
    {
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            if (behavior.card.target.bufListDetail.HasBuf<BattleUnitBuf_fairy>() && IsMelee(behavior.card.card))
            {
                behavior.card.target?.bufListDetail.AddKeywordBufByCard(KeywordBuf.Fairy, 1, owner);
            }
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.card.target.bufListDetail.HasBuf<BattleUnitBuf_fairy>())
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus()
                {
                    power = 1
                });
            }
            else
            {
                behavior.ApplyDiceStatBonus(new DiceStatBonus()
                {
                    power = -1
                });
            }
            owner.battleCardResultLog?.SetPassiveAbility(this);
        }

        public bool IsMelee(BattleDiceCardModel card)
        {
            return card.GetSpec().Ranged == CardRange.Near;
        }
    }

    /// <summary>
    /// "A Paladin's Strength" passive
    /// </summary>
    public class PassiveAbility_Paladin : PassiveAbility_250322
    {
        // Literally the same as Geb's Passive but untransferrable.
    }
}
