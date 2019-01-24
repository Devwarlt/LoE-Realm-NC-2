﻿package com.company.assembleegameclient.screens.charrects {
import com.company.assembleegameclient.appengine.CharacterStats;
import com.company.assembleegameclient.appengine.SavedCharacter;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.screens.events.DeleteCharacterEvent;
import com.company.assembleegameclient.ui.tooltip.MyPlayerToolTip;
import com.company.assembleegameclient.util.FameUtil;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.rotmg.graphics.DeleteXGraphic;
import com.gskinner.motion.GTween;
import com.gskinner.motion.plugins.ColorAdjustPlugin;

import flash.display.DisplayObject;
import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;

import kabam.rotmg.classes.model.CharacterClass;
import kabam.rotmg.fortune.components.ItemWithTooltip;
import kabam.rotmg.text.model.TextKey;
import kabam.rotmg.text.view.TextFieldDisplayConcrete;
import kabam.rotmg.text.view.stringBuilder.LineBuilder;
import kabam.rotmg.text.view.stringBuilder.StaticStringBuilder;

import org.osflash.signals.Signal;

public class CurrentCharacterRect extends CharacterRect {

    private static var toolTip_:MyPlayerToolTip = null;

    public const selected:Signal = new Signal();
    public const deleteCharacter:Signal = new Signal();
    public const showToolTip:Signal = new Signal(Sprite);
    public const hideTooltip:Signal = new Signal();

    public var charName:String;
    public var charStats:CharacterStats;
    public var char:SavedCharacter;
    public var myPlayerToolTipFactory:MyPlayerToolTipFactory;
    private var charType:CharacterClass;
    private var deleteButton:Sprite;
    private var icon:DisplayObject;
    private var newPetIcon:ItemWithTooltip;
    private var samplePetIcons:Array = [0x4700, 0x4701, 0x4702, 0x4703, 0x4704, 0x4705, 0x4705, 0x4706, 0x4707];
    protected var statsMaxedText:TextFieldDisplayConcrete;
    protected var statsMaxedTextLabel:TextFieldDisplayConcrete;
    protected var statsMaxedTextLabelSprite:Sprite;

    public function CurrentCharacterRect(_arg1:String, _arg2:CharacterClass, _arg3:SavedCharacter, _arg4:CharacterStats) {
        this.myPlayerToolTipFactory = new MyPlayerToolTipFactory();
        super();
        this.charName = _arg1;
        this.charType = _arg2;
        this.char = _arg3;
        this.charStats = _arg4;
        var _local5:String = _arg2.name;
        var _local6:int = _arg3.charXML_.Level;
        var _local7:int = _arg3.charXML_.NewPet;
        super.className = new LineBuilder().setParams(TextKey.CURRENT_CHARACTER_DESCRIPTION, {
            "level": _local6,
            "className": "\t" + (_local6.toString().length == 1 ? "\t" : "") + _local5
        });
        super.color = 0x5C5C5C;
        super.overColor = 0x7F7F7F;
        super.init();
        this.makeTaglineText();
        this.makeDeleteButton();
        this.makeNewPetIcon(_local7);
        this.addEventListeners();
    }

    private function addEventListeners():void {
        addEventListener(Event.REMOVED_FROM_STAGE, this.onRemovedFromStage);
        selectContainer.addEventListener(MouseEvent.CLICK, this.onSelect);
        this.deleteButton.addEventListener(MouseEvent.CLICK, this.onDelete);
    }

    private function onSelect(_arg1:MouseEvent):void {
        this.selected.dispatch(this.char);
    }

    private function onDelete(_arg1:MouseEvent):void {
        this.deleteCharacter.dispatch(this.char);
    }

    public function setIcon(_arg1:DisplayObject):void {
        ((this.icon) && (selectContainer.removeChild(this.icon)));
        this.icon = _arg1;
        this.icon.x = 4;
        this.icon.y = 4;
        ((this.icon) && (selectContainer.addChild(this.icon)));
    }

    private function makeNewPetIcon(_arg1:int):void {
        if (_arg1 == 0) {
            this.newPetIcon = new ItemWithTooltip(this.samplePetIcons[int(this.samplePetIcons.length * Math.random())], 64, false, true);
            this.newPetIcon.filters = [TextureRedrawer.matrixFilter(0x363636)];
        } else
            this.newPetIcon = new ItemWithTooltip(_arg1, 64, false, true);
        this.newPetIcon.x = 328;
        addChild(this.newPetIcon);
    }

    private function makeTaglineText():void {
        if ((this.getNextStarFame() > 0) && (Player.accountType_ <= 1)) {
            super.makeTagline(new LineBuilder().setParams(TextKey.CURRENT_CHARACTER_TAGLINE, {
                "quest": (((this.charStats == null)) ? 0 : this.charStats.numStars())
            }), new LineBuilder().setParams(TextKey.CURRENT_CHARACTER_TAGLINEFAME, {
                "fame": Parameters.addComma(this.char.fame()),
                "nextStarFame": Parameters.addComma(this.getNextStarFame())
            }), new LineBuilder().setParams(TextKey.CURRENT_CHARACTER_TAGLINEEXP, {
                "experience": Parameters.addComma(this.char.xp())
            }));
        } else {
            super.makeTagline(new LineBuilder().setParams(TextKey.CURRENT_CHARACTER_TAGLINE, {
                "quest": 5
            }), new LineBuilder().setParams(TextKey.CURRENT_CHARACTER_TAGLINEFAME_NOQUEST, {
                "fame": Parameters.addComma(this.char.fame())
            }), new LineBuilder().setParams(TextKey.CURRENT_CHARACTER_TAGLINEEXP, {
                "experience": Parameters.addComma(this.char.xp())
            }), true);
        }
        taglineClassText.x = (taglineClassText.x + taglineClassIcon.width);
        taglineFameText.x = (taglineFameText.x + taglineFameIcon.width);
        taglineExpText.x = (taglineExpText.x + tagLineExpIcon.width);
    }

    private function getNextStarFame():int {
        return (FameUtil.nextStarFame((((this.charStats == null)) ? 0 : this.charStats.bestFame()), this.char.fame()));
    }

    private function makeDeleteButton():void {
        this.deleteButton = new DeleteXGraphic();
        this.deleteButton.addEventListener(MouseEvent.MOUSE_DOWN, this.onDeleteDown);
        this.deleteButton.x = (WIDTH - 40);
        this.deleteButton.y = ((HEIGHT - this.deleteButton.height) * 0.5);
        addChild(this.deleteButton);
    }

    override protected function onMouseOver(_arg1:MouseEvent):void {
        super.onMouseOver(_arg1);
        this.removeToolTip();
        toolTip_ = this.myPlayerToolTipFactory.create(this.charName, this.char.charXML_, this.charStats);
        toolTip_.createUI();
        this.showToolTip.dispatch(toolTip_);
    }

    override protected function onRollOut(_arg1:MouseEvent):void {
        super.onRollOut(_arg1);
        this.removeToolTip();
    }

    private function onRemovedFromStage(_arg1:Event):void {
        this.removeToolTip();
    }

    private function removeToolTip():void {
        this.hideTooltip.dispatch();
    }

    private function onDeleteDown(_arg1:MouseEvent):void {
        _arg1.stopImmediatePropagation();
        dispatchEvent(new DeleteCharacterEvent(this.char));
    }


}
}
