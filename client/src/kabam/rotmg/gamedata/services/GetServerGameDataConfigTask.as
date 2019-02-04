package kabam.rotmg.gamedata.services {
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.dialogs.ErrorDialog;

import flash.external.ExternalInterface;
import flash.system.Capabilities;

import kabam.lib.tasks.BaseTask;
import kabam.rotmg.appengine.api.AppEngineClient;
import kabam.rotmg.core.StaticInjectorContext;
import kabam.rotmg.dialogs.control.OpenDialogSignal;

import robotlegs.bender.framework.api.ILogger;
import robotlegs.bender.framework.impl.Logger;

public class GetServerGameDataConfigTask extends BaseTask implements GetGameDataTask {
    private static function get getCapability():String {
        return Capabilities.playerType;
    }

    private static function get getDomain():String {
        switch (getCapability) {
            case "ActiveX":
            case "PlugIn":
                return ExternalInterface.call("window.location.href.toString");
            case "StandAlone":
                return Parameters.root.loaderInfo.url;
            case "Desktop":
            case "External":
            default:
                return "unknown";
        }
    }

    [Inject]
    public var client:AppEngineClient;
    [Inject]
    public var openDialog:OpenDialogSignal;
    [Inject]
    private var logger:Logger = StaticInjectorContext.getInjector().getInstance(ILogger);

    override protected function startTask():void {
        this.client.complete.addOnce(this.onComplete);
        this.client.setMaxRetries(3);
        this.client.sendRequest("/security/gameData", {
            "capability": getCapability,
            "domain": getDomain
        });
    }

    private function onComplete(_arg1:Boolean, _arg2:*):void {
        if (!_arg1 || _arg2.indexOf("Error") >= 0)
            onTextError(_arg2);
        else
            completeTask(_arg1);

        reset();
    }

    private function onTextError(_arg1:String):void {
        this.openDialog.dispatch(new ErrorDialog(_arg1, true));
        completeTask(false);
    }
}
}