package io.decagames.rotmg.ui.labels {
import flash.text.TextField;
import flash.text.TextFieldAutoSize;
public class UILabel extends TextField {

    public static var DEBUG:Boolean = false;

    public function UILabel(){
        if (DEBUG)
            this.debugDraw();
        this.embedFonts = true;
        this.selectable = false;
        this.autoSize = TextFieldAutoSize.LEFT;
    }
    private function debugDraw():void{
        this.border = true;
        this.borderColor = 0xFF0000;
    }
    override public function set y(_arg1:Number):void{
        super.y = _arg1;
    }
    override public function get textWidth():Number{
        return ((super.textWidth + 4));
    }

}
}
