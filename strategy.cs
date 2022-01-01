using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class experience      //如果编译多个策略请随机修改此类名，以免策略冲突
{
    //两个简单的例子,剩下的等大佬来解决。
    //最好把项目文件夹放到 .\Hearthstone\BepInEx\ 目录，这样不用再次指定引用路径。
    //编译之后把Dll放到插件同一目录即可
    public void Nomination()          //登场处理
    {
    }

    public void Combat()        //战斗处理
    {
        Random rd = new Random();
        int stime = rd.Next(1199, 1681);
        //int stime = rd.Next(100, 200);
        //MessageBox.Show(stime.ToString());
        MyHsHelper.MyHsHelper.State state = new MyHsHelper.MyHsHelper.State();
        long timestamp = (System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        while ((timestamp - state.开始时间) < stime)
        {
            System.Threading.Thread.Sleep(1000);
            timestamp = (System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            //MessageBox.Show((timestamp - state.开始时间).ToString());
        }
        //MessageBox.Show("ok");
    }
}





