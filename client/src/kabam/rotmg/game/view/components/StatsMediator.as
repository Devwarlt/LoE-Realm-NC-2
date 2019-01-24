package kabam.rotmg.game.view.components {
import com.company.assembleegameclient.objects.Player;

import kabam.rotmg.ui.signals.UpdateHUDSignal;

import robotlegs.bender.bundles.mvcs.Mediator;

public class StatsMediator extends Mediator {

    [Inject]
    public var view:StatsView;
    [Inject]
    public var updateHUD:UpdateHUDSignal;


    override public function initialize():void {
        this.updateHUD.add(this.onUpdateHUD);
    }

    override public function destroy():void {
        this.updateHUD.remove(this.onUpdateHUD);
    }

    private function onUpdateHUD(_arg1:Player):void {
        this.view.update(_arg1);
    }


}
}
