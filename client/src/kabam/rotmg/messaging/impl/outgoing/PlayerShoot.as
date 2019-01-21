package kabam.rotmg.messaging.impl.outgoing {
import flash.utils.IDataOutput;

import kabam.rotmg.messaging.impl.data.WorldPosData;

public class PlayerShoot extends OutgoingMessage
{
    public var bulletId_:uint;
    public var containerType_:int;
    public var startingPos_:WorldPosData;
    public var angle_:Number;
    public var attackAmount_:int;
    public var isDazed_:Boolean;
    public var isBeserk_:Boolean;
    public var minAttackFrequency_:Number;
    public var maxAttackFrequency_:Number;
    public var weaponRateOfFire_:Number;
    public var isAbility_:Boolean;

    public function PlayerShoot(_arg1:uint, _arg2:Function) {
        this.startingPos_ = new WorldPosData();
        super(_arg1, _arg2);
    }

    override public function writeToOutput(_arg1:IDataOutput):void {
        _arg1.writeByte(this.bulletId_);
        _arg1.writeShort(this.containerType_);
        this.startingPos_.writeToOutput(_arg1);
        _arg1.writeFloat(this.angle_);
        _arg1.writeInt(this.attackAmount_);
        _arg1.writeBoolean(this.isDazed_);
        _arg1.writeBoolean(this.isBeserk_);
        _arg1.writeFloat(this.minAttackFrequency_);
        _arg1.writeFloat(this.maxAttackFrequency_);
        _arg1.writeFloat(this.weaponRateOfFire_);
        _arg1.writeBoolean(this.isAbility_);
    }

    override public function toString():String {
        return (formatToString("PLAYERSHOOT", "bulletId_", "containerType_", "startingPos_", "angle_", "attackAmount_", "isDazed_", "isBeserk_", "minAttackFrequency_", "maxAttackFrequency_", "weaponRateOfFire_", "isAbility_"));
    }
}
}