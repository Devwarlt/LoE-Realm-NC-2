package com.company.assembleegameclient.ui {
import com.company.assembleegameclient.util.FameUtil;

import flash.display.Sprite;
import flash.filters.DropShadowFilter;

import kabam.rotmg.text.model.TextKey;
import kabam.rotmg.text.view.TextFieldDisplayConcrete;
import kabam.rotmg.text.view.stringBuilder.LineBuilder;
import kabam.rotmg.text.view.stringBuilder.StaticStringBuilder;
import kabam.rotmg.ui.view.SignalWaiter;

public class RankText extends Sprite {

    public var background:Sprite = null;
    public var largeText_:Boolean;
    private var numStars_:int = -1;
    private var accountType_:int = -1;
    private var prefix_:TextFieldDisplayConcrete = null;
    private var accountTypePrefix_:TextFieldDisplayConcrete = null;
    private var waiter:SignalWaiter;
    private var icon:Sprite;

    public function RankText(numStars:int, largeText:Boolean, showPrefix:Boolean, isAccountType:Boolean = false, accountType:int = -1) {
        this.waiter = new SignalWaiter();
        super();
        this.largeText_ = largeText;
        if (showPrefix) {
            this.prefix_ = this.makeText();
            this.prefix_.setStringBuilder(new LineBuilder().setParams(TextKey.RANK_TEXT_RANK));
            this.prefix_.filters = [new DropShadowFilter(0, 0, 0)];
            this.prefix_.textChanged.addOnce(this.position);
            addChild(this.prefix_);
            /*this.accountTypePrefix_ = this.makeText2(accountType);
            this.accountTypePrefix_.setStringBuilder(new LineBuilder().setParams(GetAccountType(accountType, true).toString()));
            this.accountTypePrefix_.filters = [new DropShadowFilter(0, 0, 0)];
            this.accountTypePrefix_.y = this.prefix_.y + 24;
            addChild(this.accountTypePrefix_);*/
        }
        mouseEnabled = false;
        mouseChildren = false;
        !isAccountType ? this.draw(numStars) : this.draw2(accountType);
    }

    public function makeText():TextFieldDisplayConcrete {
        var _local1:int = ((this.largeText_) ? 18 : 16);
        var _local2:TextFieldDisplayConcrete = new TextFieldDisplayConcrete();
        _local2.setSize(_local1).setColor(0xB3B3B3);
        _local2.setBold(this.largeText_);
        return (_local2);
    }

    public function makeText2(accountType:int):TextFieldDisplayConcrete {
        var _local1:int = ((this.largeText_) ? 18 : 16);
        var _local2:TextFieldDisplayConcrete = new TextFieldDisplayConcrete();
        _local2.setSize(_local1).setColor(GetAccountTypeColor(accountType));
        _local2.setBold(this.largeText_);
        return (_local2);
    }

    internal static const REGULAR:int = 0;
    internal static const VIP:int = 1;
    internal static const MOD:int = 2;
    internal static const DEVELOPER:int = 3;
    internal static const ADMIN: int = 4;

    private static function GetAccountType(accountType:int, full:Boolean = false):String {
        var label:String = null;
        switch (accountType) {
            case VIP:
                label = "VIP";
                break;
            case MOD:
                label = "MOD";
                break;
            case DEVELOPER:
                label = "DEVELOPER";
                break;
            case ADMIN:
                label = "ADMIN";
                break;
            case REGULAR:
            default:
                label = "REGULAR";
                break;
        }
        return full ? label.concat(" Account") : label;
    }

    private static function GetAccountTypeColor(accountType:int):uint {
        switch (accountType) {
            case VIP:
                return 0xFFFF00;
            case MOD:
                return 0x1E90FF;
            case DEVELOPER:
                return 0xB8860B;
            case ADMIN:
                return 0xB03060;
            case REGULAR:
            default:
                return 0xB3B3B3;
        }
    }

    public function draw2(accountType:int):void {
        var text:TextFieldDisplayConcrete;
        if (accountType == this.accountType_) {
            return;
        }
        this.accountType_ = accountType;
        if (((!((this.background == null))) && (contains(this.background)))) {
            removeChild(this.background);
        }
        if (this.accountType_ < 0) {
            return;
        }
        this.background = new Sprite();
        text = this.makeText();
        text.setSize(12).setColor(0xFFFFFF);
        text.setVerticalAlign(TextFieldDisplayConcrete.BOTTOM);
        text.setStringBuilder(new StaticStringBuilder("Account type: " + GetAccountType(accountType)));
        text.filters = [new DropShadowFilter(0, 0, 0, 1, 4, 4, 2)];
        this.background.addChild(text);
        addChild(this.background);
    }

    public function draw(numStars:int):void {
        var text:TextFieldDisplayConcrete;
        var onTextChanged:Function;
        onTextChanged = function ():void {
            text.y = text.height;
            icon.x = (text.width + 2);
            icon.y = (text.y - icon.height);
            var _local1:int = (icon.x + icon.width);
            background.graphics.clear();
            background.graphics.beginFill(0, 0.4);
            var _local2:Number = (icon.height + 8);
            background.graphics.drawRoundRect(-2, (icon.y - 3), (_local1 + 6), _local2, 12, 12);
            background.graphics.endFill();
            position();
        };
        if (numStars == this.numStars_) {
            return;
        }
        this.numStars_ = numStars;
        if (((!((this.background == null))) && (contains(this.background)))) {
            removeChild(this.background);
        }
        if (this.numStars_ < 0) {
            return;
        }
        this.background = new Sprite();
        text = this.makeText();
        text.setVerticalAlign(TextFieldDisplayConcrete.BOTTOM);
        text.setStringBuilder(new StaticStringBuilder(this.numStars_.toString()));
        text.filters = [new DropShadowFilter(0, 0, 0, 1, 4, 4, 2)];
        this.background.addChild(text);
        this.icon = this.largeText_ ? FameUtil.numStarsToBigImage(this.numStars_) : FameUtil.numStarsToImage(this.numStars_);
        this.background.addChild(this.icon);
        text.textChanged.addOnce(onTextChanged);
        addChild(this.background);
        if (this.prefix_ != null) {
            this.positionWhenTextIsReady();
        }
    }

    private function positionWhenTextIsReady():void {
        if (this.waiter.isEmpty()) {
            this.position();
        }
        else {
            this.waiter.complete.addOnce(this.position);
        }
    }

    private function position():void {
        if (this.prefix_) {
            this.background.x = this.prefix_.width;
            this.prefix_.y = (this.icon.y - 3);
        }
    }


}
}