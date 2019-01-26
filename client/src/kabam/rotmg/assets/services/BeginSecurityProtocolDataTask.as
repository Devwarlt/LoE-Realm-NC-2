package kabam.rotmg.assets.services {
import com.adobe.crypto.SHA256;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.dialogs.ErrorDialog;

import kabam.lib.tasks.BaseTask;
import kabam.rotmg.appengine.api.AppEngineClient;
import kabam.rotmg.core.signals.SetLoadingMessageSignal;
import kabam.rotmg.dialogs.control.OpenDialogSignal;
import kabam.rotmg.text.model.NumberKey;
import kabam.rotmg.text.model.TextKey;

public class BeginSecurityProtocolDataTask extends BaseTask {
    [Inject]
    public var client:AppEngineClient;
    [Inject]
    public var setLoadingMessage:SetLoadingMessageSignal;
    [Inject]
    public var openDialog:OpenDialogSignal;

    //LOESOFT Hash
    private var LOESOFT_HASH:String = "loesoft_";

    //AES-128-CFB Algorithm
    private var protocolToken:String = Parameters.TOKEN;

    //RC4-40 Algorithm [length - 3 + Negative]
    private var protocolID:String = (Parameters.ID).toString();

    private var playerData:String = TextKey.PLAYER;
    private var rateoffireData:String = TextKey.RATEOFFIRE;
    private var rateoffireValueData:String = (NumberKey.RATEOFFIREVALUE).toString();
    private var mpcostData:String = TextKey.MPCOST;
    private var numprojectilesValueData:String = (NumberKey.NUMPROJECTILESVALUE).toString();
    private var arcgapValueData:String = (NumberKey.ARCGAPVALUE).toString();
    private var cooldownData:String = TextKey.COOLDOWN;
    private var cooldownValueData:String = (NumberKey.COOLDOWNVALUE).toString();

    override protected function startTask():void {
        var ParseJSONSecurityProtocols:Object = {
            "LOESOFT_HASH": SHA256.hash(this.LOESOFT_HASH),
            "protocolToken": SHA256.hash(this.LOESOFT_HASH + this.protocolToken),
            "protocolID": SHA256.hash(this.LOESOFT_HASH + this.protocolID),
            "playerData": SHA256.hash(this.LOESOFT_HASH + this.playerData),
            "rateoffireData": SHA256.hash(this.LOESOFT_HASH + this.rateoffireData),
            "rateoffireValueData": SHA256.hash(this.LOESOFT_HASH + this.rateoffireValueData),
            "mpcostData": SHA256.hash(this.LOESOFT_HASH + this.mpcostData),
            "numprojectilesValueData": SHA256.hash(this.LOESOFT_HASH + this.numprojectilesValueData),
            "arcgapValueData": SHA256.hash(this.LOESOFT_HASH + this.arcgapValueData),
            "cooldownData": SHA256.hash(this.LOESOFT_HASH + this.cooldownData),
            "cooldownValueData": SHA256.hash(this.LOESOFT_HASH + this.cooldownValueData),
            "crudeLOESOFT_HASH": (this.LOESOFT_HASH),
            "crudeprotocolToken": (this.LOESOFT_HASH + this.protocolToken),
            "crudeprotocolID": (this.LOESOFT_HASH + this.protocolID),
            "crudeplayerData": (this.LOESOFT_HASH + this.playerData),
            "cruderateoffireData": (this.LOESOFT_HASH + this.rateoffireData),
            "cruderateoffireValueData": (this.LOESOFT_HASH + this.rateoffireValueData),
            "crudempcostData": (this.LOESOFT_HASH + this.mpcostData),
            "crudenumprojectilesValueData": (this.LOESOFT_HASH + this.numprojectilesValueData),
            "crudearcgapValueData": (this.LOESOFT_HASH + this.arcgapValueData),
            "crudecooldownData": (this.LOESOFT_HASH + this.cooldownData),
            "crudecooldownValueData": (this.LOESOFT_HASH + this.cooldownValueData)
        };
        this.client.complete.addOnce(this.onComplete);
        this.client.setMaxRetries(3);
        this.client.sendRequest("/security/securityProtocols", ParseJSONSecurityProtocols);
    }

    private function onComplete(success:Boolean, data:*):void {
        if (!success || data.length == 8)
            onTextError();

        completeTask(success, data);
    }

    private function onTextError():void {
        var _local1:String = (
            "Max number of attempts reached limit and server detected invalid protocols into " +
            "your client, make sure to use properly game client to avoid this message.\n\n" +
            "Kind Regards, LoESoft"
        );
        var _local2:ErrorDialog = new ErrorDialog(_local1, true);
        this.openDialog.dispatch(_local2);
        completeTask(false);
    }
}
}
