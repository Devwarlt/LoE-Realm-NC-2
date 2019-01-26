package kabam.rotmg.game.view.components {
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.StatusBar;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.util.AssetLibrary;

import flash.display.Bitmap;

import flash.display.Sprite;
import flash.filters.DropShadowFilter;
import flash.geom.ColorTransform;

import kabam.rotmg.pets.util.PetsViewAssetFactory;

import kabam.rotmg.text.model.TextKey;
import kabam.rotmg.text.view.TextFieldDisplayConcrete;
import kabam.rotmg.text.view.stringBuilder.LineBuilder;
import kabam.rotmg.text.view.stringBuilder.StaticStringBuilder;

public class StatsView extends Sprite {
    private var contents_:Sprite;
    private var attackBarBackground_:StatusBar;
    private var attackBar_:StatusBar;
    private var defenseBarBackground_:StatusBar;
    private var defenseBar_:StatusBar;
    private var expIcon_:Bitmap;
    private var expText_:TextFieldDisplayConcrete;
    private var expGoalIcon_:Bitmap;
    private var expGoalText_:TextFieldDisplayConcrete;
    private var regularStats_:Sprite;
    private var attackIcon_:Bitmap;
    private var attackText_:TextFieldDisplayConcrete;
    private var defenseIcon_:Bitmap;
    private var defenseText_:TextFieldDisplayConcrete;
    private var speedIcon_:Bitmap;
    private var speedText_:TextFieldDisplayConcrete;
    private var dexterityIcon_:Bitmap;
    private var dexterityText_:TextFieldDisplayConcrete;
    private var vitalityIcon_:Bitmap;
    private var vitalityText_:TextFieldDisplayConcrete;
    private var wisdomIcon_:Bitmap;
    private var wisdomText_:TextFieldDisplayConcrete;

    public function StatsView() {
        super();
        this.createSpriteContent();
        this.createAttDefBars();
        this.createStatsText();
        this.contents_.addChild(this.regularStats_);
        addChild(this.contents_);
    }

    private function createSpriteContent():void {
        this.contents_ = new Sprite();
        this.contents_.graphics.clear();
        this.contents_.graphics.beginFill(0, 0);
        this.contents_.graphics.drawRect(0, 0, 186, 112);
        this.regularStats_ = new Sprite();
        this.regularStats_.filters = [TextureRedrawer.OUTLINE_FILTER];
    }

    private function createAttDefBars():void {
        this.attackBarBackground_ = new StatusBar(176, 16, 0x545454, 0x545454, null);
        this.attackBar_ = new StatusBar(176, 16, 0xFF1493, 0x545454, TextKey.EXP_BAR_LEVEL, true, 10, true);
        this.defenseBarBackground_ = new StatusBar(176, 16, 0x545454, 0x545454, null);
        this.defenseBar_ = new StatusBar(176, 16, 0x000000, 0x545454, TextKey.EXP_BAR_LEVEL, true, 10, true);
        this.contents_.addChild(this.attackBarBackground_);
        this.contents_.addChild(this.attackBar_);
        this.contents_.addChild(this.defenseBarBackground_);
        this.contents_.addChild(this.defenseBar_);
        this.attackBar_.x = this.attackBarBackground_.x =
                this.defenseBar_.x = this.defenseBarBackground_.x = 4;
        this.attackBar_.y = this.attackBarBackground_.y = 4;
        this.defenseBar_.y = this.defenseBarBackground_.y = 24;
    }

    private function createStatsText():void {
        this.expIcon_ = new Bitmap(AssetLibrary.getImageFromSet("lofiInterfaceBig", 15));
        this.expIcon_.transform.colorTransform = new ColorTransform((0x00 / 0xFF), (0xFA / 0xFF), (0x9A / 0xFF));
        this.expIcon_.filters = [new DropShadowFilter(0, 0, 0)];
        this.expIcon_.x = 4;
        this.expIcon_.y = 44;
        this.contents_.addChild(this.expIcon_);
        this.expText_ = PetsViewAssetFactory.returnTextfield(0x00FF7F, 12, false, true).setHTML(true);
        this.expText_.filters = [new DropShadowFilter(0, 0, 0)];
        this.expText_.x = 18;
        this.expText_.y = 56;
        this.contents_.addChild(this.expText_);

        this.expGoalIcon_ = new Bitmap(AssetLibrary.getImageFromSet("lofiInterfaceBig", 15));
        this.expGoalIcon_.transform.colorTransform = new ColorTransform((0xFF / 0xFF), (0xD7 / 0xFF), (0x00 / 0xFF));
        this.expGoalIcon_.filters = [new DropShadowFilter(0, 0, 0)];
        this.expGoalIcon_.x = this.expIcon_.x;
        this.expGoalIcon_.y = this.expIcon_.y + 16;
        this.contents_.addChild(this.expGoalIcon_);
        this.expGoalText_ = PetsViewAssetFactory.returnTextfield(0xEEDD82, 12, false, true).setHTML(true);
        this.expGoalText_.filters = [new DropShadowFilter(0, 0, 0)];
        this.expGoalText_.x = this.expText_.x;
        this.expGoalText_.y = this.expText_.y + 16;
        this.contents_.addChild(this.expGoalText_);

        this.attackIcon_ = new Bitmap(AssetLibrary.getImageFromSet("lofiInterfaceBig", 34));
        this.attackIcon_.filters = [TextureRedrawer.matrixFilter(0x9400D3)];
        this.attackIcon_.x = this.expText_.x - 12;
        this.attackIcon_.y = this.expGoalIcon_.y + 26;
        this.regularStats_.addChild(this.attackIcon_);
        this.attackText_ = PetsViewAssetFactory.returnTextfield(0xDA70D6, 10, true, true).setHTML(true);
        this.attackText_.filters = [new DropShadowFilter(0, 0, 0)];
        this.attackText_.x = this.expGoalText_.x + 5;
        this.attackText_.y = this.expGoalText_.y + 25;
        this.contents_.addChild(this.attackText_);

        this.defenseIcon_ = new Bitmap(AssetLibrary.getImageFromSet("lofiInterfaceBig", 35));
        this.defenseIcon_.filters = [TextureRedrawer.matrixFilter(0x708090)];
        this.defenseIcon_.x = this.attackIcon_.x;
        this.defenseIcon_.y = this.attackIcon_.y + 16;
        this.regularStats_.addChild(this.defenseIcon_);
        this.defenseText_ = PetsViewAssetFactory.returnTextfield(0xD3D3D3, 10, true, true).setHTML(true);
        this.defenseText_.filters = [new DropShadowFilter(0, 0, 0)];
        this.defenseText_.x = this.attackText_.x;
        this.defenseText_.y = this.attackText_.y + 16;
        this.contents_.addChild(this.defenseText_);

        this.speedIcon_ = new Bitmap(AssetLibrary.getImageFromSet("lofiInterfaceBig", 36));
        this.speedIcon_.filters = [TextureRedrawer.matrixFilter(0x32CD32)];
        this.speedIcon_.x = this.attackIcon_.x + 60;
        this.speedIcon_.y = this.attackIcon_.y;
        this.regularStats_.addChild(this.speedIcon_);
        this.speedText_ = PetsViewAssetFactory.returnTextfield(0x9ACD32, 10, true, true).setHTML(true);
        this.speedText_.filters = [new DropShadowFilter(0, 0, 0)];
        this.speedText_.x = this.attackText_.x + 60;
        this.speedText_.y = this.attackText_.y;
        this.contents_.addChild(this.speedText_);

        this.dexterityIcon_ = new Bitmap(AssetLibrary.getImageFromSet("lofiInterfaceBig", 37));
        this.dexterityIcon_.filters = [TextureRedrawer.matrixFilter(0xCD6600)];
        this.dexterityIcon_.x = this.speedIcon_.x;
        this.dexterityIcon_.y = this.speedIcon_.y + 16;
        this.regularStats_.addChild(this.dexterityIcon_);
        this.dexterityText_ = PetsViewAssetFactory.returnTextfield(0xFFA500, 10, true, true).setHTML(true);
        this.dexterityText_.filters = [new DropShadowFilter(0, 0, 0)];
        this.dexterityText_.x = this.speedText_.x;
        this.dexterityText_.y = this.speedText_.y + 16;
        this.contents_.addChild(this.dexterityText_);

        this.vitalityIcon_ = new Bitmap(AssetLibrary.getImageFromSet("lofiInterfaceBig", 38));
        this.vitalityIcon_.filters = [TextureRedrawer.matrixFilter(0xCD2626)];
        this.vitalityIcon_.x = this.speedIcon_.x + 60;
        this.vitalityIcon_.y = this.speedIcon_.y;
        this.regularStats_.addChild(this.vitalityIcon_);
        this.vitalityText_ = PetsViewAssetFactory.returnTextfield(0xFF3030, 10, true, true).setHTML(true);
        this.vitalityText_.filters = [new DropShadowFilter(0, 0, 0)];
        this.vitalityText_.x = this.speedText_.x + 60;
        this.vitalityText_.y = this.speedText_.y;
        this.contents_.addChild(this.vitalityText_);

        this.wisdomIcon_ = new Bitmap(AssetLibrary.getImageFromSet("lofiInterfaceBig", 39));
        this.wisdomIcon_.filters = [TextureRedrawer.matrixFilter(0x3A5FCD)];
        this.wisdomIcon_.x = this.vitalityIcon_.x;
        this.wisdomIcon_.y = this.vitalityIcon_.y + 16;
        this.regularStats_.addChild(this.wisdomIcon_);
        this.wisdomText_ = PetsViewAssetFactory.returnTextfield(0x4876FF, 10, true, true).setHTML(true);
        this.wisdomText_.filters = [new DropShadowFilter(0, 0, 0)];
        this.wisdomText_.x = this.vitalityText_.x;
        this.wisdomText_.y = this.vitalityText_.y + 16;
        this.contents_.addChild(this.wisdomText_);
    }

    public function update(_arg1:Player):void {
        if (!_arg1)
            return;

        // update att bar
        this.attackBar_.setLabelText(TextKey.ATT_EXP_BAR_LEVEL, {"level": _arg1.attackLevel_});
        this.attackBarBackground_.draw(1, 1, 0, 1);
        this.attackBar_.draw(_arg1.attackExp_, _arg1.nextAttackExp_, 0);

        // update def bar
        this.defenseBar_.setLabelText(TextKey.DEF_EXP_BAR_LEVEL, {"level": _arg1.defenseLevel_});
        this.defenseBarBackground_.draw(1, 1, 0, 1);
        this.defenseBar_.draw(_arg1.defenseExp_, _arg1.nextDefenseExp_, 0);

        //update labels
        this.expText_.setStringBuilder(new LineBuilder().setParams(Parameters.formatValue((_arg1.exp_ + getExperienceBase(_arg1.level_))) + " <b>XP</b>"));
        this.expGoalText_.setStringBuilder(new LineBuilder().setParams(Parameters.formatValue(_arg1.nextLevelExp_ - _arg1.exp_) + " <b>XP</b>\nto Level <b>" + (_arg1.level_ + 1) + "</b>"));
        this.attackText_.setStringBuilder(new LineBuilder().setParams(makeStatsFormattedString(_arg1.attackLevel_, _arg1.attackBoost_, true)));
        this.defenseText_.setStringBuilder(new LineBuilder().setParams(makeStatsFormattedString(_arg1.defenseLevel_, _arg1.defenseBoost_, true)));
        this.speedText_.setStringBuilder(new LineBuilder().setParams(makeStatsFormattedString2(_arg1.speed_, _arg1.speedBoost_)));
        this.dexterityText_.setStringBuilder(new LineBuilder().setParams(makeStatsFormattedString2(_arg1.dexterity_, _arg1.dexterityBoost_)));
        this.vitalityText_.setStringBuilder(new LineBuilder().setParams(makeStatsFormattedString2(_arg1.vitality_, _arg1.vitalityBoost_)));
        this.wisdomText_.setStringBuilder(new LineBuilder().setParams(makeStatsFormattedString2(_arg1.wisdom_, _arg1.wisdomBoost_)));
    }

    private static function makeStatsFormattedString(_arg1:int, _arg2:String = "0", _arg3:Boolean = false):String {
        return makeStatsFormattedNumber(_arg1, Number(_arg2), _arg3);
    }

    private static function makeStatsFormattedString2(_arg1:String, _arg2:String = "0", _arg3:Boolean = false):String {
        return makeStatsFormattedNumber(Number(_arg1), Number(_arg2), _arg3);
    }

    private static function makeStatsFormattedNumber(_arg1:int, _arg2:int = 0, _arg3:Boolean = false):String {
        var _local1:int = _arg2 > 0 ? Math.abs(_arg1 - _arg2) : _arg2 + _arg1;
        return (!_arg3 ? (_local1) : (_arg1)) + "" + (_arg2 > 0 ? " + " + _arg2 : "");
    }

    private static function getExperienceBase(_arg1:int, _arg2:Boolean = false):Number {
        return _arg1 == 1 ? 0 : ((75 * _arg1 * _arg1 * _arg1 - 125 * _arg1 *_arg1 + 900 * _arg1) / (!_arg2 ? 2 : 20));
    }
}
}
