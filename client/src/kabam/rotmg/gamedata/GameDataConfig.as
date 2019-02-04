package kabam.rotmg.gamedata {
import kabam.rotmg.gamedata.services.GetServerGameDataConfigTask;
import kabam.rotmg.startup.control.StartupSequence;

import org.swiftsuspenders.Injector;

import robotlegs.bender.framework.api.IConfig;

public class GameDataConfig implements IConfig {

    [Inject]
    public var injector:Injector;
    [Inject]
    public var startup:StartupSequence;

    public function configure():void {
        this.injector.map(GetServerGameDataConfigTask);
        this.startup.addTask(GetServerGameDataConfigTask, -997);
    }
}
}
