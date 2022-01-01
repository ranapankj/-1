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
            fire = new List<string>(new string[] { "米尔豪斯", "瓦尔登", "萨尔", "凯恩", "安度因", "迪亚波罗" });

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
        //MessageBox.Show("战斗处理");
        ZonePlay zonePlay = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.FRIENDLY);
        ZonePlay enemyPlayZone = ZoneMgr.Get().FindZoneOfType<ZonePlay>(global::Player.Side.OPPOSING);
        List<string> fire = new List<string>(new string[] { "米尔豪斯", "瓦尔登", "萨尔", "凯恩", "安度因", "迪亚波罗" });
        List<string> AbilityNames = new List<string>(new string[] { "魔爆术", "冰风暴", "闪电风暴", "大地践踏", "神圣新星", "苦修", "火焰践踏", "末日" });
        foreach (string name in fire)
        {
            foreach (Card card in zonePlay.GetCards())
            {
                if (name == card.GetEntity().GetName())
                {
                    MyHsHelper.MyHsHelper.Battles battles = new MyHsHelper.MyHsHelper.Battles();
                    //battles.source = card.GetEntity();
                    List<Entity> AbilityEntitys = GetLettuceAbilityEntitys(card.GetEntity());

                    foreach (string AbilityName in AbilityNames)
                    {
                        foreach (Entity AbilityEntity in AbilityEntitys)
                        {
                            string s = AbilityEntity.GetName();
                            s = s.Substring(0, s.Length - 1);
                            if (AbilityName == s && GameState.Get().HasResponse(AbilityEntity, new bool?(false)))
                            {
                                battles.Ability = AbilityEntity;
                                break;
                            }
                        }
                        if (battles.Ability != null) { break; }
                    }

                    if (name == "安度因")
                    {
                        battles.target = HandleCards(enemyPlayZone.GetCards(), true, false, false, TAG_ROLE.TANK);
                    }

                    if (name == "迪亚波罗")
                    {
                        battles.target = HandleCards(enemyPlayZone.GetCards(),true,false, true, TAG_ROLE.CASTER);
                    }
                    MyHsHelper.MyHsHelper.BattleQueue.Enqueue(battles);
                }
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
    private Entity HandleCards(List<Card> cards,bool healthMin = false , bool healthMax = false ,bool isTaunt = false, TAG_ROLE tAG_ROLE = TAG_ROLE.INVALID)
    {
        foreach (Card card in cards)
        {
            if (card.GetEntity().GetMercenaryRole() == tAG_ROLE  && !card.GetEntity().IsStealthed() && isTaunt)
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

}





