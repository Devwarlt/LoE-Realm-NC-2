package kabam.rotmg.maploading.view {
import flash.events.TimerEvent;
import flash.utils.Timer;

import kabam.rotmg.maploading.commands.CharacterAnimationFactory;
import kabam.rotmg.maploading.signals.HideMapLoadingSignal;
import kabam.rotmg.maploading.signals.HideMapLoadingSignalNoFade;
import kabam.rotmg.maploading.signals.MapLoadedSignal;
import kabam.rotmg.messaging.impl.incoming.MapInfo;

import robotlegs.bender.bundles.mvcs.Mediator;

public class MapLoadingMediator extends Mediator {

    [Inject]
    public var view:MapLoadingView;
    [Inject]
    public var mapLoading:MapLoadedSignal;
    [Inject]
    public var hideMapLoading:HideMapLoadingSignal;
    [Inject]
    public var hideMapLoadingNoFade:HideMapLoadingSignalNoFade;
    [Inject]
    public var characterAnimationFactory:CharacterAnimationFactory;


    override public function initialize():void {
        this.view.showAnimation(this.characterAnimationFactory.make());
        this.mapLoading.addOnce(this.onMapLoaded);
        var timer:Timer = new Timer(2000, 1);
        timer.addEventListener(TimerEvent.TIMER_COMPLETE, this.onHide);
        timer.start();
        //this.hideMapLoading.add(this.onHide);
        //this.hideMapLoadingNoFade.add(this.onHideNoFade);
    }

    private function onMapLoaded(_arg1:MapInfo):void {
        this.view.showMap(_arg1.displayName_, _arg1.difficulty_);
    }

    override public function destroy():void {
        this.hideMapLoading.remove(this.onHide);
    }

    private function onHide(_arg1:TimerEvent):void {
        this.view.disable();
    }

    private function onHideNoFade():void {
        this.view.disableNoFadeOut();
    }


}
}
