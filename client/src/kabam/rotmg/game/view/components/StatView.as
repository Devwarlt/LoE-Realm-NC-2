package kabam.rotmg.game.view.components {
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.tooltip.TextToolTip;

import flash.display.Sprite;
import flash.events.MouseEvent;
import flash.filters.DropShadowFilter;
import flash.text.TextFieldAutoSize;

import kabam.rotmg.text.view.TextFieldDisplayConcrete;
import kabam.rotmg.text.view.stringBuilder.LineBuilder;
import kabam.rotmg.text.view.stringBuilder.StaticStringBuilder;

import org.osflash.signals.Signal;
import org.osflash.signals.natives.NativeSignal;

public class StatView extends Sprite {

    public static var toMaxTextSignal:Signal = new Signal(Boolean);

    public var fullName_:String;
    public var description_:String;
    public var nameText_:TextFieldDisplayConcrete;
    public var valText_:TextFieldDisplayConcrete;
    public var redOnZero_:Boolean;
    public var val_:int = -1;
    public var boost_:int = -1;
    public var max_:int = -1;
    public var valColor_:uint = 0xB3B3B3;
    public var level_:int = 0;
    public var toolTip_:TextToolTip;
    public var mouseOver:NativeSignal;
    public var mouseOut:NativeSignal;

    public function StatView(_arg1:String, _arg2:String, _arg3:String, _arg4:Boolean) {
        this.toolTip_ = new TextToolTip(0x363636, 0x9B9B9B, "", "", 200);
        super();
        this.fullName_ = _arg2;
        this.description_ = _arg3;
        this.nameText_ = new TextFieldDisplayConcrete().setSize(13).setColor(0xB3B3B3);
        this.nameText_.setStringBuilder(new LineBuilder().setParams(_arg1));
        this.configureTextAndAdd(this.nameText_);
        this.valText_ = new TextFieldDisplayConcrete().setSize(13).setColor(this.valColor_).setBold(true);
        this.valText_.setStringBuilder(new StaticStringBuilder("-"));
        this.configureTextAndAdd(this.valText_);
        this.redOnZero_ = _arg4;
        this.mouseOver = new NativeSignal(this, MouseEvent.MOUSE_OVER, MouseEvent);
        this.mouseOut = new NativeSignal(this, MouseEvent.MOUSE_OUT, MouseEvent);
        toMaxTextSignal.add(this.setNewText);
    }

    public function configureTextAndAdd(_arg1:TextFieldDisplayConcrete):void {
        _arg1.setAutoSize(TextFieldAutoSize.LEFT);
        _arg1.filters = [new DropShadowFilter(0, 0, 0)];
        addChild(_arg1);
    }

    public function addTooltip():void {
        this.toolTip_.setTitle(new LineBuilder().setParams(this.fullName_));
        this.toolTip_.setText(new LineBuilder().setParams(this.description_));
        if (!stage.contains(this.toolTip_)) {
            stage.addChild(this.toolTip_);
        }
    }

    public function removeTooltip():void {
        if (this.toolTip_.parent != null) {
            this.toolTip_.parent.removeChild(this.toolTip_);
        }
    }

    public function draw(statBase:int, statBoost:int, statMax:int, level:int = 0):void {
        var _local5:uint;

        if (level == this.level_ && statBase == this.val_ && statBoost == this.boost_)
            return;

        this.val_ = statBase;
        this.boost_ = statBoost;
        this.max_ = statMax;
        this.level_ = level;

        if (this.boost_ > 0)
            _local5 = 6206769;
        else
            _local5 = 0xB3B3B3;

        if (this.valColor_ != _local5) {
            this.valColor_ = _local5;
            this.valText_.setColor(this.valColor_);
        }
        this.setNewText();
    }

    public function setNewText():void {
        var _local3:int;
        var _local2:String = this.val_.toString();

        if (this.boost_ != 0)
            _local2 = (_local2 + (((" (" + (((this.boost_ > 0)) ? "+" : "")) + this.boost_.toString()) + ")"));

        this.valText_.setStringBuilder(new StaticStringBuilder(_local2));
        this.valText_.x = 24;
    }

}
}