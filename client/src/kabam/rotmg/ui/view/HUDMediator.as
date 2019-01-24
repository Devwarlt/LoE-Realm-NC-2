package kabam.rotmg.ui.view {
import com.company.assembleegameclient.objects.Player;

import flash.display.Sprite;
import kabam.rotmg.ui.model.HUDModel;
import kabam.rotmg.ui.signals.UpdateHUDSignal;

import robotlegs.bender.bundles.mvcs.Mediator;

public class HUDMediator extends Mediator {

    [Inject]
    public var view:HUDView;
    [Inject]
    public var hudModel:HUDModel;
    [Inject]
    public var updateHUD:UpdateHUDSignal;


    override public function initialize():void {
        this.updateHUD.addOnce(this.onInitializeHUD);
        this.updateHUD.add(this.onUpdateHUD);
    }

    private function dockStats(_arg1:Sprite):void {
        this.view.removeChild(_arg1);
        _arg1.stopDrag();
    }

    override public function destroy():void {
        this.updateHUD.remove(this.onUpdateHUD);
    }

    private function onUpdateHUD(_arg1:Player):void {
        this.view.draw();
    }

    private function onInitializeHUD(_arg1:Player):void {
        this.view.setPlayerDependentAssets(this.hudModel.gameSprite);
    }


}
}
