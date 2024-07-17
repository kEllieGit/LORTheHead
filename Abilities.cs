namespace TheHead
{
	#region Card Abilities

	public class DiceCardSelfAbility_Indiscriminate : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "Indiscriminate_Keyword" };

		public static string Desc = "[Indiscriminate]";

        public override bool IsTargetableAllUnit()
        {
			return true;
        }
    }

	public class DiceCardSelfAbility_FairyDraw : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "Fairy_Keyword", "Energy_Keyword" };

		public static string Desc = "[On Use] Draw 1 page. If target has Fairy, Draw 2 pages instead";

		public override void OnUseCard()
		{
			if (card.target.bufListDetail.HasBuf<BattleUnitBuf_fairy>())
            {
                owner?.allyCardDetail.DrawCards(2);
            }
			else
            {
                owner?.allyCardDetail.DrawCards(1);
            }
		}
	}

    #endregion

    #region Dice Abilities

    public class DiceCardAbility_ClashPunishFairy_1 : DiceCardAbilityBase
	{
		public override string[] Keywords => new string[] { "Fairy_Keyword" };

		public static string Desc = "[On Clash Lose] Inflict 1 Fairy to self";

		public override void OnLoseParrying()
		{
			owner?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Fairy, 1, owner);
		}
	}

	public class DiceCardAbility_ClashPunishFairy_2 : DiceCardAbilityBase
	{
		public override string[] Keywords => new string[] { "Fairy_Keyword" };

		public static string Desc = "[On Clash Lose] Inflict 2 Fairy to self";

		public override void OnLoseParrying()
		{
			owner?.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Fairy, 2, owner);
		}
	}

	public class DiceCardAbility_IncreaseCost_1 : DiceCardAbilityBase
	{
		public static string Desc = "[On Hit] Increase Cost of all of target's pages by 1 for the next Scene";

		public class BattleUnitBuf_costUp1 : BattleUnitBuf
		{
			public override int GetCardCostAdder(BattleDiceCardModel card)
			{
				return 1;
			}

			public override void OnRoundEnd()
			{
				Destroy();
			}
		}

		public override void OnSucceedAttack(BattleUnitModel target)
		{
			target?.bufListDetail.AddReadyBuf(new BattleUnitBuf_costUp1());
		}
	}

	#endregion
}
