package com.company.assembleegameclient.ui.tooltip {

import com.company.assembleegameclient.appengine.CharacterStats;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.ui.GameObjectListItem;
import com.company.assembleegameclient.ui.LineBreakDesign;
import com.company.assembleegameclient.ui.panels.itemgrids.EquippedGrid;
import com.company.assembleegameclient.ui.panels.itemgrids.InventoryGrid;
import com.company.assembleegameclient.util.AnimatedChars;
import com.company.assembleegameclient.util.FameUtil;
import com.company.assembleegameclient.util.TextureRedrawer;

import flash.filters.DropShadowFilter;

import kabam.rotmg.classes.model.CharacterClass;
import kabam.rotmg.classes.model.CharacterSkin;
import kabam.rotmg.classes.model.ClassesModel;
import kabam.rotmg.constants.GeneralConstants;
import kabam.rotmg.core.StaticInjectorContext;
import kabam.rotmg.core.model.PlayerModel;
import kabam.rotmg.text.model.TextKey;
import kabam.rotmg.text.view.TextFieldDisplayConcrete;
import kabam.rotmg.text.view.stringBuilder.LineBuilder;

public class MyPlayerToolTip extends ToolTip {

    private var classes:ClassesModel;
    public var player_:Player;
    private var playerPanel_:GameObjectListItem;
    private var lineBreak_:LineBreakDesign;
    private var bestLevel_:TextFieldDisplayConcrete;
    private var nextClassQuest_:TextFieldDisplayConcrete;
    private var eGrid:EquippedGrid;
    private var iGrid:InventoryGrid;
    private var bGrid:InventoryGrid;
    private var accountName:String;
    private var charXML:XML;
    private var charStats:CharacterStats;
    private var _XOffset:int = 4;

    public function MyPlayerToolTip(_arg1:String, _arg2:XML, _arg3:CharacterStats) {
        super(0x363636, 1, 0xFFFFFF, 1);
        this.accountName = _arg1;
        this.charXML = _arg2;
        this.charStats = _arg3;
    }

    public function createUI():void {
        var _local5:Number;
        this.classes = StaticInjectorContext.getInjector().getInstance(ClassesModel);
        var _local1:int = int(this.charXML.ObjectType);
        var _local2:XML = ObjectLibrary.xmlLibrary_[_local1];
        this.player_ = Player.fromPlayerXML(this.accountName, this.charXML);
        this.player_.accountId_ = StaticInjectorContext.getInjector().getInstance(PlayerModel).charList.accountId_;
        var _local3:CharacterClass = this.classes.getCharacterClass(this.player_.objectType_);
        var _local4:CharacterSkin = _local3.skins.getSkin(this.charXML.Texture);
        this.player_.animatedChar_ = AnimatedChars.getAnimatedChar(_local4.template.file, _local4.template.index);
        this.playerPanel_ = new GameObjectListItem(0xB3B3B3, true, this.player_, false, true);
        this.playerPanel_.x = this._XOffset;
        addChild(this.playerPanel_);
        _local5 = 36;
        this.eGrid = new EquippedGrid(null, this.player_.slotTypes_, this.player_);
        this.eGrid.filters = [TextureRedrawer.OUTLINE_FILTER];
        this.eGrid.x = 8 + this._XOffset;
        this.eGrid.y = _local5;
        addChild(this.eGrid);
        this.eGrid.setItems(this.player_.equipment_);
        _local5 = (_local5 + 44);
        this.iGrid = new InventoryGrid(null, this.player_, GeneralConstants.NUM_EQUIPMENT_SLOTS);
        this.iGrid.filters = [TextureRedrawer.OUTLINE_FILTER];
        this.iGrid.x = 8 + this._XOffset;
        this.iGrid.y = _local5;
        addChild(this.iGrid);
        this.iGrid.setItems(this.player_.equipment_);
        _local5 = (_local5 + 88);
        if (this.player_.hasBackpack_) {
            this.bGrid = new InventoryGrid(null, this.player_, GeneralConstants.NUM_EQUIPMENT_SLOTS * 3);
            this.bGrid.filters = [TextureRedrawer.OUTLINE_FILTER];
            this.bGrid.x = 8 + this._XOffset;
            this.bGrid.y = _local5;
            addChild(this.bGrid);
            this.bGrid.setItems(this.player_.equipment_);
            _local5 = (_local5 + 88);
        }
        _local5 = (_local5 + 8);
        this.lineBreak_ = new LineBreakDesign(100, 0x1C1C1C);
        this.lineBreak_.x = 6;
        this.lineBreak_.y = _local5;
        addChild(this.lineBreak_);
        this.makeBestLevelText();
        this.bestLevel_.x = 8 + this._XOffset;
        this.bestLevel_.y = (height - 2);
        var _local6:int = FameUtil.nextStarFame(this.charStats == null ? 0 : this.charStats.bestFame(), 0);
        if (_local6 > 0 && this.player_.accountType_ < 2)
            this.makeNextClassQuestText(_local6, _local2);
    }

    public function makeNextClassQuestText(_arg1:int, _arg2:XML):void {
        this.nextClassQuest_ = new TextFieldDisplayConcrete().setSize(13).setColor(16549442).setTextWidth(174);
        this.nextClassQuest_.setStringBuilder(new LineBuilder().setParams(TextKey.MY_PLAYER_TOOL_TIP_NEXT_CLASS_QUEST, {
            "nextStarFame": _arg1,
            "character": ClassToolTip.getDisplayId(_arg2)
        }));
        this.nextClassQuest_.filters = [new DropShadowFilter(0, 0, 0)];
        addChild(this.nextClassQuest_);
        waiter.push(this.nextClassQuest_.textChanged);
    }

    public function makeBestLevelText():void {
        this.bestLevel_ = new TextFieldDisplayConcrete().setSize(14).setColor(6206769);
        var _local1:int = (((this.charStats == null)) ? 0 : this.charStats.numStars());
        var _local2:String = (((this.charStats) != null) ? this.charStats.bestLevel() : 0).toString();
        var _local3:String = (((this.charStats) != null) ? this.charStats.bestFame() : 0).toString();
        this.bestLevel_.setStringBuilder(new LineBuilder().setParams(TextKey.BESTLEVEL__STATS, {
            "numStars": _local1,
            "bestLevel": _local2,
            "fame": _local3
        }));
        this.bestLevel_.filters = [new DropShadowFilter(0, 0, 0)];
        addChild(this.bestLevel_);
        waiter.push(this.bestLevel_.textChanged);
    }

    override protected function alignUI():void {
        if (this.nextClassQuest_) {
            this.nextClassQuest_.x = 8 + this._XOffset;
            this.nextClassQuest_.y = (this.bestLevel_.getBounds(this).bottom - 2);
        }
    }

    override public function draw():void {
        this.lineBreak_.setWidthColor((width - 10), 0x1C1C1C);
        super.draw();
    }


}
}
