package com.company.assembleegameclient.util {
import io.decagames.rotmg.ui.labels.UILabel;
import flash.text.TextFormat;

import kabam.rotmg.text.model.FontModel;
import flash.filters.GlowFilter;

public class TierUtil {
    private static const STANDARD_OUTLINE_FILTER:Array = [new GlowFilter(0, 1, 2, 2, 10, 1)];

    public static const UNTIERED_COLOR:uint = 9055202;
    public static const SET_COLOR:uint = 0xFF9900;
    public static const LT_COLOR:uint = 0xFF6347;
    public static const SRT_COLOR:uint = 0xFFD700;

    public static function getTierTag(_arg1:XML, _arg2:int=12):UILabel{
        var label:UILabel;
        var color:Number;
        var text:String;
        var _local3:Boolean = (isPet(_arg1) == false);
        var _local4:Boolean = (_arg1.hasOwnProperty("Consumable") == false);
        var _local5:Boolean = (_arg1.hasOwnProperty("InvUse") == false);
        var _local6:Boolean = (_arg1.hasOwnProperty("Treasure") == false);
        var _local7:Boolean = (_arg1.hasOwnProperty("PetFood") == false);
        var _local8:Boolean = _arg1.hasOwnProperty("Tier");
        var _local9:Boolean = (_arg1.hasOwnProperty("LT"));
        var _local10:Boolean = (_arg1.hasOwnProperty("SRT"));
        if (((((((((_local3) && (_local4))) && (_local5))) && (_local6))) && (_local7)))
        {
            label = new UILabel();
            if (_local8)
            {
                color = 0xFFFFFF;
                text = ("T" + _arg1.Tier);
            }
            else if (_local9) {
                color = LT_COLOR;
                text = "LT";
            }
            else if (_local10) {
                color = SRT_COLOR;
                text = "SRT";
            }
            else
            {
                if (_arg1.hasOwnProperty("@setType"))
                {
                    color = SET_COLOR;
                    text = "ST";
                } else
                {
                    color = UNTIERED_COLOR;
                    text = "UT";
                }
            }
            label.text = text;
            tierLevelLabel(label, _arg2, color);
            return (label);
        }
        return (null);
    }

    public static function isPet(_arg1:XML):Boolean{
        var activateTags:XMLList;
        var itemDataXML:XML = _arg1;
        activateTags = itemDataXML.Activate.(text() == "PermaPet");
        return ((activateTags.length() >= 1));
    }

    public static function tierLevelLabel(_arg1:UILabel, _arg2:int=12, _arg3:Number=0xFFFFFF, _arg4:String="right"):void{
        createLabelFormat(_arg1, _arg2, _arg3, _arg4, true);
    }

    public static function createLabelFormat(_arg1:UILabel, _arg2:int=12, _arg3:Number=0xFFFFFF, _arg4:String="left", _arg5:Boolean=false, _arg6:Array=null):void{
        var _local7:TextFormat = createTextFormat(_arg2, _arg3, _arg4, _arg5);
        applyTextFormat(_local7, _arg1);
        if (_arg6)
            _arg1.filters = _arg6;
    }

    public static function createTextFormat(_arg1:int, _arg2:uint, _arg3:String, _arg4:Boolean):TextFormat{
        var _local5:TextFormat = new TextFormat();
        _local5.color = _arg2;
        _local5.bold = _arg4;
        _local5.font = FontModel.DEFAULT_FONT_NAME;
        _local5.size = _arg1;
        _local5.align = _arg3;
        return (_local5);
    }

    private static function applyTextFormat(_arg1:TextFormat, _arg2:UILabel):void{
        _arg2.defaultTextFormat = _arg1;
        _arg2.setTextFormat(_arg1);
    }

    public static function getTextOutlineFilter():Array{
        return (STANDARD_OUTLINE_FILTER);
    }
}
}
