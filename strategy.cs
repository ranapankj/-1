using System.Collections.Generic;
using System.Windows.Forms;

public class Strategy      //如果编译多个策略请随机修改此类名，以免策略冲突
{
    //两个简单的例子,剩下的等大佬来解决。
    //最好把项目文件夹放到 .\Hearthstone\BepInEx\ 目录，这样不用再次指定引用路径。
    //编译之后把Dll放到插件同一目录即可
    public void Nomination()          //登场处理
    {
        //MessageBox.Show("登场处理");
        ZoneHand zoneHand = ZoneMgr.Get().FindZoneOfType<ZoneHand>(global::Player.Side.FRIENDLY);
        if (zoneHand != null)
        {
            List<string> fire;
            fire = new List<string>(new string[] { "米尔豪斯·法力风暴", "瓦尔登·晨拥", "萨尔", "凯恩·血蹄", "迪亚波罗", "安度因·乌瑞恩" });

            foreach (string name in fire)
            {
                foreach (Card card in zoneHand.GetCards())
                {
                    if (name == card.GetEntity().GetName())
                    {
                        MyHsHelper.MyHsHelper.EntranceQueue.Enqueue(card.GetEntity());
                        break;
                    }
                }
                if (MyHsHelper.MyHsHelper.EntranceQueue.Count >= 3) { break; }
            }
        }
    }

    public void Combat()        //战斗处理
    {
        ZonePlay zonePlay = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.FRIENDLY);
        ZonePlay enemyPlayZone = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.OPPOSING);
        MyHsHelper.MyHsHelper.Battles Diablos = new MyHsHelper.MyHsHelper.Battles();
        foreach (Card card in zonePlay.GetCards())
        {
            string name = card.GetEntity().GetName();
            MyHsHelper.MyHsHelper.Battles battles = new MyHsHelper.MyHsHelper.Battles();
            //battles.source = card.GetEntity();
            List<Entity> AbilityEntitys = GetLettuceAbilityEntitys(card.GetEntity());

            if (name == "米尔豪斯·法力风暴")
            {
                battles.Ability = selAbility(AbilityEntitys, "魔爆术");
            }

            if (name == "瓦尔登·晨拥")
            {
                battles.Ability = selAbility(AbilityEntitys, "冰风暴");
            }

            if (name == "萨尔")
            {
                battles.Ability = selAbility(AbilityEntitys, "闪电风暴");
            }

            if (name == "凯恩·血蹄")
            {
                battles.Ability = selAbility(AbilityEntitys, "大地践踏");
                if (battles.Ability.GetName().IndexOf("大地践踏") == -1)
                { 
                    battles.Ability = selAbility(AbilityEntitys, "坚韧光环");
                }
            }

            if (name == "安度因·乌瑞恩")
            {
                battles.Ability = selAbility(AbilityEntitys, "神圣新星");
                if (battles.Ability.GetName().IndexOf("苦修") > 0)
                { 
                    battles.target = HandleCards(enemyPlayZone.GetCards(), true, false, true, TAG_ROLE.TANK);
                }
            }

            if (name == "迪亚波罗")
            {
                Diablos.Ability = selAbility(AbilityEntitys, "火焰践踏");
                if (Diablos.Ability.GetName().IndexOf("火焰践踏") == -1)
                {
                    Diablos.Ability = selAbility(AbilityEntitys, "末日");
                }
                if (Diablos.Ability.GetName().IndexOf("末日") > 0)
                {
                    Diablos.target = HandleCards(enemyPlayZone.GetCards(), true, false, true, TAG_ROLE.CASTER);
                }
            }
            if (battles.Ability != null)
            { 
            MyHsHelper.MyHsHelper.BattleQueue.Enqueue(battles);
            }
        }
        if (Diablos.Ability != null)
        {
            MyHsHelper.MyHsHelper.BattleQueue.Enqueue(Diablos);
            for (int i = 0; i < MyHsHelper.MyHsHelper.BattleQueue.Count - 1; i++)
            {
                Diablos = MyHsHelper.MyHsHelper.BattleQueue.Dequeue();
                MyHsHelper.MyHsHelper.BattleQueue.Enqueue(Diablos);
            }
        }
    }

    /// <summary>
    /// 选择目标
    /// </summary>
    /// <param name="cards">目标列表</param>
    /// <param name="healthMin">攻击最低血量目标</param>
    /// <param name="healthMax">攻击最高血量目标</param>
    /// <param name="isTaunt">攻击嘲讽目标</param>
    /// <param name="tAG_ROLE">目标类型(护卫，斗士，施法者)</param>
    /// <returns>返回目标Entity</returns>
    private Entity HandleCards(List<Card> cards, bool healthMin = false, bool healthMax = false, bool isTaunt = false, TAG_ROLE tAG_ROLE = TAG_ROLE.INVALID)
    {

        foreach (Card card in cards)
        {
            if (card.GetEntity().GetMercenaryRole() == tAG_ROLE && !card.GetEntity().IsStealthed() && isTaunt)
            {
                return card.GetEntity();
            }
        }
        if (isTaunt)
        {
            foreach (Card card in cards)
            {
                if (card.GetEntity().HasTaunt() && !card.GetEntity().IsStealthed())
                {
                    return card.GetEntity();
                }
            }
        }
        Entity target = cards[0].GetEntity();
        if (healthMin)
        {
            foreach (Card card in cards)
            {
                if (card.GetEntity().GetCurrentHealth() < target.GetCurrentHealth() && !card.GetEntity().IsStealthed())
                {
                    target = card.GetEntity();
                }
            }
        }
        if (healthMax)
        {
            foreach (Card card in cards)
            {
                if (card.GetEntity().GetCurrentHealth() > target.GetCurrentHealth() && !card.GetEntity().IsStealthed())
                {
                    target = card.GetEntity();
                }
            }
        }
        return target;
    }
    private List<Entity> GetLettuceAbilityEntitys(Entity entity)
    {
        List<Entity> m_displayedAbilityEntitys = new List<Entity>();
        foreach (int id in entity.GetLettuceAbilityEntityIDs())
        {
            Entity entity3 = GameState.Get().GetEntity(id);
            if (entity3 != null && !entity3.IsLettuceEquipment())
            {
                m_displayedAbilityEntitys.Add(entity3);
            }
        }
        return m_displayedAbilityEntitys;
    }

    private Entity selAbility(List<Entity> AbilityEntitys, string AbilityName)
    {
        //MessageBox.Show(AbilityName);
        foreach (Entity AbilityEntity in AbilityEntitys)        //按技能名返回技能
        {
            string s = AbilityEntity.GetName();
            s = s.Substring(0, s.Length - 1);
            //MessageBox.Show(s);
            if (AbilityName == s && GameState.Get().HasResponse(AbilityEntity, new bool?(false)))
            {
                return AbilityEntity;
            }
        }
        //如果技能不可用则返回第一个技能
        Entity entity = new Entity();
        foreach (Entity AbilityEntity in AbilityEntitys)
        {
            if (GameState.Get().HasResponse(AbilityEntity, new bool?(false)))
            {
                entity = AbilityEntity;
                break;
            }
        }
        return entity;
    }
}





